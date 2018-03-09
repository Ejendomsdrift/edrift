using Translations.Models;

namespace Translations.Interfaces
{
    public interface ITranslationLogRepository
    {
        ResourceLog Get(string alias);

        void Save(ResourceLog entity);
    }
}