using System;
using System.Collections.Generic;
using System.Linq;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Models
{
    public class JobAssign
    {
        public Guid Id { get; set; }

        public List<Guid> HousingDepartmentIdList { get; set; } = new List<Guid>();

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

        public bool IsEnabled { get; set; }

        public bool IsLocalIntervalChanged { get; set; }

        public List<Responsible> JobResponsibleList { get; set; } = new List<Responsible>();
    }
}
