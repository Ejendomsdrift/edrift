using System.Collections.Generic;
using MemberCore.Contract.Enums;

namespace SecurityCore.Contract.Interfaces
{
    public interface ISecurityService
    {

        Dictionary<string, bool> HasAccessByKeyList(ISecurityQuery query);

        Dictionary<string, bool> HasAccessByGroupName(ISecurityQuery query);

        void Save(ISecurityPermissionModel permission);

        IEnumerable<RoleType> GetRoles(string key);
    }
}
