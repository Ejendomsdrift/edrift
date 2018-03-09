using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.ChartBuildConfigs
{
    public class TaskRatioChartConfig<T> : ITaskRatioCharConfig<T>
    {
        public IDictionary<string, IEnumerable<T>> TypesGroupings { get; set; }
        public Func<ITaskChartModel, T> TaskGroupingSelector { get; set; }
    }
}