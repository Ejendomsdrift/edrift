using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Models.Group
{
    public class GroupViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<Guid> MemberIds { get; set; } = Enumerable.Empty<Guid>();
        public IEnumerable<MemberViewModel> AllowedMembers { get; set; } = Enumerable.Empty<MemberViewModel>();
    }
}