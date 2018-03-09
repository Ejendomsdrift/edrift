using System;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJobHeaderModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Title { get; set; }
        DateTime? Date { get; set; }
        string Address { get; set; }
        JobStatus JobStatus { get; set; }
        JobTypeEnum JobType { get; set; }
        string Color { get; set; }
        int? Estimate { get; set; }
        bool? IsUrgent { get; set; }
        string AssignedHousingDepartmentName { get; set; }
        string CancellingReason { get; set; }
    }
}