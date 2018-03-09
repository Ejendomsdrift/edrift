using System.Collections.Generic;

namespace YearlyPlanning.ReadModel
{
    public class DepartmentYearPlanViewModel
    {
        public IEnumerable<YearPlanItemViewModel> YearPlanItems { get; set; }
    }
}