using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.EventSourcing;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoEventStore.Models
{
    [BsonIgnoreExtraElements]
    public class Commit
    {
        [BsonId]
        public long CheckpointNumber { get; set; }
        public long CommitSequence { get; set; }
        public DateTime CommitStamp { get; set; }

        public string AggregateId { get; set; }
        public int StreamRevisionFrom { get; set; }
        public int StreamRevisionTo { get; set; }
        public Guid? MemberId { get; set; }

        public ICollection<EventWrap> Events { get; set; }

        public static Commit FromAggregate(IAggregateRoot aggregate, long checkpoint, long commitSequence)
        {
            var uncommitedEvents = aggregate.UncommitedEvents.ToList();

            var streamRevision = aggregate.Version - (uncommitedEvents.Count - 1);
            var streamRevisionStart = streamRevision;
            var events = uncommitedEvents.Select(e => new EventWrap
            {
                StreamRevision = streamRevision++,
                Payload = e
            }).ToArray();

            return new Commit
            {
                CheckpointNumber = checkpoint,
                CommitSequence = commitSequence,
                CommitStamp = DateTime.UtcNow,

                AggregateId = aggregate.Id,

                StreamRevisionFrom = streamRevisionStart,
                StreamRevisionTo = streamRevision - 1,

                Events = events,
            };
        }
    }
}