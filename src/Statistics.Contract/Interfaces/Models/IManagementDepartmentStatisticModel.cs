using System;
using System.Collections.Generic;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IManagementDepartmentStatisticModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        List<IHousingDepartmentStatisticModel> HousingDepartments { get; set; }
    }
}