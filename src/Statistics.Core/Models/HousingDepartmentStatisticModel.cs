using System;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Models
{
    public class HousingDepartmentStatisticModel : IHousingDepartmentStatisticModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}