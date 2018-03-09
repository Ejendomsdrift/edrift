using System;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace Web.Models
{
    public class JobHeaderViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Address { get; set; }
        public JobStatus JobStatus { get; set; }
        public JobTypeEnum JobType { get; set; }
        public string Color { get; set; }
        public int? Estimate { get; set; }
        public bool? IsUrgent { get; set; }
        public string AssignedHousingDepartmentName { get; set; }
        public string CancellingReason { get; set; }
    }
}