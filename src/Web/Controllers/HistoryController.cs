using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using HistoryCore.Contract.Interfaces;
using YearlyPlanning.Contract.Interfaces;
using MemberCore.Contract.Interfaces;

namespace Web.Controllers
{
    [RoutePrefix("api/history")]
    public class HistoryController: ApiController
    {
        private readonly IHistoryService historyService;
        private readonly IJobService jobService;
        private readonly IMemberService memberService;

        public HistoryController(
            IHistoryService historyService, 
            IJobService jobService, 
            IMemberService memberService)
        {
            this.historyService = historyService;
            this.jobService = jobService;
            this.memberService = memberService;
        }

        [HttpGet, Route("getHistoryByAddress")]
        public IEnumerable<IHistoryModel> GetHistoryByAddress(string address)
        {
            return historyService.GetChangeStatusHistory(address);
        }
        
        [HttpGet, Route("getHistoryByDayAssignId")]
        public async Task<IEnumerable<IHistoryModel>> GetHistoryByDayAssignId(Guid dayAssignId)
        {
            return await historyService.GetChangeStatusHistory(dayAssignId);
        }

        [HttpGet, Route("getCanceledHistory")]
        public async Task<IEnumerable<IHistoryModel>> GetCanceledHistory(Guid dayAssignId)
        {
            return await historyService.GetCanceledHistory(dayAssignId);
        }

        [HttpGet, Route("getAddressesForManagementDepartment")]
        public IEnumerable<string> GetAddressesForManagementDepartment()
        {
            var currentMember = memberService.GetCurrentUser();                        
            return jobService.GetAddressesForManagementDepartment(currentMember.ActiveManagementDepartmentId);
        }        
    }
}