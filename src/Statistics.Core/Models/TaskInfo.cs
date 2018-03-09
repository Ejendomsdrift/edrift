using YearlyPlanning.Contract.Enums;

namespace Statistics.Core.Models
{
    public class TaskInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string CompletitionDate { get; set; }
        public string RejectionDate { get; set; }
        public string CancelingDate { get; set; }
        public string CancelingReason { get; set; }
        public string RejectionReason { get; set; }
        public string StatusName { get; set; }
        public decimal SpentTime { get; set; }
        public string CreatorName { get; set; }
        public string TenantType { get; set; }
        public TenantTaskTypeEnum? TenantTypeEnum { get; set; }
        public string IsProcessedGroup { get; set; }
        public string IsOverdue { get; set; }
        public string TaskType { get; set; }
        public string HousingDepartmentName { get; set; }
        public string Address { get; set; }
        public string CategoryName { get; set; }
        public string OriginalTaskId { get; set; }
    }
}
