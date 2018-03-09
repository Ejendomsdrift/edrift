using System;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class OperationalTaskChangeTimeEvent : EventBase
    {
        public DateTime Time { get; set; }
    }
}
