using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Translations.Configurations;
using Translations.Interfaces;
using Translations.Models;

namespace Translations.Implementation
{
    public class TranslationRepository : ITranslationRepository
    {
        public readonly IMongoCollection<Resource> collection;

        public IQueryable<Resource> Query => collection.AsQueryable();

        public TranslationRepository(ITranslationRepositoryConfiguration configuration)
        {
            var database = new MongoClient(configuration.ConnectionString).GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<Resource>(nameof(Resource));
        }

        public Task<List<Resource>> GetAll()
        {
            return collection.Aggregate().ToListAsync();
        }

        public async Task<Resource> Get(string alias)
        {
            using (var cursor = await collection.FindAsync(f => f.Alias == alias, new FindOptions<Resource> { Limit = 1 }))
            {
                return cursor.FirstOrDefault();
            }
        }

        public Resource GetResource(string alias)
        {
            var resource = collection.Find(r => r.Alias == alias).SingleOrDefault();
            return resource;
        }

        public void Save(Resource entity)
        {
            collection.FindOneAndReplace(Builders<Resource>.Filter.Eq(x => x.Alias, entity.Alias), entity,
               new FindOneAndReplaceOptions<Resource> { IsUpsert = true });
        }

        public async Task Delete(string alias)
        {
            await collection.DeleteOneAsync(Builders<Resource>.Filter.Eq(x => x.Alias, alias));
        }
    }
}