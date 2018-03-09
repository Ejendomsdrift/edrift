using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class FacilityTaskAssignDepartmentModel
    {
        public string JobId { get; set; }

        public IEnumerable<Guid> AssignedDepartmentIds { get; set; }

        public IEnumerable<Guid> UnassignedDepartmentIds { get; set; }
    }
}