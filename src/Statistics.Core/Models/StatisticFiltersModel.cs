using Statistics.Contract.Interfaces.Models;
using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Statistics.Core.Models
{
    public class StatisticFiltersModel : IStatisticFiltersModel
    {
        public IEnumerable<IManagementDepartmentStatisticModel> ManagementDepartments { get; set; }
        public IDictionary<Guid, string> CategoriesIdsToNamesRelation { get; set; }
        public IEnumerable<IdValueModel<Guid, string>> CancelingReasons { get; set; }
    }
}
