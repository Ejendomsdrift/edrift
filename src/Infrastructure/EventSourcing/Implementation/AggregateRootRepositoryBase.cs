using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing.Implementation
{
    public abstract class AggregateRootRepositoryBase<T> : IAggregateRootRepository<T> where T : IAggregateRoot, new()
    {
        public abstract Task<T> Get(string aggregateId);

        public abstract Task Save(T aggregate, Guid? userId = null);

        protected T BuildAggregate(IEnumerable<IEvent> events)
        {
            var result = new T();
            foreach (var e in events)
            {
                result.ApplyEvent(e);
            }
            return result;
        }

        protected int CalculateExpectedVersion(IAggregateRoot aggregate, List<T> events)
        {
            var expectedVersion = aggregate.Version - events.Count;
            return expectedVersion;
        }
    }
}