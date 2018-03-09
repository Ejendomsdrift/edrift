using System;
using System.Collections.Generic;

namespace Web.Models.Group
{
    public class GroupMemberViewModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public bool IsCanBeDeleted { get; set; }

        public IEnumerable<MemberModelWithTimeView> Members { get; set; }
    }
}