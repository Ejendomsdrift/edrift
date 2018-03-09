using System;
using System.Collections.Generic;
using System.Linq;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public class FacilityTaskDepartmentAssignViewModel
    {
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public Guid ManagementDepartmentId { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsIntervalChanged { get; set; }

        public string Description { get; set; }

        public int TillYear { get; set; }

        public int PerWeek { get; set; }

        public bool IsLocked { get; set; }

        public RoleType Creator { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public IEnumerable<WeekModel> Weeks { get; set; } = Enumerable.Empty<WeekModel>();

        public List<UploadFileModel> Uploads { get; set; } = new List<UploadFileModel>();

        public IEnumerable<int> DaysPerWeek { get; set; } = Enumerable.Empty<int>();
    }
}
