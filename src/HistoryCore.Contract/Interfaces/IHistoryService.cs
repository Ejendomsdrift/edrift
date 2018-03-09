using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HistoryCore.Contract.Interfaces
{
    public interface IHistoryService
    {
        Task<IEnumerable<IHistoryModel>> GetChangeStatusHistory(Guid dayAssignId);
        Task<IEnumerable<IHistoryModel>> GetCanceledHistory(Guid dayAssignId);
        IEnumerable<IHistoryModel> GetChangeStatusHistory(string address);
    }
}
