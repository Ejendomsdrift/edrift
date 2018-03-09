using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Contract.Interfaces.ChartsDataBuilders
{
    public interface ISpentTimeChartDataBuilder
    {
        IChartData<ITaskChartModel> Build(IChartDataQueryingRestrictions restrictions, ISpentTimeChartConfig config, ITimePeriod period, IDictionary<Guid, int> categorySortPriority);
    }
}