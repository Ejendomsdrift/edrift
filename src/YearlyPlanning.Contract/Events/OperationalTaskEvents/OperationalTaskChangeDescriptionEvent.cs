using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskChangeDescriptionEvent : EventBase
    {
        public string Description { get; set; }
    }
}
