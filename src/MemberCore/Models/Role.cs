using MongoRepository.Contract.Interfaces;
using System;

namespace MemberCore.Models
{
    public class Role : IEntity, IEquatable<Role>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int RoleId { get; set; }

        public string ManagementDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        //Please note that interface IEquatable<Role> is required for Except linq method that uses in member service - do not delete code below
        //compare from IEquatable for except method when we set delete flag for absent roles
        public bool Equals(Role other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return RoleId.Equals(other.RoleId) && ManagementDepartmentId.Equals(other.ManagementDepartmentId);
        }

        public override int GetHashCode()
        {
            int departmentHashCode = ManagementDepartmentId == null ? 0 : ManagementDepartmentId.GetHashCode();
            int roleHashCode = RoleId.GetHashCode();

            return departmentHashCode ^ roleHashCode;
        }
    }
}
