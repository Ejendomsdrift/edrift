using Statistics.Contract.Interfaces.Models;
using System;

namespace Statistics.Core.Models
{
    public class CancelingReasonDataModel : ICancelingReasonDataModel
    {
        public Guid HousingDepartmentId { get; set; }
        public Guid DayAssignId { get; set; }
        public Guid ReasonId { get; set; }
        public string Reason { get; set; }
        public string RejectionDate { get; set; }
        public decimal SpentTime { get; set; } // In hours
        public Guid ManagementDepartmentId { get; set; }
        public string JobId { get; set; }
        public string Title { get; set; }
        public string HousingDepartmentName { get; set; }
        public string Address { get; set; }
        public string CreatorName { get; set; }
        public string TenantType { get; set; }
        public string StatusName { get; set; }
    }
}
