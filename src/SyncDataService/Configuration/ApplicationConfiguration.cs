using System.Configuration;
using SyncDataService.Interfaces;

namespace SyncDataService.Configuration
{
    public class ApplicationConfiguration : ConfigurationSection, IApplicationConfiguration
    {
        private const string ConnectionStringKey = "connectionString";

        public static ApplicationConfiguration Configuration => ConfigurationManager.GetSection("applicationConfiguration") as ApplicationConfiguration;

        [ConfigurationProperty(ConnectionStringKey, IsRequired = true)]
        public string ConnectionString => (string)base[ConnectionStringKey];
    }
}
