using MongoRepository.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Models
{
    public class HousingDepartment : IEntity, IEquatable<HousingDepartment>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string SyncDepartmentId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsDeleted { get; set; }

        public List<string> AddressList { get; set; } = new List<string>();

        public bool Equals(HousingDepartment other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return SyncDepartmentId.Equals(other.SyncDepartmentId);
        }

        public override int GetHashCode()
        {
            return SyncDepartmentId.GetHashCode();
        }
    }
}
