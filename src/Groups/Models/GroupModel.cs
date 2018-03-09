using System;
using System.Collections.Generic;
using System.Linq;
using GroupsContract.Models;

namespace Groups.Models
{
    public class GroupModel: IGroupModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<Guid> MemberIds { get; set; } = Enumerable.Empty<Guid>();
    }
}
