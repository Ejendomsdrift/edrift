using System.Collections.Generic;
using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing
{
    public interface IAggregateRoot
    {
        string Id { get; }

        int Version { get; }

        IEnumerable<IEvent> UncommitedEvents { get; }

        void ApplyEvent(IEvent e);

        void MarkEventsAsCommitted();
    }
}
