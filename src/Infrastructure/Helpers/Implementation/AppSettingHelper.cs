using Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Infrastructure.Helpers.Implementation
{
    public class AppSettingHelper : IAppSettingHelper
    {
        public static T GetAppSetting<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            var result = Convert.ChangeType(value, typeof(T));
            return (T)result;
        }

        public T GetAppSetting<T>(string key, bool isRequired = true)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (!value.HasValue())
            {
                if (isRequired)
                {
                    throw new ArgumentException(StringExtensions.Format("The key {0} in appSettings is missing!", key));
                }

                return default(T);
            }

            var result = Convert.ChangeType(value, typeof(T));
            return (T)result;
        }

        public IEnumerable<T> GetCollectionAppSetting<T>(string key, string separator, bool isRequired = true)
        {
            string value = GetAppSetting<string>(key, isRequired);
            return value.GetCollection<T>(separator);
        }

        public Dictionary<TK, TV> GetDictionaryAppSetting<TK, TV>(string key, string itemSeparator = ",", string keyValueSeparator = "=")
        {
            string value = GetAppSetting<string>(key, true);
            var data = value.GetCollection<string>(itemSeparator);
            var dicData = data.Select(map => map.SplitBySeparator(keyValueSeparator))
                              .ToDictionary(k => (TK)Convert.ChangeType(k.First(), typeof(TK)),
                                            v => (TV)Convert.ChangeType(v.Last(), typeof(TV)));
            return dicData;
        }

        public T GetFromJson<T>(string key)
        {
            string value = GetAppSetting<string>(key, true);
            var result = value.Deserialize<T>();
            return result;
        }
    }
}
