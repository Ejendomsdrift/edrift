using Infrastructure.EventSourcing.Implementation;
using System;

namespace GroupsContract.Events
{
    public class GroupCreated : EventBase
    {
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }
    }
}