using System;
using System.Collections.Generic;
using GroupsContract.Models;

namespace GroupsContract.Interfaces
{
    public interface IGroupService
    {
        IGroupModel Get(Guid id);
        IEnumerable<IGroupModel> GetAll();
        IEnumerable<IGroupModel> GetAllByUserId(Guid userId);
        IEnumerable<IGroupModel> GetByIds(IEnumerable<Guid> ids);
        IEnumerable<IGroupModel> GetAllByUserIds(IEnumerable<Guid> userIds);
        IEnumerable<IGroupModel> GetByManagementId(Guid managementId);
        bool IsUniqueName(Guid managementId, string groupName);
    }
}
