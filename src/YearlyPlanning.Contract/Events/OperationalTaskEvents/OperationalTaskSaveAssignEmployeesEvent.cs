using System;
using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskSaveAssignEmployeesEvent : EventBase
    {
        public Guid GroupId { get; set; }       

        public IEnumerable<Guid> AssignedEmployees { get; set; }
    }
}
