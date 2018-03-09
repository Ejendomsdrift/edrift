using System;

namespace GroupsContract.Commands
{
    public class MemberUnassign : GroupCommand
    {
        public Guid MemberId { get; set; }

        public MemberUnassign(string id, Guid memberId) : base(id)
        {
            MemberId = memberId;
        }
    }
}
