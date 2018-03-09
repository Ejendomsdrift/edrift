using System.Collections.Generic;
using SecurityCore.Contract.Interfaces;

namespace SecurityCore.Models
{
    public class SecurityPermissionModel : ISecurityPermissionModel
    {
        public string Key { get; set; }
        public string GroupName { get; set; }
        public List<IRuleModel> Rules { get; set; }
    }
}
