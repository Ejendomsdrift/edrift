using Statistics.Contract.Interfaces.Models;
using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Contract.Interfaces
{
    public interface ITaskChartModelBuilder
    {
        IEnumerable<ITaskChartModel> Build(
            IDictionary<Guid, IEnumerable<Guid>> managementToHousingDepartmentsRelation,
            IEnumerable<IDayAssign> dayAssigns, 
            IDictionary<Guid, int> categorySortPriority,
            bool showLastCompletedOrCanceledStatus);
    }
}