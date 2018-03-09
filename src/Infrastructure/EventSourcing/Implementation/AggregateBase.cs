using System;
using System.Collections.Generic;
using System.Threading;
using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing.Implementation
{
    public abstract class AggregateBase : IAggregateRoot
    {
        private readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private readonly List<IEvent> uncommitedEvents = new List<IEvent>();
        private readonly Dictionary<Type, Action<IEvent>> transitions = new Dictionary<Type, Action<IEvent>>();

        public string Id { get; protected set; }

        public int Version { get; private set; } = -1;

        public IEnumerable<IEvent> UncommitedEvents => uncommitedEvents;

        public void ApplyEvent(IEvent e)
        {
            Lock.EnterWriteLock();
            try
            {
                var eventType = e.GetType();
                if (transitions.ContainsKey(eventType))
                {
                    transitions[eventType](e);
                }
                Version++;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void MarkEventsAsCommitted()
        {
            Lock.EnterWriteLock();
            try
            {
                uncommitedEvents.Clear();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        protected void RegisterTransition<T>(Action<T> transition) where T : IEvent
        {
            transitions.Add(typeof(T), o => transition((T)o));
        }

        protected void RaiseEvent(IEvent e)
        {
            e.SourceId = Id;
            ApplyEvent(e);
            uncommitedEvents.Add(e);
        }
    }
}
