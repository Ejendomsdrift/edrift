using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;
using Translations.Configurations;
using Translations.Interfaces;
using Translations.Models;

namespace Translations.Implementation
{
    public class TranslationLogRepository : ITranslationLogRepository
    {
        public readonly IMongoCollection<ResourceLog> logCollection;

        public TranslationLogRepository(ITranslationRepositoryConfiguration configuration)
        {
            var database = new MongoClient(configuration.ConnectionString).GetDatabase(configuration.DatabaseName);
            logCollection = database.GetCollection<ResourceLog>(nameof(ResourceLog));
        }

        public ResourceLog Get(string alias)
        {
            List<ResourceLog> logs = logCollection.Find(r => r.Resource.Alias == alias).ToList();

            if (logs.Any())
            {
                return logs.OrderByDescending(r => r.Date).First();
            }

            return null;
        }

        public void Save(ResourceLog entity)
        {
            logCollection.InsertOne(entity);
        }
    }
}