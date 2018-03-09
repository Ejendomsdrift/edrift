using System;
using System.Linq.Expressions;

namespace MongoRepository.Contract.Interfaces
{
    public interface IQueryOptions<T> where T : IEntity
    {
        Expression<Func<T, object>> SortField { get; set; }

        bool IsDescendingSort { get; set; }

        int Skip { get; set; }

        int Take { get; set; }
    }
}
