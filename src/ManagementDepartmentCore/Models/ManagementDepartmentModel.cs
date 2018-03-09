using ManagementDepartmentCore.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Models
{
    public class ManagementDepartmentModel : IManagementDepartmentModel
    {
        public Guid Id { get; set; }

        public string SyncDepartmentId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string ManagementDepartmentRefId { get; set; }

        public List<IHousingDepartmentModel> HousingDepartmentList { get; set; }

        public bool IsDeleted { get; set; }
    }
}
