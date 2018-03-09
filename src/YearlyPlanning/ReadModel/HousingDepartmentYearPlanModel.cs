using System.Collections.Generic;

namespace YearlyPlanning.ReadModel
{
    public class HousingDepartmentYearPlanModel
    {
        public IEnumerable<YearPlanItemViewModel> YearPlanItems { get; set; }
        public IDictionary<string, List<YearPlanWeekData>> WeeksData { get; set; }
    }
}
