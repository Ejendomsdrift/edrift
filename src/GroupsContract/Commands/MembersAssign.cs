using System;
using System.Collections.Generic;

namespace GroupsContract.Commands
{
    public class MembersAssign : GroupCommand
    {
        public IEnumerable<Guid> MemberIds { get; set; }

        public MembersAssign(string id, IEnumerable<Guid> memberIds) : base(id)
        {
            MemberIds = memberIds;
        }
    }
}