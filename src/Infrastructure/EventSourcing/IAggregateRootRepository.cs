using System;
using System.Threading.Tasks;

namespace Infrastructure.EventSourcing
{
    public interface IAggregateRootRepository<T> where T : IAggregateRoot
    {
        Task<T> Get(string aggregateId);

        Task Save(T aggregate, Guid? userId = null);
    }
}