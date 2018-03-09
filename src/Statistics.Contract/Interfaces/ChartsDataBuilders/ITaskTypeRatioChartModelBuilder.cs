using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Contract.Interfaces.ChartsDataBuilders
{
    public interface ITaskRatioChartModelBuilder
    {
        IRatioChartData<TGroup, ITaskChartModel> Build<TGroup>(IChartDataQueryingRestrictions restrictions, ITaskRatioCharConfig<TGroup> chartConfig, ITimePeriod period, 
            bool showLastCompletedOrCanceledStatus, IDictionary<Guid, int> categorySortPriority);
    }
}