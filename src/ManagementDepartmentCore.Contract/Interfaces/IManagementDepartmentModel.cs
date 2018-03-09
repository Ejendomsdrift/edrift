using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Contract.Interfaces
{
    public interface IManagementDepartmentModel
    {
        Guid Id { get; set; }

        string SyncDepartmentId { get; set; }

        string Type { get; set; }

        string Name { get; set; }

        string ManagementDepartmentRefId { get; set; }

        List<IHousingDepartmentModel> HousingDepartmentList { get; set; }

        bool IsDeleted { get; set; }
    }
}
