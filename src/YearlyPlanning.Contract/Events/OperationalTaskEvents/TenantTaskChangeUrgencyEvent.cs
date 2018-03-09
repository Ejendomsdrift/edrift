using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class TenantTaskChangeUrgencyEvent : EventBase
    {
        public bool IsUrgent { get; set; }
    }
}
