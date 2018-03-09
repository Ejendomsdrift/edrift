using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoRepository.Contract.Interfaces;
using MongoDB.Driver;
using System.Linq;

namespace MongoRepository.Implementation
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> collection;

        public Repository(IDbConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<T>(typeof(T).Name);
        }

        public IQueryable<T> Query => collection.AsQueryable();

        public IEnumerable<T> GetAll()
        {
            return collection.FindSync(FilterDefinition<T>.Empty).ToList(); 
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, IQueryOptions<T> options = null)
        {
            if (options == null)
            {
                return collection.FindSync(filter).ToList();
            }

            var optionsModel = GetFindOptions(options);
            return collection.FindSync(filter, optionsModel).ToList();
        }

        public T FindOne(Expression<Func<T, bool>> filter)
        {
            return collection.FindSync(filter, new FindOptions<T> { Limit = 1 }).FirstOrDefault();
        }

        public long DeleteAll()
        {
            return collection.DeleteMany(FilterDefinition<T>.Empty).DeletedCount;
        }

        public long Delete(Expression<Func<T, bool>> filter)
        {
            return collection.DeleteMany(filter).DeletedCount;
        }

        public void Save(T model)
        {
            collection.FindOneAndReplace(
                Builders<T>.Filter.Eq(m => m.Id, model.Id),
                model,
                new FindOneAndReplaceOptions<T> {IsUpsert = true});
        }

        public void Save(Expression<Func<T, bool>> filter, T model)
        {
            T element = FindOne(filter);
            
            if (element == null)
            {
                collection.InsertOne(model);
            }
            else
            {
                model.Id = element.Id;
                collection.ReplaceOne(filter, model);
            }
        }

        public void Save(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                Save(model);
            }
        }

        public void UpdateSingleProperty<TProp>(Guid id, Expression<Func<T, TProp>> property, TProp value)
        {
            var filter = Builders<T>.Filter.Eq(p => p.Id, id);
            var update = Builders<T>.Update.Set(property, value);
            collection.UpdateOne(filter, update);
        }

        public void UpdateManySingleProperty<TProp>(Expression<Func<T, bool>> filter, Expression<Func<T, TProp>> property, TProp value)
        {
            var update = Builders<T>.Update.Set(property, value);
            collection.UpdateMany(filter, update);
        }

        public IDictionary<TProp, IEnumerable<T>> FindWithGroupingByProperty<TProp>(Expression<Func<T, bool>> filter, Expression<Func<T, TProp>> property)
        {
            var filterResult = Query.Where(filter);
            var groupingResult = filterResult.GroupBy(property).ToList();
            var result = groupingResult.ToDictionary(pair => pair.Key, pair => (IEnumerable<T>) pair);
            return result;
        }

        private FindOptions<T> GetFindOptions(IQueryOptions<T> options)
        {
            var result = new FindOptions<T>();
            result.Limit = options.Take;
            result.Skip = options.Skip;

            if (options.SortField != null)
            {
                if (options.IsDescendingSort)
                {
                    result.Sort = new SortDefinitionBuilder<T>().Descending(options.SortField);
                }
                else
                {
                    result.Sort = new SortDefinitionBuilder<T>().Ascending(options.SortField);
                }
            }

            return result;
        }
    }
}
