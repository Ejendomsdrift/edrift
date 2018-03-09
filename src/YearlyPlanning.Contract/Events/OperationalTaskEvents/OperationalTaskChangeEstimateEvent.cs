using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskChangeEstimateEvent : EventBase
    {
        public decimal Estimate { get; set; }
    }
}
