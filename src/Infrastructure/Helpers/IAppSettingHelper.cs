using System.Collections.Generic;

namespace Infrastructure.Helpers
{
    public interface IAppSettingHelper
    {
        T GetAppSetting<T>(string key, bool isRequired = true);

        IEnumerable<T> GetCollectionAppSetting<T>(string key, string separator, bool isRequired = true);

        Dictionary<TK, TV> GetDictionaryAppSetting<TK, TV>(string key, string itemSeparator = ",", string keyValueSeparator = "=");

        T GetFromJson<T>(string key);
    }
}
