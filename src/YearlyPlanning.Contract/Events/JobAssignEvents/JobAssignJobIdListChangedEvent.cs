using Infrastructure.EventSourcing.Implementation;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignJobIdListChangedEvent : EventBase
    {
        public List<string> JobIdList { get; set; }
    }
}
