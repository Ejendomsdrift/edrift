using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignTillYearChangedEvent : EventBase
    {
        public int TillYear { get; set; }
        public ChangedByRole ChangedByRole { get; set; }
        public bool IsLocalIntervalChanged { get; set; }
    }
}