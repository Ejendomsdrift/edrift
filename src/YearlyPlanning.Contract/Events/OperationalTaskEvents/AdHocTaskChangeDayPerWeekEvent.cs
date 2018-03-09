using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class AdHocTaskChangeDayPerWeekEvent : EventBase
    {
        public IEnumerable<int> Days { get; set; }
    }
}
