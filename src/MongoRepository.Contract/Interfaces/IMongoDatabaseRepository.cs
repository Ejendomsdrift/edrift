using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository.Contract.Interfaces
{
    public interface IMongoDatabaseRepository
    {
        IEnumerable<string> GetAllCollections();
        Task DropDataBase(IEnumerable<string> collections);
    }
}
