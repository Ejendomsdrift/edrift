using MemberCore.Contract.Enums;
using System;
using System.Collections.Generic;

namespace MemberCore.Contract.Interfaces
{
    public interface IMemberModel
    {
        Guid MemberId { get; }

        string UserName { get; }

        string Name { get; }

        string Avatar { get; }

        IEnumerable<RoleType> Roles { get; }

        RoleType CurrentRole { get; set; } 

        bool IsDeleted { get; }

        string WorkingPhone { get; }

        string MobilePhone { get; }

        string Email { get; }

        int DaysAhead { get; }

        IDictionary<RoleType, IEnumerable<Guid>> ManagementsToActiveRolesRelation { get; }

        Guid? ActiveManagementDepartmentId { get; }

        bool IsAdmin();
    }
}
