namespace MongoEventStore.Configurations
{
    public interface IMongoEventStoreConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
