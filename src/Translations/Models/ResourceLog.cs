using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Translations.Models
{
    [BsonIgnoreExtraElements]
    public class ResourceLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public Resource Resource { get; set; }
        public Guid PreviousLogId { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public ActionTypes ActionType { get; set; }

        public ResourceLog()
        {
            Date = DateTime.Now;
        }
    }
}
