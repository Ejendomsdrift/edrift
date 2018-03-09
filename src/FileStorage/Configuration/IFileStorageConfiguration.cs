namespace FileStorage.Configuration
{
    public interface IFileStorageConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
