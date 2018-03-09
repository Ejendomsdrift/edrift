using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoRepository.Contract.Interfaces;

namespace MongoRepository.Implementation
{
    public class MongoDatabaseRepository: IMongoDatabaseRepository
    {
        private readonly IMongoDatabase database;
        public MongoDatabaseRepository(IDbConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            database = client.GetDatabase(configuration.DatabaseName);
        }

        public IEnumerable<string> GetAllCollections()
        {
            var result = database.ListCollections().ToList();
            return result.Select(c=>c["name"].ToString());
        }

        public async Task DropDataBase(IEnumerable<string> collections)
        {
            foreach(var collection in collections)
            {
               await database.DropCollectionAsync(collection);
            }
        }
    }
}
