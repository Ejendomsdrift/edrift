using YearlyPlanning.Contract.Enums;

namespace Web.Models.Task
{
    public class NewTenantTaskModel : NewOperationalTaskModel
    {
        public string Comment { get; set; } 
        public string ResidentName { get; set; } 
        public string ResidentPhone { get; set; } 
        public bool IsUrgent { get; set; }
        public TenantTaskTypeEnum Type { get; set; }
    }
}