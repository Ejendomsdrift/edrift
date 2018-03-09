using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.EventSourcing.Implementation;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Events.JobAssignEvents;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning
{
    public class JobAssignDomain : AggregateBase
    {
        public List<Guid> HousingDepartmentIdList { get; set; } = new List<Guid>();
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
        public int TillYear { get; set; }
        public int RepeatsPerWeek { get; set; }
        public bool IsLocked { get; set; }
        public RoleType CreatedByRole { get; set; }
        public ChangedByRole ChangedByRole { get; set; }
        public IEnumerable<WeekModel> WeekList { get; set; } = Enumerable.Empty<WeekModel>();
        public List<UploadFileModel> UploadList { get; set; } = new List<UploadFileModel>();
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; } = Enumerable.Empty<DayPerWeekModel>();
        public List<string> JobIdList { get; set; } = new List<string>();
        public bool IsGlobal { get; set; }
        public bool IsLocalIntervalChanged { get; set; }
        public List<Responsible> JobResponsibleList { get; set; } = new List<Responsible>();

        public JobAssignDomain()
        {
            RegisterTransition<JobAssignCreatedEvent>(Apply);
            RegisterTransition<JobAssignCreatedFromGlobalEvent>(Apply);
            RegisterTransition<JobAssignEvent>(Apply);
            RegisterTransition<JobUnassignEvent>(Apply);
            RegisterTransition<JobAssignDescriptionChangedEvent>(Apply);
            RegisterTransition<JobAssignTillYearChangedEvent>(Apply);
            RegisterTransition<JobAssignWeeksChangedEvent>(Apply);
            RegisterTransition<JobAssignAllWeeksChangedEvent>(Apply);
            RegisterTransition<JobAssignLockIntervalEvent>(Apply);
            RegisterTransition<AdHockJobAssignCreatedEvent>(Apply);
            RegisterTransition<JobAssignChangeIsEnabledEvent>(Apply);
            RegisterTransition<TenantJobAssignCreatedEvent>(Apply);
            RegisterTransition<JobAssignSaveDaysPerWeekEvent>(Apply);
            RegisterTransition<JobAssignJobIdListChangedEvent>(Apply);
            RegisterTransition<JobAssignSheduleChangedEvent>(Apply);
            RegisterTransition<CopyCommonJobAssignInfoEvent>(Apply);
        }

        public static JobAssignDomain Create(string id, List<string> jobIds, RoleType createdByRole, int tillYear)
        {
            return new JobAssignDomain(id, jobIds, createdByRole, tillYear);
        }

        public static JobAssignDomain Create(string id, List<Guid> housingDepartmentIdList, string description, int tillYear, int repeatsPerWeek, RoleType createdByRole,
            ChangedByRole changedByRole, IEnumerable<WeekModel> weekList, List<UploadFileModel> uploadList, IEnumerable<DayPerWeekModel> dayPerWeekList, List<string> jobIds, bool isGlobal, bool isLocked, List<Responsible> jobResponsibleList)
        {
            return new JobAssignDomain(id, housingDepartmentIdList, description, tillYear, repeatsPerWeek,
                createdByRole, changedByRole, weekList, uploadList, dayPerWeekList, jobIds, isGlobal, isLocked, jobResponsibleList);
        }

        public JobAssignDomain(string id, List<string> jobIds, RoleType createdByRole, int tillYear)
        {
            Id = id;

            RaiseEvent(new JobAssignCreatedEvent
            {
                IsEnabled = true,
                RepeatsPerWeek = 1,
                IsLocked = false,
                CreatedByRole = createdByRole,
                JobIdList = jobIds,
                IsGlobal = true,
                HousingDepartmentIdList = new List<Guid>(),
                WeekList = Enumerable.Empty<WeekModel>(),
                DayPerWeekList = Enumerable.Empty<DayPerWeekModel>(),
                UploadList = new List<UploadFileModel>(),
                ChangedByRole = ChangedByRole.None,
                TillYear = tillYear
            });
        }

        private void Apply(JobAssignCreatedEvent e)
        {
            Id = e.SourceId;
            IsEnabled = e.IsEnabled;
            RepeatsPerWeek = e.RepeatsPerWeek;
            IsLocked = e.IsLocked;
            CreatedByRole = e.CreatedByRole;
            JobIdList = e.JobIdList;
            IsGlobal = e.IsGlobal;
            HousingDepartmentIdList = e.HousingDepartmentIdList.ToList();
            UploadList = e.UploadList;
            TillYear = e.TillYear;
        }

        public JobAssignDomain(CreateOperationalTaskAssignCommand message)
        {
            Id = message.Id;

            RaiseEvent(new AdHockJobAssignCreatedEvent
            {
                DepartmentId = message.DepartmentId,
                TillYear = message.TillYear,
                Description = message.Description,
                RepeatsPerWeek = message.RepeatsPerWeek,
                WeekList = message.WeekList,
                JobIdList = message.JobIdList,
                CreatedByRole = message.CreatedByRole,
                IsEnabled = message.IsEnabled
            });
        }

        private void Apply(AdHockJobAssignCreatedEvent e)
        {
            Id = e.SourceId;
            IsGlobal = true;

            AddHousingDepartmentId(e.DepartmentId);
            TillYear = e.TillYear;
            Description = e.Description;
            RepeatsPerWeek = e.RepeatsPerWeek;
            WeekList = e.WeekList;
            JobIdList = e.JobIdList;
            CreatedByRole = e.CreatedByRole;
            IsEnabled = e.IsEnabled;
        }

        public JobAssignDomain(string id, List<Guid> housingDepartmentIdList, string description, int tillYear, int repeatsPerWeek, RoleType createdByRole,
            ChangedByRole changedByRole, IEnumerable<WeekModel> weekList, List<UploadFileModel> uploadList, IEnumerable<DayPerWeekModel> dayPerWeekList, List<string> jobIds, bool isGlobal, bool isLocked, List<Responsible> jorResponsibleList)
        {
            Id = id;

            RaiseEvent(new JobAssignCreatedFromGlobalEvent
            {
                HousingDepartmentIdList = housingDepartmentIdList,
                IsEnabled = true,
                Description = description,
                TillYear = tillYear,
                RepeatsPerWeek = repeatsPerWeek,
                IsLocked = isLocked,
                CreatedByRole = createdByRole,
                ChangedByRole = changedByRole,
                WeekList = weekList,
                UploadList = uploadList,
                DayPerWeekList = dayPerWeekList,
                JobIdList = jobIds,
                IsGlobal = isGlobal,
                JobResponsibleList = jorResponsibleList
            });
        }

        private void Apply(JobAssignCreatedFromGlobalEvent e)
        {
            Id = e.SourceId;
            HousingDepartmentIdList = e.HousingDepartmentIdList;
            IsEnabled = e.IsEnabled;
            Description = e.Description;
            TillYear = e.TillYear;
            RepeatsPerWeek = e.RepeatsPerWeek;
            IsLocked = e.IsLocked;
            CreatedByRole = e.CreatedByRole;
            ChangedByRole = e.ChangedByRole;
            WeekList = e.WeekList;
            UploadList = e.UploadList;
            DayPerWeekList = e.DayPerWeekList;
            JobIdList = e.JobIdList;
            IsGlobal = e.IsGlobal;
        }
        private void Apply(TenantJobAssignCreatedEvent e)
        {
            Id = e.SourceId;
            IsGlobal = true;

            AddHousingDepartmentId(e.DepartmentId);
            Description = e.Description;
            RepeatsPerWeek = e.RepeatsPerWeek;
            WeekList = e.WeekList;
            JobIdList = e.JobIdList;
            CreatedByRole = e.CreatedByRole;
            IsEnabled = e.IsEnabled;
            TillYear = e.TillYear;
        }

        public void AssignDepartment(Guid departmentId)
        {
            RaiseEvent(new JobAssignEvent
            {
                DepartmentId = departmentId
            });
        }

        private void Apply(JobAssignEvent e)
        {
            HousingDepartmentIdList.Add(e.DepartmentId);
        }

        public void RemoveDepartment(Guid departmentId)
        {
            RaiseEvent(new JobUnassignEvent
            {
                DepartmentId = departmentId
            });
        }

        private void Apply(JobUnassignEvent e)
        {
            HousingDepartmentIdList.Remove(e.DepartmentId);
        }

        public void ChangeIsEnabled(bool isEnabled)
        {
            RaiseEvent(new JobAssignChangeIsEnabledEvent
            {
                IsEnabled = isEnabled
            });
        }

        private void Apply(JobAssignChangeIsEnabledEvent e)
        {
            IsEnabled = e.IsEnabled;
        }

        public void ChangeDescription(string description)
        {
            RaiseEvent(new JobAssignDescriptionChangedEvent { Description = description });
        }

        private void Apply(JobAssignDescriptionChangedEvent e)
        {
            Description = e.Description;
        }

        public void ChangeTillYear(int tillYear, ChangedByRole changedByRole, bool isLocalIntervalChanged)
        {
            RaiseEvent(new JobAssignTillYearChangedEvent { TillYear = tillYear, ChangedByRole = changedByRole, IsLocalIntervalChanged = isLocalIntervalChanged});
        }

        private void Apply(JobAssignTillYearChangedEvent e)
        {
            TillYear = e.TillYear;
            ChangedByRole = e.ChangedByRole;
            IsLocalIntervalChanged = e.IsLocalIntervalChanged;
        }

        public void ChangeWeeks(IEnumerable<WeekModel> weeks, ChangedByRole changedByRole, bool isLocalIntervalChanged)
        {
            RaiseEvent(new JobAssignWeeksChangedEvent { Weeks = weeks, ChangedByRole = changedByRole, IsLocalIntervalChanged = isLocalIntervalChanged});
        }

        private void Apply(JobAssignWeeksChangedEvent e)
        {
            WeekList = e.Weeks.ToList();
            ChangedByRole = e.ChangedByRole;
            IsLocalIntervalChanged = e.IsLocalIntervalChanged;
        }

        public void ChangeAllWeeks(IEnumerable<WeekModel> weeks, ChangedByRole changedByRole)
        {
            RaiseEvent(new JobAssignAllWeeksChangedEvent { Weeks = weeks, ChangedByRole = changedByRole });
        }

        private void Apply(JobAssignAllWeeksChangedEvent e)
        {
            WeekList = e.Weeks.ToList();
            ChangedByRole = e.ChangedByRole;
        }

        public void LockInterval(bool isLocked)
        {
            RaiseEvent(new JobAssignLockIntervalEvent { IsLocked = isLocked });
        }

        private void Apply(JobAssignLockIntervalEvent e)
        {
            IsLocked = e.IsLocked;
        }

        public void SaveDaysPerWeek(IEnumerable<DayPerWeekModel> dayPerWeekList, ChangedByRole changedByRole)
        {
            RaiseEvent(new JobAssignSaveDaysPerWeekEvent { DayPerWeekList = dayPerWeekList, ChangedByRole = changedByRole });
        }

        private void Apply(JobAssignSaveDaysPerWeekEvent e)
        {
            DayPerWeekList = e.DayPerWeekList;
            ChangedByRole = e.ChangedByRole;
        }

        public void ChangeJobShedule(IEnumerable<DayPerWeekModel> dayPerWeekList, int repeatsPerWeek, ChangedByRole changedBy, bool isLocalIntervalChanged)
        {
            RaiseEvent(new JobAssignSheduleChangedEvent
            {
                DayPerWeekList = dayPerWeekList,
                RepeatsPerWeek = repeatsPerWeek,
                ChangedBy = changedBy,
                IsLocalIntervalChanged = isLocalIntervalChanged
            });
        }

        private void Apply(JobAssignSheduleChangedEvent e)
        {
            DayPerWeekList = e.DayPerWeekList;
            RepeatsPerWeek = e.RepeatsPerWeek;
            ChangedByRole = e.ChangedBy;
            IsLocalIntervalChanged = IsLocalIntervalChanged;
        }
        
        public void ChangeJobIdList(List<string> jobIds)
        {
            RaiseEvent(new JobAssignJobIdListChangedEvent { JobIdList = jobIds });
        }

        private void Apply(JobAssignJobIdListChangedEvent e)
        {
            JobIdList = e.JobIdList;
        }

        public void CopyCommonJobAssignInfo(int tillYear, IEnumerable<WeekModel> weekList,
            IEnumerable<DayPerWeekModel> dayPerWeekList, int repeatsPerWeek, ChangedByRole changedByRole)
        {
            RaiseEvent(new CopyCommonJobAssignInfoEvent
            {
                ChangedByRole = changedByRole,
                DayPerWeekList = dayPerWeekList,
                TillYear = tillYear,
                RepeatsPerWeek = repeatsPerWeek,
                WeekList = weekList
            });
        }

        private void Apply(CopyCommonJobAssignInfoEvent e)
        {
            ChangedByRole = e.ChangedByRole;
            DayPerWeekList = e.DayPerWeekList;
            TillYear = e.TillYear;
            RepeatsPerWeek = e.RepeatsPerWeek;
            WeekList = e.WeekList;
        }

        public static JobAssignDomain CreateAdHockAssign(CreateOperationalTaskAssignCommand message)
        {
            return new JobAssignDomain(message);
        }

        private void AddHousingDepartmentId(Guid departmentId)
        {
            if (HousingDepartmentIdList == null) { HousingDepartmentIdList = new List<Guid>(); }
            if (!HousingDepartmentIdList.Contains(departmentId))
            {
                HousingDepartmentIdList.Add(departmentId);
            }
        }
    }
}
