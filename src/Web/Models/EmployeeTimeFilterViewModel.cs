using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class EmployeeTimeFilterViewModel
    {
        public Guid ManagementDepartmentId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public IEnumerable<Guid> MemberIds { get; set; }
    }
}