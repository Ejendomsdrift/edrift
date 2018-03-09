using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignChangeIsEnabledEvent : EventBase
    {
        public bool IsEnabled { get; set; }
    }
}
