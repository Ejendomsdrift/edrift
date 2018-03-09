using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.Models
{
    [BsonIgnoreExtraElements]
    public class EventWrap
    {
        public int StreamRevision { get; set; }
        public object Payload { get; set; }
    }
}