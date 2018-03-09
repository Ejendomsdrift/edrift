using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class CopyCommonJobAssignInfoEvent: EventBase
    {
        public int TillYear { get; set; }

        public IEnumerable<WeekModel> WeekList { get; set; }

        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public int RepeatsPerWeek { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

    }
}
