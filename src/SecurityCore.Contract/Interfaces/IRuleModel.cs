using System.Collections.Generic;
using Infrastructure.Enums;
using MemberCore.Contract.Enums;
using SecurityCore.Contract.Enums;

namespace SecurityCore.Contract.Interfaces
{
    public interface IRuleModel
    {
        SecurityPages? Page { get; set; }
        List<RoleType> ViewRoleList { get; set; }
        List<RoleType> EditRoleList { get; set; }
        List<RoleType> UserRoleList { get; set; }
        List<PlatformType> AllowedPlatformList { get; set; }
        bool IsEditable { get; set; }
        bool IsUserShouldHaveAllRoles { get; set; }
        bool IsDisabledForGroupingTask { get; set; }
    }
}