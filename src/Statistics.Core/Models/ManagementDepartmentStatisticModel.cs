using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Models
{
    public class ManagementDepartmentStatisticModel : IManagementDepartmentStatisticModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<IHousingDepartmentStatisticModel> HousingDepartments { get; set; }
    }
}