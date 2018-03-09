using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IStatisticFiltersModel
    {
        IEnumerable<IManagementDepartmentStatisticModel> ManagementDepartments { get; set; }
        IDictionary<Guid, string> CategoriesIdsToNamesRelation { get; set; }
        IEnumerable<IdValueModel<Guid, string>> CancelingReasons { get; set; }
    }
}