using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace Web.Models
{
    public class WeekPlanTaskRequestViewModel
    {
        public Guid? HousingDepartmentId { get; set; }
        public int Week { get; set; }
        public int? StartWeek { get; set; }
        public int Year { get; set; }
        public IEnumerable<Guid> MemberIds { get; set; }
        public WeekPlanListViewTabEnum ListViewCurrentTab { get; set; }
    }
}