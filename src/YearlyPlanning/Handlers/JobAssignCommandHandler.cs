using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Handlers
{
    public class JobAssignCommandHandler :
        IHandler<CreateJobAssignCommand>,
        IHandler<AssignJobCommand>,
        IHandler<UnassignJobCommand>,
        IHandler<ChangeJobAssignDescriptionCommand>,
        IHandler<ChangeJobAssignTillYearCommand>,
        IHandler<ChangeJobAssignWeeksCommand>,
        IHandler<ChangeJobAssignAllWeeksCommand>,
        IHandler<ChangeLockIntervalValueCommand>,
        IHandler<CreateOperationalTaskAssignCommand>,
        IHandler<CreateJobAssignFromJobAssignCommand>,
        IHandler<SaveDaysPerWeekCommand>,
        IHandler<ChangeJobAssignJobIdListCommand>,
        IHandler<ChangeJobAssignSheduleCommand>,
        IHandler<JobAssignCopyCommonInfoCommand>
    {
        private readonly IAggregateRootRepository<JobAssignDomain> repository;

        public JobAssignCommandHandler(IAggregateRootRepository<JobAssignDomain> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(CreateJobAssignCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id.ToString());
                if (item != null)
                {
                    throw new Exception($"JobAssign with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }

            var jobAssign = JobAssignDomain.Create(message.Id, new List<string> { message.JobId }, message.CreatedByRole, message.TillYear);
            await repository.Save(jobAssign);
        }

        public async Task Handle(CreateJobAssignFromJobAssignCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id);
                if (item != null)
                {
                    throw new Exception($"JobAssign task with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }

            if (message.RewriteChangedByWeeks)
            {
                CopyWeekListFromGlobalAssign(message.WeekList);
            }

            var jobAssign = JobAssignDomain.Create(message.Id, message.HousingDepartmentIdList, message.Description, message.TillYear, message.RepeatsPerWeek,
                message.CreatedByRole, message.ChangedByRole, message.WeekList, message.UploadList, message.DayPerWeekList, message.JobIdList, message.IsGlobal, message.IsLocked, message.JobResponsibleLIst);
            await repository.Save(jobAssign);
        }

        public async Task Handle(CreateOperationalTaskAssignCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id);
                if (item != null)
                {
                    throw new Exception($"Operational task with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }


            var jobAssign = JobAssignDomain.CreateAdHockAssign(message);
            await repository.Save(jobAssign);
        }

        public async Task Handle(AssignJobCommand message)
        {
            JobAssignDomain jobAssign = await repository.Get(message.Id);
            CreateOrUpdateJobAssign(jobAssign, message.HousingDepartmentId);
            await repository.Save(jobAssign);
        }

        public async Task Handle(UnassignJobCommand message)
        {
            foreach (var departmentId in message.HousingDepartmentIds)
            {
                var jobAssign = await repository.Get(message.Id);
                DeleteOrUpdateAssign(jobAssign, departmentId);
                await repository.Save(jobAssign);
            }
        }

        public async Task Handle(ChangeJobAssignDescriptionCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeDescription(message.Description);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeJobAssignTillYearCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeTillYear(message.TillYear, message.ChangedByRole, message.IsLocalIntervalChanged);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeJobAssignWeeksCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeWeeks(message.WeekList, message.ChangedByRole, message.IsLocalIntervalChanged);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeJobAssignAllWeeksCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeAllWeeks(message.WeekList, message.ChangedByRole);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeLockIntervalValueCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.LockInterval(message.IsLocked);
            await repository.Save(jobAssign);
        }

        public async Task Handle(SaveDaysPerWeekCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            if (message.ChangedByRole == ChangedByRole.None)
            {
                message.ChangedByRole = jobAssign.ChangedByRole;
            }

            jobAssign.SaveDaysPerWeek(message.DayPerWeekList, message.ChangedByRole);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeJobAssignSheduleCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeJobShedule(message.DayPerWeekList, message.RepeatsPerWeek, message.ChangedBy, message.IsLocalIntervalChanged);
            await repository.Save(jobAssign);
        }

        public async Task Handle(ChangeJobAssignJobIdListCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.ChangeJobIdList(message.JobIdList);
            await repository.Save(jobAssign);
        }

        private void CreateOrUpdateJobAssign(JobAssignDomain jobAssign, Guid departmentId)
        {
            if (jobAssign.IsGlobal)
            {
                jobAssign.AssignDepartment(departmentId);
            }
            else
            {
                jobAssign.ChangeIsEnabled(true);
            }
        }

        private void DeleteOrUpdateAssign(JobAssignDomain jobAssign, Guid departmentId)
        {
            if (jobAssign.IsGlobal)
            {
                jobAssign.RemoveDepartment(departmentId);
            }
            else
            {
                jobAssign.ChangeIsEnabled(false);
            }
        }

        private void CopyWeekListFromGlobalAssign(IEnumerable<WeekModel> weekList)
        {
            foreach (var week in weekList)
            {
                week.ChangedBy = WeekChangedBy.Administrator | WeekChangedBy.Coordinator;
            }
        }

        public async Task Handle(JobAssignCopyCommonInfoCommand message)
        {
            var jobAssign = await repository.Get(message.Id);
            jobAssign.CopyCommonJobAssignInfo(message.TillYear, message.WeekList, message.DayPerWeekList, message.RepeatsPerWeek, message.ChangedByRole);
            await repository.Save(jobAssign);
        }
    }
}
