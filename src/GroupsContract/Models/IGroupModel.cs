using System;
using System.Collections.Generic;

namespace GroupsContract.Models
{
    public interface IGroupModel
    {
        Guid Id { get; }
        string Name { get; }
        Guid ManagementId { get; }
        bool Deleted { get; }
        IEnumerable<Guid> MemberIds { get; }
    }
}
