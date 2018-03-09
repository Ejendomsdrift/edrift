using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace Web.Models.Task
{
    public class JobAssignViewModel
    {
        public Guid Id { get; set; }

        public List<Guid> HousingDepartmentIdList { get; set; }

        public string Description { get; set; }

        public int TillYear { get; set; }

        public int RepeatsPerWeek { get; set; }

        public bool IsLocked { get; set; }

        public RoleType CreatedByRole { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public IEnumerable<WeekModel> WeekList { get; set; }

        public List<UploadFileViewModel> UploadList { get; set; }

        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public List<string> JobIdList { get; set; }

        public bool IsGlobal { get; set; }

        public bool IsEnabled { get; set; }
    }
}