using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignSheduleChangedEvent : EventBase
    {
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }
        public int RepeatsPerWeek { get; set; }
        public ChangedByRole ChangedBy { get; set; }
        public bool IsLocalIntervalChanged { get; set; }
    }
}
