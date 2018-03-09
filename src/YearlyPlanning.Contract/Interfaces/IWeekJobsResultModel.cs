using System.Collections.Generic;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IWeekJobsResultModel
    {
        IEnumerable<IWeekPlanListViewModel> Jobs { get; set; }
        bool IsAllowedPreviousWeeks { get; set; }
        int PreviousNotEmptyWeekNumber { get; set; }
    }
}
