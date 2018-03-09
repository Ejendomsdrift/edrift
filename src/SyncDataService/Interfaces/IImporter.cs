using System.Collections.Generic;
using System.Threading.Tasks;
using SyncDataService.Models;

namespace SyncDataService.Interfaces
{
    public interface IImporter
    {
        Task<List<MemberBO>> ImportMembers();

        Task<List<ManagementDepartmentBO>> ImportManagements();
    }
}