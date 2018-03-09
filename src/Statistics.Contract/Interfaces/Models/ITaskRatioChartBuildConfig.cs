using System;
using System.Collections.Generic;

namespace Statistics.Contract.Interfaces.Models
{
    public interface ITaskRatioCharConfig<T>
    {
        IDictionary<string, IEnumerable<T>> TypesGroupings { get; set; }
        Func<ITaskChartModel, T> TaskGroupingSelector { get; set; }
    }
}