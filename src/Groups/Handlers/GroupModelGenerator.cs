using System;
using System.Threading.Tasks;
using Groups.Models;
using GroupsContract.Events;
using Infrastructure.Messaging;
using MongoRepository.Contract.Interfaces;

namespace Groups.Handlers
{
    public class GroupModelGenerator :
        IHandler<GroupNameSet>,
        IHandler<GroupCreated>,
        IHandler<GroupDeleted>,
        IHandler<MemberAssigned>,
        IHandler<MemberUnassigned>
    {
        private readonly IRepository<Group> groupRepository;

        public GroupModelGenerator(IRepository<Group> groupRepository)
        {
            this.groupRepository = groupRepository;
        }

        public async Task Handle(GroupCreated message)
        {
            var group = new Group
            {
                Id = Guid.Parse(message.SourceId),
                ManagementId = message.ManagementId,
                Name = message.Name,
                Deleted = message.Deleted
            };

            groupRepository.Save(group);
        }

        public async Task Handle(GroupNameSet message)
        {
            groupRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), g => g.Name, message.Name);
        }

        public async Task Handle(GroupDeleted message)
        {
            groupRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), g => g.Deleted, true);
        }

        public async Task Handle(MemberAssigned message)
        {
            var group = groupRepository.FindOne(g => g.Id == Guid.Parse(message.SourceId));
            var memberIds = group.MemberIds;
            memberIds.Add(message.MemberId);

            groupRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), g => g.MemberIds, memberIds);
        }

        public async Task Handle(MemberUnassigned message)
        {
            var group = groupRepository.FindOne(g => g.Id == Guid.Parse(message.SourceId));
            var memberIds = group.MemberIds;
            memberIds.Remove(message.MemberId);

            groupRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), g => g.MemberIds, memberIds);
        }
    }
}