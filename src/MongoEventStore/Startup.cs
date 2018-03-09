using Infrastructure.EventSourcing.Implementation;
using Infrastructure.Extensions;
using MongoDB.Bson.Serialization;
using System;
using System.Linq;

namespace MongoEventStore
{
    public static class Startup
    {
        public static void RegisterBsonClassMaps()
        {
            var baseType = typeof(EventBase);
            var events = baseType.GetInheritedClasses().Where(i => i != baseType);

            foreach (var ev in events)
            {
                if (BsonClassMap.IsClassMapRegistered(ev)) return;

                var cm = (BsonClassMap)Activator.CreateInstance(typeof(BsonClassMap<>).MakeGenericType(ev));
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                BsonClassMap.RegisterClassMap(cm);
            }
        }
    }
}
