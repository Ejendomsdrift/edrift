namespace MongoRepository.Contract.Interfaces
{
    public interface IDbConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
