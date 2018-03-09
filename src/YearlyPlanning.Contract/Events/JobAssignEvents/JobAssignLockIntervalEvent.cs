using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignLockIntervalEvent : EventBase
    {
        public bool IsLocked { get; set; }
    }
}
