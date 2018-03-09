using System;
using System.Linq;
using System.Threading.Tasks;
using GroupsContract.Commands;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;

namespace Groups.Handlers
{
    public class GroupCommandHandler :
                IHandler<MembersAssign>,
                IHandler<MemberUnassign>,
                IHandler<CreateGroup>,
                IHandler<UpdateGroup>,
                IHandler<DeleteGroup>
    {
        private readonly IAggregateRootRepository<GroupSource> repository;

        public GroupCommandHandler(IAggregateRootRepository<GroupSource> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(MembersAssign message)
        {
            var group = await repository.Get(message.Id);
            UnnasignMembers(message, group);
            AssignMembers(message, group);

            await repository.Save(group);
        }

        public async Task Handle(MemberUnassign message)
        {
            var group = await repository.Get(message.Id);
            if (group.MemberIds.Contains(message.MemberId))
            {
                group.UnassignMember(message.MemberId);
            }

            await repository.Save(group);
        }

        public async Task Handle(CreateGroup message)
        {
            try
            {
                var item = await repository.Get(message.Id.ToString());
                if (item != null)
                {
                    throw new Exception($"Group with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }
            var group = GroupSource.Create(message.Id, message.Name, message.ManagementId, message.Deleted);
            await repository.Save(group);
        }

        public async Task Handle(UpdateGroup message)
        {
            var group = await repository.Get(message.Id);

            if (!string.Equals(group.Name, message.Name, StringComparison.InvariantCulture))
            {
                group.SetName(message.Name);
            }

            await repository.Save(group);
        }

        public async Task Handle(DeleteGroup message)
        {
            var group = await repository.Get(message.Id);
            group.Delete();
            await repository.Save(group);
        }

        private void UnnasignMembers(MembersAssign message, GroupSource group)
        {
            var memberIds = group.MemberIds.ToList();
            foreach (var memberId in memberIds)
            {
                if (!message.MemberIds.Contains(memberId))
                {
                    group.UnassignMember(memberId);
                }
            }
        }

        private void AssignMembers(MembersAssign message, GroupSource group)
        {
            foreach (var memberId in message.MemberIds)
            {
                if (!group.MemberIds.Contains(memberId))
                {
                    group.AssignMember(memberId);
                }
            }
        }
    }
}