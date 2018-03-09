using SyncDataService.Interfaces;
using System.Configuration;

namespace SyncDataService.Configuration
{
    public class RestClientConfiguration : ConfigurationSection, IRestClientConfiguration
    {
        private const string AccessTokenKey = "accessToken";

        public static RestClientConfiguration Configuration => ConfigurationManager.GetSection("restClientConfiguration") as RestClientConfiguration;

        [ConfigurationProperty(AccessTokenKey, IsRequired = true)]
        public string AccessToken => (string)base[AccessTokenKey];
    }
}