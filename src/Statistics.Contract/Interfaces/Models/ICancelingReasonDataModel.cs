using System;

namespace Statistics.Contract.Interfaces.Models
{
    public interface ICancelingReasonDataModel
    {
        Guid HousingDepartmentId { get; set; }
        Guid ManagementDepartmentId { get; set; }
        Guid DayAssignId { get; set; }
        Guid ReasonId { get; set; }
        string Reason { get; set; }
        string RejectionDate { get; set; }
        decimal SpentTime { get; set; }
        string JobId { get; set; }
        string Title { get; set; }
        string HousingDepartmentName { get; set; }
        string Address { get; set; }
        string CreatorName { get; set; }
        string TenantType { get; set; }
        string StatusName { get; set; }
    }
}