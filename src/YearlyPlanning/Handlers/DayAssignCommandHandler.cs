using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.Messaging;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using System;
using Infrastructure.Constants;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Commands.OperationalTaskCommands;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Handlers
{
    public class DayAssignCommandHandler :
        IHandler<CreateDayAssignCommand>,
        IHandler<ChangeDayAssignDateCommand>,
        IHandler<ChangeDayAssignEstimatedMinutesCommand>,
        IHandler<ChangeDayAssignMembersComand>,
        IHandler<RemoveDayAssignMembersCommand>,
        IHandler<ChangeJobIdAndJobAssignIdInDayAssignCommand>,
        IHandler<ChangeDayAssignStatusCommand>,
        IHandler<ChangeTenantTaskTypeCommand>,
        IHandler<ChangeTenantTaskUrgencyCommand>,
        IHandler<ChangeOperationalTaskDateCommand>,
        IHandler<ChangeOperationalTaskTimeCommand>,
        IHandler<ChangeTenantTaskResidentNameCommand>, 
        IHandler<ChangeTenantTaskResidentPhoneCommand>,
        IHandler<ChangeTenantTaskCommentCommand>
    {
        private readonly IAggregateRootRepository<DayAssignDomain> repository;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly IAppSettingHelper appSettingHelper;

        public DayAssignCommandHandler(IAggregateRootRepository<DayAssignDomain> repository, 
                                       IDayAssignProvider dayAssignProvider,
                                       IAppSettingHelper appSettingHelper)
        {
            this.repository = repository;
            this.dayAssignProvider = dayAssignProvider;
            this.appSettingHelper = appSettingHelper;
        }

        public async Task Handle(CreateDayAssignCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id);
                if (item != null)
                {
                    throw new Exception($"DayAssign with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }

            var time = GetTime(message.Date);
            var date = message.Date ?? GetDate(message.WeekDay, message.WeekNumber);
            var year = message.Date?.Year ?? DateTime.UtcNow.Year;

            var dayAssign = DayAssignDomain.Create(
                message.Id, message.JobId, message.JobAssignId, message.DepartmentId, message.GroupId, message.UserIdList, date, time, message.EstimatedMinutes,
                message.WeekNumber, message.Address, message.TeamLeadId, message.WeekDay, message.DayPerWeekId, message.IsAssignedToAllUsers, message.Comment, message.ResidentName,
                message.ResidentPhone, message.Type, message.ExpiredDayAssignId, message.ExpiredWeekNumber, year, message.IsUrgent);

            await repository.Save(dayAssign, message.CreatorId);
        }

        public async Task Handle(ChangeDayAssignDateCommand message)
        {
            int defaultTaskStartUtcHour = appSettingHelper.GetAppSetting<int>(Constants.AppSetting.DefaultTaskStartUtcTime);
            var dayAssign = await repository.Get(message.Id);
            var time = dayAssign.Time ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, defaultTaskStartUtcHour, 0, 0, DateTimeKind.Utc); 
            var date = message.Date.HasValue 
                ? (DateTime?)new DateTime(message.Date.Value.Year, message.Date.Value.Month, message.Date.Value.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc)
                : null;

            dayAssign.ChangeDate(date, message.WeekDay, date?.Year ?? dayAssign.Year);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeDayAssignEstimatedMinutesCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeEstimatedMinutes(message.EstimatedMinutes);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeDayAssignMembersComand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.AssignUsers(message.GroupId, message.UserIdList, message.TeamLeadId, message.IsAssignedToAllUsers);
            await repository.Save(dayAssign);
        }

        public async Task Handle(RemoveDayAssignMembersCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.RemoveDayAssignMembers(message.UserIdList);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeJobIdAndJobAssignIdInDayAssignCommand message)
        {
            DayAssign dayAssign = await dayAssignProvider.GetByJobIdAndDepartmentId(message.Id, message.DepartmentId);
            if (dayAssign != null)
            {
                var dayAssignDomain = await repository.Get(dayAssign.Id.ToString());              
                dayAssignDomain.ChangeDayAssignJobAssignId(message.JobAssignId);
                await repository.Save(dayAssignDomain);
            }
        }

        public async Task Handle(ChangeDayAssignStatusCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeDayAssignStatus(message.Status);
            await repository.Save(dayAssign, message.CreatorId);
        }

        public async Task Handle(ChangeTenantTaskTypeCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeTenantTaskType(message.Type);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeTenantTaskUrgencyCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeTenantTaskUrgency(message.IsUrgent);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeOperationalTaskDateCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            var time = dayAssign.Time == default(DateTime) ? dayAssign.Date : dayAssign.Time;
            time = time ?? DateTime.UtcNow.Date;
            var date = new DateTime(message.Date.Year, message.Date.Month, message.Date.Day, time.Value.Hour, time.Value.Minute, time.Value.Second, DateTimeKind.Utc);
            dayAssign.ChangeDate(date, message.WeekDay, date.Year);
            dayAssign.ChangeWeekNumber(date.GetWeekNumber());
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeOperationalTaskTimeCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            var date = dayAssign.Date.Value;
            var time = new DateTime(date.Year, date.Month, date.Day, message.Time.Hour, message.Time.Minute, message.Time.Second, DateTimeKind.Utc);
            dayAssign.ChangeOperationalTaskTime(time);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeTenantTaskResidentNameCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeTenantTaskResidentName(message.ResidentName);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeTenantTaskResidentPhoneCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeTenantTaskResidentPhone(message.ResidentPhone);
            await repository.Save(dayAssign);
        }

        public async Task Handle(ChangeTenantTaskCommentCommand message)
        {
            var dayAssign = await repository.Get(message.Id);
            dayAssign.ChangeTenantTaskComment(message.Comment);
            await repository.Save(dayAssign);
        }

        private DateTime? GetTime(DateTime? date)
        {
            if (date.HasValue)
            {
                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, date.Value.Hour, date.Value.Minute, date.Value.Second, DateTimeKind.Utc);
            }

            return null;
        }

        private DateTime? GetDate(int? weekDay, int? weekNumber)
        {
            if (weekDay.HasValue && weekNumber.HasValue)
            {
                var year = DateTime.UtcNow.Year;
                return CalendarHelper.GetDateByWeekAndDayNumber(year, weekNumber.Value, weekDay.Value);
            }

            return null;
        }
    }
}