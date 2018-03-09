using MongoEventStore.Configurations;

namespace MongoEventStore.Tests.Stubs
{
    internal class LocalTestConfiguration : IMongoEventStoreConfiguration
    {
        public string ConnectionString { get; } = "mongodb://localhost:27017/";
        public string DatabaseName { get; } = "Edrift_MongoEventStore_Tests";
    }
}
