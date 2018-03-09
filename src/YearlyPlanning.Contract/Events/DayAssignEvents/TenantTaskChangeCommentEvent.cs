using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class TenantTaskChangeCommentEvent : EventBase
    {
        public string Comment { get; set; }
    }
}
