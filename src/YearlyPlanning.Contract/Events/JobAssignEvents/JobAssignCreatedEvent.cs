using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;
using System;
using MemberCore.Contract.Enums;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignCreatedEvent: EventBase
    {
        public bool IsEnabled { get; set; }
        public int RepeatsPerWeek { get; set; }
        public bool IsLocked { get; set; }
        public int TillYear { get; set; }
        public RoleType CreatedByRole { get; set; }
        public ChangedByRole ChangedByRole { get; set; }
        public IEnumerable<WeekModel> WeekList { get; set; } 
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }
        public List<UploadFileModel> UploadList { get; set; }
        public List<string> JobIdList { get; set; } = new List<string>();
        public bool IsGlobal { get; set; }
        public List<Guid> HousingDepartmentIdList { get; set; }
    }
}
