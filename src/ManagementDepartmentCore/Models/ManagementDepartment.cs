using MongoRepository.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Models
{    
    public class ManagementDepartment: IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string SyncDepartmentId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string ManagementDepartmentRefId { get; set; }

        public List<HousingDepartment> HousingDepartmentList { get; set; } = new List<HousingDepartment>();

        public bool IsDeleted { get; set; }
    }
}
