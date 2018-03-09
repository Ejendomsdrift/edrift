using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Extensions
{
    public static class SerializationExtentions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject((object) obj, Formatting.None, Settings);
        }

        public static T Deserialize<T>(this object obj)
        {
            return obj == null ? default(T) : obj.ToString().Deserialize<T>();
        }

        public static T Deserialize<T>(this string value)
        {
            return (T) JsonConvert.DeserializeObject(value, typeof(T), Settings);
        }
    }
}