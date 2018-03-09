using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignDescriptionChangedEvent : EventBase
    {
        public string Description { get; set; }
    }
}