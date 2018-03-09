using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskChangeAddress : EventBase
    {
        public string Address { get; set; }
    }
}
