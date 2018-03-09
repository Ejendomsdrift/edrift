using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignWeekNumberChangedEvent : EventBase
    {
        public int WeekNumber { get; set; }
    }
}
