using MongoDB.Bson.Serialization.Attributes;

namespace Translations.Models
{
    public class Translation
    {
        [BsonRequired]
        public string Language { get; set; }
        [BsonRequired]
        public string Value { get; set; }
    }
}
