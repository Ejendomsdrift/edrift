using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Translations.Models;

namespace Translations.Interfaces
{
    public interface ITranslationService
    {
        IDictionary<string, string> Get(IEnumerable<string> keys, string language);

        Task<Dictionary<string, string>> Translations(string language);

        Task<IEnumerable<ResourceModel>> All();

        Task<IEnumerable<string>> Languages();

        Task<ResourceModel> Get(string alias, string language);

        void Save(ResourceModel model);

        Task Delete(string alias);

        void Import(string json);

        Task<Stream> Export();
    }
}
