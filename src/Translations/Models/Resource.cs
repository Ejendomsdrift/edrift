using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Translations.Models
{
    public class Resource
    {
        [BsonId]
        public string Alias { get; set; }
        public string Description { get; set; }
        public IList<Translation> Translations { get; set; }

        public Resource()
        {
            Translations = new List<Translation>();
        }
    }
}
