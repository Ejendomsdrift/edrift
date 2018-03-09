using System.Collections.Generic;
using Infrastructure.Enums;
using MemberCore.Contract.Enums;
using Web.Enums;

namespace Web.Models.Security
{
    public class RuleViewModel
    {
        public Pages? Page { get; set; }
        public List<RoleType> ViewRoleList { get; set; } = new List<RoleType>();
        public List<RoleType> EditRoleList { get; set; } = new List<RoleType>();
        public List<RoleType> UserRoleList { get; set; } = new List<RoleType>();
        public List<PlatformType> AllowedPlatformList { get; set; } = new List<PlatformType>();
        public bool IsEditable { get; set; }
        public bool IsUserShouldHaveAllRoles { get; set; }
        public bool IsDisabledForGroupingTask { get; set; }
    }
}