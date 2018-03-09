using System;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignDateChanged : EventBase
    {
        public DateTime? Date { get; set; }

        public int? WeekDay { get; set; }

        public int Year { get; set; }
    }
}