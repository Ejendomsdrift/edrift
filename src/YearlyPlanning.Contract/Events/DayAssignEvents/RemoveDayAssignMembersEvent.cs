using System;
using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class RemoveDayAssignMembersEvent: EventBase
    {       
        public List<Guid> UserIdList { get; set; }
    }
}
