using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Implementation
{
    public class ChartData<TEntity> : IChartData<TEntity>
    {
        public IEnumerable<TEntity> Data { get; set; }
    }

    public class RatioChartData<TGroup, TEntity> : ChartData<TEntity>, IRatioChartData<TGroup, TEntity>
    {
        public IDictionary<string, IEnumerable<TGroup>> Groups { get; set; }
    }
}