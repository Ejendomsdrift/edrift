using System.Collections.Generic;
using YearlyPlanning.ReadModel;

namespace Web.Models
{
    public class FacilityTaskAssignModel
    {
        public string TaskId { get; set; }

        public List<FacilityTaskDepartmentAssignViewModel> Departments { get; set; }
    }
}