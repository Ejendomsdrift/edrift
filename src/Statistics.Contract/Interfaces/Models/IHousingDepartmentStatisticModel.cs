using System;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IHousingDepartmentStatisticModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}