using System.Configuration;
using FileStorage.Configuration;
using MongoEventStore.Configurations;
using Translations.Configurations;
using YearlyPlanning.Configuration;
using MongoRepository.Contract.Interfaces;

namespace Web.Core.Configurations
{
    public class MongoDBConfiguration : ConfigurationSection, 
        IDbConfiguration,
        ITranslationRepositoryConfiguration,
        IYearlyPlanningConfiguration,
        IMongoEventStoreConfiguration,
        IFileStorageConfiguration
    {
        private const string ConnectionStringKey = "ConnectionString";
        private const string DatabaseNameKey = "DatabaseName";

        public static MongoDBConfiguration Configuration => ConfigurationManager.GetSection("mongoConfiguration") as MongoDBConfiguration;

        [ConfigurationProperty(ConnectionStringKey, IsRequired = true)]
        public string ConnectionString => ((string)base[ConnectionStringKey]).Replace("{DB_NAME}", DatabaseName);

        [ConfigurationProperty(DatabaseNameKey, IsRequired = true)]
        public string DatabaseName => (string)base[DatabaseNameKey];
    }
}
