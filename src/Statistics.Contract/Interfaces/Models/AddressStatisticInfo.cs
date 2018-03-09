using System;

namespace Statistics.Contract.Interfaces.Models
{
    public class AddressStatisticInfo : IAddressStatisticInfo
    {
        public Guid ManagementDepartmentId { get; set; }
        public Guid HousingDepartmentId { get; set; }
        public string HousingDepartmentName { get; set; }
        public string Address { get; set; }
        public decimal SpentTime { get; set; }
        public string Amount { get; set; }        
    }
}