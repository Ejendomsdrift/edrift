using System.Collections.Generic;

namespace Web.Models.Security
{
    public class SecurityPermissionViewModel
    {
        public string Key { get; set; }
        public string GroupName { get; set; }
        public List<RuleViewModel> Rules { get; set; }
    }
}