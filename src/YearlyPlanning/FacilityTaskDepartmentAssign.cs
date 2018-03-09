using System.Collections.Generic;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning
{
    public class FacilityTaskDepartmentAssign
    {
        private const int OneTime = 1;

        public string DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public bool IsAssign { get; set; }

        public bool IsIntervalChanged { get; set; }

        public bool IsLocked { get; set; }

        public string Description { get; set; }

        public int PerWeek { get; set; } = OneTime;

        public int TillYear { get; set; }

        public bool IsHidden { get; set; }

        public IEnumerable<int> DaysPerWeek { get; set; }

        public List<WeekModel> Weeks { get; set; } = new List<WeekModel>();

        public RoleType Creator { get; set; }

        public ChangedByRole ChangedByRole { get; set; }
    }
}