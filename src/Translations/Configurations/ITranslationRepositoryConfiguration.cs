namespace Translations.Configurations
{
    public interface ITranslationRepositoryConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
