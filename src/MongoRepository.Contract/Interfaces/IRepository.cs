using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MongoRepository.Contract.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        IQueryable<T> Query { get; }

        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Expression<Func<T, bool>> filter, IQueryOptions<T> options = null);

        T FindOne(Expression<Func<T, bool>> filter);

        long DeleteAll();

        long Delete(Expression<Func<T, bool>> filter);

        void Save(T model);

        void Save(IEnumerable<T> models);

        void Save(Expression<Func<T, bool>> filter, T model);

        void UpdateSingleProperty<TProp>(Guid id, Expression<Func<T, TProp>> property, TProp value);

        void UpdateManySingleProperty<TProp>(Expression<Func<T, bool>> filter, Expression<Func<T, TProp>> property, TProp value);

        IDictionary<TProp, IEnumerable<T>> FindWithGroupingByProperty<TProp>(Expression<Func<T, bool>> filter, Expression<Func<T, TProp>> property);
    }
}
