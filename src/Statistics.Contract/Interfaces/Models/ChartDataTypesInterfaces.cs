using System.Collections.Generic;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IChartData<TEntity>
    {
        IEnumerable<TEntity> Data { get; set; }
    }

    public interface IRatioChartData<TGroup, TEntity> : IChartData<TEntity>
    {
        IDictionary<string, IEnumerable<TGroup>> Groups { get; set; }
    }
}
