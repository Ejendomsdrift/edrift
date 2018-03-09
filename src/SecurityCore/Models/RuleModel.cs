using System.Collections.Generic;
using Infrastructure.Enums;
using MemberCore.Contract.Enums;
using SecurityCore.Contract.Enums;
using SecurityCore.Contract.Interfaces;

namespace SecurityCore.Models
{
    public class RuleModel : IRuleModel
    {
        public SecurityPages? Page { get; set; }
        public List<RoleType> ViewRoleList { get; set; }
        public List<RoleType> EditRoleList { get; set; }
        public List<RoleType> UserRoleList { get; set; }
        public List<PlatformType> AllowedPlatformList { get; set; }
        public bool IsEditable { get; set; }
        public bool IsUserShouldHaveAllRoles { get; set; }
        public bool IsDisabledForGroupingTask { get; set; }
    }
}
