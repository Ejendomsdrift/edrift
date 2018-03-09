using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class WeekJobsResultModel : IWeekJobsResultModel
    {
        public IEnumerable<IWeekPlanListViewModel> Jobs { get; set; } = Enumerable.Empty<IWeekPlanListViewModel>();
        public bool IsAllowedPreviousWeeks { get; set; }
        public int PreviousNotEmptyWeekNumber { get; set; }
    }
}
