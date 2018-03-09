using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignEstimatedMinutesChanged : EventBase
    {
        public int? EstimatedMinutes { get; set; }
    }
}