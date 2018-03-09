using Statistics.Contract.Interfaces.Models;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Core.Models
{
    public class TaskChartModel : ITaskChartModel
    {
        public Guid TaskId { get; set; }
        public JobTypeEnum JobType { get; set; }
        public JobStatus JobStatus { get; set; }
        public string TenantType { get; set; }
        public Guid HousingDepartmentId { get; set; }
        public Guid ManagementDepartmentId { get; set; }
        public decimal SpentTime { get; set; } // in minutes
        public bool IsOverdue { get; set; }        
        public Guid CategoryId { get; set; }
        public int CategorySortPriority { get; set; }
        public IEnumerable<string> CancelingReasons { get; set; }
    }
}
