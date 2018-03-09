using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobEvents
{
    public class JobVisibilityChanged: EventBase
    {
        public bool IsHidden { get; set; }
    }
}
