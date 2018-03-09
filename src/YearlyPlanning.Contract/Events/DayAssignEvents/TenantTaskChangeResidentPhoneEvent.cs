using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class TenantTaskChangeResidentPhoneEvent : EventBase
    {
        public string ResidentPhone { get; set; }
    }
}
