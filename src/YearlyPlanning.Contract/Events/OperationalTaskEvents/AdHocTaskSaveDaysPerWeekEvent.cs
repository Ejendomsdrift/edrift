using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class AdHocTaskSaveDaysPerWeekEvent : EventBase
    {
        public IEnumerable<int> DaysPerWeek { get; set; }
    }
}
