using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing.Implementation
{
    public abstract class EventBase : IEvent
    {
        public string SourceId { get; set; }
    }
}