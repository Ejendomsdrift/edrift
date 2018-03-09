using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class WeekPlanGridModel: IWeekPlanGridModel
    {
        public IEnumerable<IWeekPlanJobModel> WeekJobs { get; set; }

        public IEnumerable<IWeekPlanJobModel> BackLogJobs { get; set; }

        public int WeekendJobCount { get; set; }

        public bool IsShowWeekend { get; set; }

    }
}
