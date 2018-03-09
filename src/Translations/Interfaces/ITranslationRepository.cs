using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Translations.Models;

namespace Translations.Interfaces
{
    public interface ITranslationRepository
    {
        IQueryable<Resource> Query { get; }

        Task<List<Resource>> GetAll();

        Task<Resource> Get(string alias);

        Resource GetResource(string alias);

        void Save(Resource entity);

        Task Delete(string alias);
    }
}