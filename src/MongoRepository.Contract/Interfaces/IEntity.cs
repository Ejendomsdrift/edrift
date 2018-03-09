using System;

namespace MongoRepository.Contract.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
