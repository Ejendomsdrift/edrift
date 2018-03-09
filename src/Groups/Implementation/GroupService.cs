using System;
using System.Collections.Generic;
using System.Linq;
using Groups.Models;
using GroupsContract.Interfaces;
using GroupsContract.Models;
using Infrastructure.Extensions;
using MongoRepository.Contract.Interfaces;

namespace Groups.Implementation
{
    public class GroupService : IGroupService
    {
        private readonly IRepository<Group> groupRepository;

        public GroupService(IRepository<Group> groupRepository)
        {
            this.groupRepository = groupRepository;
        }

        public IGroupModel Get(Guid id)
        {
            var group = groupRepository.FindOne(g => g.Id == id);
            var result = group.Map<GroupModel>();
            return result;
        }

        public IEnumerable<IGroupModel> GetAllByUserId(Guid userId)
        {
            var groups = groupRepository.Find(g => g.MemberIds.Contains(userId));
            var result = groups.Select(g => g.Map<GroupModel>());
            return result;
        }

        public IEnumerable<IGroupModel> GetAll()
        {
            var groups = groupRepository.Query.Where(i => !i.Deleted).ToList();
            var result = groups.Select(g => g.Map<GroupModel>());
            return result;
        }

        public IEnumerable<IGroupModel> GetAllByUserIds(IEnumerable<Guid> userIds)
        {
            var groups = groupRepository.Find(g => g.MemberIds.Any(i => userIds.Contains(i)));
            var result = groups.Select(g => g.Map<GroupModel>());
            return result;
        }

        public IEnumerable<IGroupModel> GetByIds(IEnumerable<Guid> ids)
        {
            if (!ids.HasValue())
            {
                return Enumerable.Empty<IGroupModel>();
            }

            var groups = groupRepository.Find(g => ids.Contains(g.Id) && !g.Deleted);
            var result = groups.Map<IEnumerable<GroupModel>>();
            return result;
        }

        public IEnumerable<IGroupModel> GetByManagementId(Guid managementId)
        {
            var groups = groupRepository.Find(g => g.ManagementId == managementId && !g.Deleted);
            var result = groups.Select(g => g.Map<GroupModel>());
            return result;
        }

        public bool IsUniqueName(Guid managementId, string groupName)
        {
            var groups = groupRepository.Find(g => g.ManagementId == managementId && !g.Deleted && g.Name.ToLowerInvariant() == groupName);
            return !groups.Any();
        }
    }
}
