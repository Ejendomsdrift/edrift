using System;
using System.Collections.Generic;
using GroupsContract.Events;
using Infrastructure.EventSourcing.Implementation;

namespace Groups
{
    public class GroupSource : AggregateBase
    {
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }
        public List<Guid> MemberIds { get; set; } = new List<Guid>();

        public GroupSource()
        {
            RegisterTransition<GroupCreated>(Apply);
            RegisterTransition<GroupNameSet>(Apply);
            RegisterTransition<GroupDeleted>(Apply);
            RegisterTransition<MemberAssigned>(Apply);
            RegisterTransition<MemberUnassigned>(Apply);
        }

        public GroupSource(string id, string name, Guid managementId, bool deleted) : this()
        {
            Id = id;

            RaiseEvent(new GroupCreated
            {
                Name = name,
                ManagementId = managementId,
                Deleted = deleted
            });
        }

        private void Apply(GroupCreated e)
        {
            Id = e.SourceId;
            Name = e.Name;
            ManagementId = e.ManagementId;
            Deleted = e.Deleted;
        }

        public void SetName(string newName)
        {
            RaiseEvent(new GroupNameSet { Name = newName });
        }

        private void Apply(GroupNameSet e)
        {
            Name = e.Name;
        }

        public void Delete()
        {
            RaiseEvent(new GroupDeleted());
        }

        private void Apply(GroupDeleted e)
        {
            Deleted = true;
        }

        public void AssignMember(Guid memberId)
        {
            RaiseEvent(new MemberAssigned
            {
                MemberId = memberId,
            });
        }

        private void Apply(MemberAssigned e)
        {
            if (MemberIds == null) { MemberIds = new List<Guid>(); }
            if (!MemberIds.Contains(e.MemberId))
            {
                MemberIds.Add(e.MemberId);
            }
        }

        public void UnassignMember(Guid memberId)
        {
            RaiseEvent(new MemberUnassigned
            {
                MemberId = memberId,
            });
        }

        private void Apply(MemberUnassigned e)
        {
            if (MemberIds == null) { MemberIds = new List<Guid>(); }
            MemberIds.Remove(e.MemberId);
        }

        public static GroupSource Create(string id, string name, Guid managementId, bool deleted)
        {
            return new GroupSource(id, name, managementId, deleted);
        }
    }
}