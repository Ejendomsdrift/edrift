using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskChangeTitleEvent : EventBase
    {
        public string Title { get; set; }
    }
}
