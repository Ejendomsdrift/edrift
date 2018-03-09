using System.Threading.Tasks;

namespace SyncDataService.Interfaces
{
    public interface IImportService
    {
        Task SyncData();
        Task SyncMembersAvatars();
    }
}