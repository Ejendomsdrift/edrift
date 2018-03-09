using System.Collections.Generic;

namespace Web.Models
{
    public class ManagementDepartmentTimeView
    {
        public IEnumerable<MemberTimeView> MembersTimeView { get; set; }
        public TimeViewModel MembersTotal { get; set; }
    }
}