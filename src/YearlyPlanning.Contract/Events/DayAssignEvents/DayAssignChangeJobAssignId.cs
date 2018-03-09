using System;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignChangeJobAssignId : EventBase
    {
        public Guid JobAssignId { get; set; }
    }
}
