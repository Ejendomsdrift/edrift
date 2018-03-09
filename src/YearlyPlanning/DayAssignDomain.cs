using System;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Events.DayAssignEvents;
using System.Collections.Generic;
using System.Linq;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Events.OperationalTaskEvents;

namespace YearlyPlanning
{
    public class DayAssignDomain : AggregateBase
    {
        public string JobId { get; set; }
        public Guid JobAssignId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? GroupId { get; set; }
        public List<Guid> UserIdList { get; set; } = new List<Guid>();
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public JobStatus StatusId { get; set; }
        public int? EstimatedMinutes { get; set; }
        public int WeekNumber { get; set; }
        public Guid? TeamLeadId { get; set; }
        public int? WeekDay { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public Guid DayPerWeekId { get; set; }
        public TenantTaskTypeEnum Type { get; set; }
        public string ResidentName { get; set; }
        public string ResidentPhone { get; set; }
        public string Comment { get; set; }
        public Guid? ExpiredDayAssignId { get; set; }
        public int? ExpiredWeekNumber { get; set; }
        public int Year { get; set; }
        public bool? IsUrgent { get; set; }

        public DayAssignDomain()
        {
            RegisterTransition<DayAssignCreated>(Apply);
            RegisterTransition<DayAssignDateChanged>(Apply);
            RegisterTransition<DayAssignEstimatedMinutesChanged>(Apply);
            RegisterTransition<DayAssignMembersGroupAssigned>(Apply);
            RegisterTransition<RemoveDayAssignMembersEvent>(Apply);
            RegisterTransition<DayAssignChangeJobAssignId>(Apply);
            RegisterTransition<OperationalTaskChangeTimeEvent>(Apply);
            RegisterTransition<TenantTaskChangeResidentNameEvent>(Apply);
            RegisterTransition<TenantTaskChangeResidentPhoneEvent>(Apply);
            RegisterTransition<TenantTaskChangeCommentEvent>(Apply);
            RegisterTransition<DayAssignChangeStatus>(Apply);
            RegisterTransition<TenantTaskChangeTypeEvent>(Apply);
            RegisterTransition<DayAssignWeekNumberChangedEvent>(Apply);
            RegisterTransition<TenantTaskChangeUrgencyEvent>(Apply);
        }

        public static DayAssignDomain Create(string id, string jobId, Guid jobAssignId, Guid departmentId, Guid? groupId, List<Guid> userIds, DateTime? date, DateTime? time,
            int? estimate, int weekNumber, string address, Guid? teamLeadId, int? weekDay, Guid dayPerWeekId, bool isAssignedToAllUsers, string comment, string residentName,
            string residentPhone, TenantTaskTypeEnum? type, Guid? expiredDayAssignId, int? expiredWeekNumber,int year, bool? isUrgent)
        {
            return new DayAssignDomain(id, jobId, jobAssignId, departmentId, groupId, userIds, date, time, estimate, weekNumber, address, teamLeadId, weekDay, dayPerWeekId,
                isAssignedToAllUsers, comment, residentName, residentPhone, type, expiredDayAssignId, expiredWeekNumber, year, isUrgent);
        }

        public DayAssignDomain(string id, string jobId, Guid jobAssignId, Guid departmentId, Guid? groupId, List<Guid> userIds, DateTime? date, DateTime? time, int? estimate,
            int weekNumber, string address, Guid? temLeadId, int? weekDay, Guid dayPerWeekId, bool isAssignedToAllUsers, string comment, string residentName, string residentPhone,
            TenantTaskTypeEnum? type, Guid? expiredDayAssignId, int? expiredWeekNumber, int year, bool? isUrgent) : this()
        {
            Id = id;

            RaiseEvent(new DayAssignCreated
            {
                JobId = jobId,
                JobAssignId = jobAssignId,
                HousingDepartmentId = departmentId,
                GroupId = groupId,
                UserIdList = userIds,
                Date = date,
                Time = time,
                StatusId = JobStatus.Pending,
                EstimatedMinutes = estimate,
                WeekNumber = weekNumber,
                Address = address,
                TeamLeadId = temLeadId,
                WeekDay = weekDay,
                IsAssignedToAllUsers = isAssignedToAllUsers,
                DayPerWeekId = dayPerWeekId,
                Comment = comment,
                ResidentName = residentName,
                ResidentPhone = residentPhone,
                Type = type,
                ExpiredDayAssignId = expiredDayAssignId,
                ExpiredWeekNumber = expiredWeekNumber,
                Year = year,
                IsUrgent = isUrgent
            });
        }

        private void Apply(DayAssignCreated e)
        {
            Id = e.SourceId;
            JobId = e.JobId;
            JobAssignId = e.JobAssignId;
            DepartmentId = e.HousingDepartmentId;
            GroupId = e.GroupId;
            UserIdList = e.UserIdList;
            Date = e.Date;
            Time = e.Time;
            StatusId = e.StatusId;
            EstimatedMinutes = e.EstimatedMinutes;
            WeekNumber = e.WeekNumber;
            TeamLeadId = e.TeamLeadId;
            WeekDay = e.WeekDay;
            IsAssignedToAllUsers = e.IsAssignedToAllUsers;
            DayPerWeekId = e.DayPerWeekId;
            ExpiredDayAssignId = e.ExpiredDayAssignId;
            ExpiredWeekNumber = e.ExpiredWeekNumber;
            Year = e.Year;
            IsUrgent = e.IsUrgent;
        }

        public void ChangeDate(DateTime? date, int? weekDay, int year)
        {
            RaiseEvent(new DayAssignDateChanged { Date = date, WeekDay = weekDay, Year = year});
        }

        private void Apply(DayAssignDateChanged e)
        {
            Date = e.Date;
            WeekDay = e.WeekDay;
            Year = e.Year;
        }

        public void ChangeWeekNumber(int weekNumber)
        {
            RaiseEvent(new DayAssignWeekNumberChangedEvent { WeekNumber = weekNumber });
        }

        private void Apply(DayAssignWeekNumberChangedEvent e)
        {
            WeekNumber = e.WeekNumber;
        }

        public void ChangeEstimatedMinutes(int? estimate)
        {
            RaiseEvent(new DayAssignEstimatedMinutesChanged { EstimatedMinutes = estimate });
        }

        private void Apply(DayAssignEstimatedMinutesChanged e)
        {
            EstimatedMinutes = e.EstimatedMinutes;
        }

        public void AssignUsers(Guid? groupId, List<Guid> userIds, Guid? teamLeadId, bool isAssignedToAllUsers)
        {
            RaiseEvent(new DayAssignMembersGroupAssigned { GroupId = groupId, UserIdList = userIds, TeamLeadId = teamLeadId, IsAssignedToAllUsers = isAssignedToAllUsers });
        }

        private void Apply(DayAssignMembersGroupAssigned e)
        {
            UserIdList = UserIdList ?? new List<Guid>();
            GroupId = e.GroupId;
            TeamLeadId = e.TeamLeadId;
            IsAssignedToAllUsers = e.IsAssignedToAllUsers;
            UserIdList.AddRange(e.UserIdList);
            UserIdList = UserIdList.Distinct().ToList();
        }

        public void RemoveDayAssignMembers(List<Guid> userIdList)
        {
            RaiseEvent(new RemoveDayAssignMembersEvent { UserIdList = userIdList });
        }

        private void Apply(RemoveDayAssignMembersEvent e)
        {
            UserIdList = e.UserIdList;
        }

        public void ChangeDayAssignJobAssignId(Guid jobAssignId)
        {
            RaiseEvent(new DayAssignChangeJobAssignId { JobAssignId = jobAssignId });
        }

        private void Apply(DayAssignChangeJobAssignId e)
        {
            JobAssignId = e.JobAssignId;
        }

        public void ChangeDayAssignStatus(JobStatus status)
        {
            RaiseEvent(new DayAssignChangeStatus { Status = status });
        }

        private void Apply(DayAssignChangeStatus e)
        {
            StatusId = e.Status;
        }

        public void ChangeTenantTaskType(TenantTaskTypeEnum type)
        {
            RaiseEvent(new TenantTaskChangeTypeEvent { Type = type });
        }

        private void Apply(TenantTaskChangeTypeEvent e)
        {
            Type = e.Type;
        }

        public void ChangeTenantTaskUrgency(bool isUrgent)
        {
            RaiseEvent(new TenantTaskChangeUrgencyEvent { IsUrgent = isUrgent });
        }

        private void Apply(TenantTaskChangeUrgencyEvent e)
        {
            IsUrgent = e.IsUrgent;
        }

        public void ChangeOperationalTaskTime(DateTime time)
        {
            RaiseEvent(new OperationalTaskChangeTimeEvent { Time = time });
        }

        private void Apply(OperationalTaskChangeTimeEvent e)
        {
            Time = e.Time;
        }

        public void ChangeTenantTaskResidentName(string residentName)
        {
            RaiseEvent(new TenantTaskChangeResidentNameEvent { ResidentName = residentName });
        }

        private void Apply(TenantTaskChangeResidentNameEvent e)
        {
            ResidentName = e.ResidentName;
        }

        public void ChangeTenantTaskResidentPhone(string residentPhone)
        {
            RaiseEvent(new TenantTaskChangeResidentPhoneEvent { ResidentPhone = residentPhone });
        }

        private void Apply(TenantTaskChangeResidentPhoneEvent e)
        {
            ResidentPhone = e.ResidentPhone;
        }

        public void ChangeTenantTaskComment(string comment)
        {
            RaiseEvent(new TenantTaskChangeCommentEvent { Comment = comment });
        }

        private void Apply(TenantTaskChangeCommentEvent e)
        {
            Comment = e.Comment;
        }
    }
}