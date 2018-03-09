using System;
using Infrastructure.EventSourcing.Implementation;

namespace GroupsContract.Events
{
    public class MemberAssigned : EventBase
    {
        public Guid MemberId { get; set; }
    }
}
