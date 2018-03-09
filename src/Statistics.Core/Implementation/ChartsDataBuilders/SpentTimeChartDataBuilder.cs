using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.ChartsDataBuilders;
using Statistics.Contract.Interfaces.Models;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation.ChartsDataBuilders
{
    public class SpentTimeChartDataBuilder : ISpentTimeChartDataBuilder
    {
        private readonly IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector;
        private readonly ITaskChartModelBuilder taskChartModelBuilder;

        public SpentTimeChartDataBuilder(IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector, ITaskChartModelBuilder taskChartModelBuilder)
        {
            this.dayAssignsTimeSpanSelector = dayAssignsTimeSpanSelector;
            this.taskChartModelBuilder = taskChartModelBuilder;
        }

        public IChartData<ITaskChartModel> Build(IChartDataQueryingRestrictions restrictions, ISpentTimeChartConfig config, ITimePeriod period, IDictionary<Guid, int> categorySortPriority)
        {
            IEnumerable<IDayAssign> dayAssigns = dayAssignsTimeSpanSelector.Get(period, restrictions);
            IEnumerable<ITaskChartModel> taskChartModels = taskChartModelBuilder.Build(restrictions.AccessibleManagementToHousingDepartmentsRelation, dayAssigns, categorySortPriority,
                restrictions.ShowLastCompletedOrCanceledStatus);
            IEnumerable<ITaskChartModel> filteredTaskChartModels = taskChartModels.Where(t => config.TaskTypesToInclude.Contains(t.JobType));

            var result = new ChartData<ITaskChartModel>
            {
                Data = filteredTaskChartModels
            };

            return result;
        }
    }
}
