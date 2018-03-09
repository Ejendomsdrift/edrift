using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Contract.Interfaces.Models
{
    public interface ITaskChartModel
    {
        Guid TaskId { get; set; }
        JobTypeEnum JobType { get; set; }
        JobStatus JobStatus { get; set; }
        string TenantType { get; set; }
        Guid HousingDepartmentId { get; set; }
        Guid ManagementDepartmentId { get; set; }
        Guid CategoryId { get; set; }
        int CategorySortPriority { get; set; }
        decimal SpentTime { get; set; }
        bool IsOverdue { get; set; }
        IEnumerable<string> CancelingReasons { get; set; }
    }
}
