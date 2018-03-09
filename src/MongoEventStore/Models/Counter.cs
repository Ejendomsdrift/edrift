using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.Models
{
    [BsonIgnoreExtraElements]
    public class Counter
    {
        [BsonId]
        public string Id { get; set; }

        public long Count { get; set; }
    }
}