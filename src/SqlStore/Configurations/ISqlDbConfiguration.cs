namespace SqlStore.Configurations
{
    public interface ISqlDbConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
