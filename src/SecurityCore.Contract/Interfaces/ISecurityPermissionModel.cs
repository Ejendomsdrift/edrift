using System.Collections.Generic;

namespace SecurityCore.Contract.Interfaces
{
    public interface ISecurityPermissionModel
    {
        string Key { get; set; }
        string GroupName { get; set; }
        List<IRuleModel> Rules { get; set; }
    }
}