using System.Collections.Generic;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IWeekPlanGridModel
    {
        IEnumerable<IWeekPlanJobModel> WeekJobs { get; set; }

        IEnumerable<IWeekPlanJobModel> BackLogJobs { get; set; }

        int WeekendJobCount { get; set; }

        bool IsShowWeekend { get; set; }
    }
}
