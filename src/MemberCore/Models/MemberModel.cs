using Infrastructure.Extensions;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace MemberCore.Models
{
    public class MemberModel : IMemberModel
    {
        public Guid MemberId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public IEnumerable<RoleType> Roles { get; set; }

        public RoleType CurrentRole { get; set; }

        public bool IsDeleted { get; set; }

        public string WorkingPhone { get; set; }

        public string MobilePhone { get; set; }

        public string Email { get; set; }

        public int DaysAhead { get; set; }

        public Lazy<IDictionary<RoleType, IEnumerable<Guid>>> LazyManagementsToActiveRolesRelation { get; set; }
        public IDictionary<RoleType, IEnumerable<Guid>> ManagementsToActiveRolesRelation => LazyManagementsToActiveRolesRelation?.Value ?? new Dictionary<RoleType, IEnumerable<Guid>>();

        public Lazy<Guid?> LazyActiveManagementDepartmentId { get; set; }
        public Guid? ActiveManagementDepartmentId => LazyActiveManagementDepartmentId?.Value;

        public bool IsAdmin()
        {
            return CurrentRole.In(RoleType.Administrator, RoleType.SuperAdmin);
        } 
    }
}
