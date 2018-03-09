using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;
using System;
using MemberCore.Contract.Enums;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignCreatedFromGlobalEvent : EventBase
    {
        public List<Guid> HousingDepartmentIdList { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
        public int TillYear { get; set; }
        public int RepeatsPerWeek { get; set; }
        public bool IsLocked { get; set; }
        public RoleType CreatedByRole { get; set; }
        public ChangedByRole ChangedByRole { get; set; }
        public IEnumerable<WeekModel> WeekList { get; set; }
        public List<UploadFileModel> UploadList { get; set; }
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }
        public List<string> JobIdList { get; set; } = new List<string>();
        public bool IsGlobal { get; set; }
        public List<Responsible> JobResponsibleList { get; set; } = new List<Responsible>();
    }
}