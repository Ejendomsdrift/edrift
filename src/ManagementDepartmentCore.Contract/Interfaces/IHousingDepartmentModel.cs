using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Contract.Interfaces
{
    public interface IHousingDepartmentModel
    {
        Guid Id { get; set; }

        string SyncDepartmentId { get; set; }

        Guid ManagementDepartmentId { get; set; }

        string Name { get; set; }

        string DisplayName { get; }

        string Type { get; set; }

        bool IsDeleted { get; set; }

        IEnumerable<string> AddressList { get; set; }
    }
}
