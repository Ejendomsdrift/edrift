using System;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IAddressStatisticInfo
    {
        Guid ManagementDepartmentId { get; set; }
        Guid HousingDepartmentId { get; set; }
        string HousingDepartmentName { get; set; }
        decimal SpentTime { get; set; }
        string Address { get; set; }
        string Amount { get; set; }
    }
}