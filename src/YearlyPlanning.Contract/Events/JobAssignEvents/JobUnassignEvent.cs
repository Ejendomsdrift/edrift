using Infrastructure.EventSourcing.Implementation;
using System;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobUnassignEvent : EventBase
    {
        public Guid DepartmentId { get; set; }
    }
}
