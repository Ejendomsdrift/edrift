using Infrastructure.EventSourcing.Implementation;
using StatusCore.Contract.Enums;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignChangeStatus: EventBase
    {
        public JobStatus Status { get; set; }
    }
}
