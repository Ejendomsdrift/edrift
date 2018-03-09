using System.Collections.Generic;

namespace Web.Models
{
    public class JobDepartmentsViewModel
    {
        public IEnumerable<HousingDepartmentViewModel> AssignedDepartments { get; set; }
        public IEnumerable<HousingDepartmentViewModel> GroupedDepartments { get; set; }
    }
}