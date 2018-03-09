using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Models
{
    public class MemberAssignModel
    {
        public Guid DayAssignId { get; set; }

        public Guid? GroupId { get; set; }

        public bool IsAssignedToAllUsers { get; set; }

        public Guid? TeamLeadId { get; set; }

        public List<Guid> UserIdList { get; set; }

        public bool IsUnassignAll { get; set; }
    }
}
