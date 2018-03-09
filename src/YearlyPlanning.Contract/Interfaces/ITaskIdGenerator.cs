using System.Threading.Tasks;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface ITaskIdGenerator
    {
        Task<string> Facility();
        Task<string> AdHoc();
        Task<string> Tenant();
        Task<string> Other();
    }
}
