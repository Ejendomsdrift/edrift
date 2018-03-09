using System;

namespace Web.Models
{
    public class HousingDepartmentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ManagementDepartmentId { get; set; }
        public bool IsDisabled { get; set; }
    }
}