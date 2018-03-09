using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class TenantTaskChangeResidentNameEvent : EventBase
    {
        public string ResidentName { get; set; }
    }
}
