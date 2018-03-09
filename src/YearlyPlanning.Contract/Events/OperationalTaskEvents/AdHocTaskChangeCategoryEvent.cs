using System;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class AdHocTaskChangeCategoryEvent : EventBase
    {
        public Guid CategoryId { get; set; }
    }
}
