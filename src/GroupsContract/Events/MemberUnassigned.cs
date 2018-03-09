using System;
using Infrastructure.EventSourcing.Implementation;

namespace GroupsContract.Events
{
    public class MemberUnassigned : EventBase
    {
        public Guid MemberId { get; set; }
    }
}
