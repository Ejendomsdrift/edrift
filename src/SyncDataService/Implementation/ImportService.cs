using System.Threading.Tasks;
using SyncDataService.Interfaces;

namespace SyncDataService.Implementation
{
    public class ImportService : IImportService
    {
        private readonly RestClient restClient;
        private readonly IImporter importer;

        public ImportService(RestClient restClient, IImporter importer)
        {
            this.restClient = restClient;
            this.importer = importer;
        }

        public async Task SyncData()
        {
            await SyncManagements();
            await SyncMembers();
        }

        public async Task SyncMembersAvatars()
        {
            await restClient.Post("SyncMembersAvatars");
        }

        private async Task SyncManagements()
        {
            var managements = await importer.ImportManagements();
            await restClient.Post("SyncManagements", managements);
        }

        private async Task SyncMembers()
        {
            var members = await importer.ImportMembers();
            await restClient.Post("SyncMembers", members);
        }
    }
}