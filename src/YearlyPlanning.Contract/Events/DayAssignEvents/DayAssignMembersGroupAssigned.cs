using System;
using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.DayAssignEvents
{
    public class DayAssignMembersGroupAssigned : EventBase
    {
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Guid> UserIdList { get; set; }
        public Guid? TeamLeadId { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
    }
}
