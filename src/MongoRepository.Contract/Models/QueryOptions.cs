using System;
using System.Linq.Expressions;
using MongoRepository.Contract.Interfaces;

namespace MongoRepository.Contract.Models
{
    public class QueryOptions<T> : IQueryOptions<T> where T : IEntity
    {
        public Expression<Func<T, object>> SortField { get; set; }

        public bool IsDescendingSort { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public QueryOptions()
        {
            Take = int.MaxValue;
        }
    }
}
