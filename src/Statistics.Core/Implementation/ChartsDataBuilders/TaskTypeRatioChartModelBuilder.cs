using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Contract.Enums;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.ChartsDataBuilders;
using Statistics.Contract.Interfaces.Models;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation.ChartsDataBuilders
{
    public class TaskRatioChartModelBuilder : ITaskRatioChartModelBuilder
    {
        private readonly IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector;
        private readonly ITaskChartModelBuilder taskChartModelBuilder;

        public TaskRatioChartModelBuilder(IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector, ITaskChartModelBuilder taskChartModelBuilder)
        {
            this.dayAssignsTimeSpanSelector = dayAssignsTimeSpanSelector;
            this.taskChartModelBuilder = taskChartModelBuilder;
        }

        public IRatioChartData<TGroup, ITaskChartModel> Build<TGroup>(IChartDataQueryingRestrictions restrictions, ITaskRatioCharConfig<TGroup> chartConfig, ITimePeriod period, 
            bool showLastCompletedOrCanceledStatus, IDictionary<Guid, int> categorySortPriority)
        {
           
            IEnumerable<IDayAssign> dayAssigns = dayAssignsTimeSpanSelector.Get(period, restrictions);
            IEnumerable<ITaskChartModel> taskChartModels = taskChartModelBuilder.Build(
                restrictions.AccessibleManagementToHousingDepartmentsRelation,
                dayAssigns,
                categorySortPriority,
                showLastCompletedOrCanceledStatus);

            IEnumerable<ITaskChartModel> filteredTaskChartModels = FilterTaskChartModelsAccordingToGroupings(taskChartModels, chartConfig);

            var result = new RatioChartData<TGroup, ITaskChartModel>
            {
                Data = filteredTaskChartModels,
                Groups = chartConfig.TypesGroupings
            };

            return result;
        }

        private IEnumerable<ITaskChartModel> FilterTaskChartModelsAccordingToGroupings<T>(IEnumerable<ITaskChartModel> models, ITaskRatioCharConfig<T> chartConfig)
        {
            IEnumerable<T> involvedTaskTypes = chartConfig.TypesGroupings.SelectMany(grouping => grouping.Value);
            IEnumerable<ITaskChartModel> result = models.Where(m => involvedTaskTypes.Contains(chartConfig.TaskGroupingSelector(m)));
            return result;
        }
    }
}