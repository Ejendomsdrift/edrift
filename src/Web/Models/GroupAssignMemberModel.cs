using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class GroupAssignMemberModel
    {
        public Guid GroupId { get; set; }

        public IEnumerable<Guid> MemberIds { get; set; }
    }
}