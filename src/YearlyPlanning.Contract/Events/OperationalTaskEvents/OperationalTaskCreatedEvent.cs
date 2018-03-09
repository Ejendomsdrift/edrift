using Infrastructure.EventSourcing.Implementation;
using System;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskCreatedEvent : EventBase
    {
        public int Year { get; set; }

        public int Week { get; set; }

        public Guid CategoryId { get; set; }

        public Guid DepartmentId { get; set; }
        
        public string Title { get; set; }
    }
}
