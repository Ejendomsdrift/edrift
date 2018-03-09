using System;

namespace Web.Models
{
    public class YearPlanViewModel
    {
        public string JobId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ManagementDepartmentId { get; set; }
        public Guid? HousingDepartmentId { get; set; }
        public int Year { get; set; }
        public bool ShowDisabled { get; set; }
    }
}