using ManagementDepartmentCore.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Models
{
    public class HousingDepartmentModel : IHousingDepartmentModel
    {
        public Guid Id { get; set; }

        public string SyncDepartmentId { get; set; }

        public Guid ManagementDepartmentId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<string> AddressList { get; set; }

        public string DisplayName => $"{SyncDepartmentId} {Name}";
    }
}
