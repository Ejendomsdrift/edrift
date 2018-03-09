using System.Threading.Tasks;
using System.Web.Http;
using MemberCore.Contract.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/sync")]
    public class SyncController : ApiController
    {
        private readonly IMemberService memberService;        
        private readonly IManagementDepartmentService managementService;
        private readonly IJobService jobService;
        private readonly IWeekPlanService weeklyPlanService;

        public SyncController(
            IMemberService memberService,
            IManagementDepartmentService managementService,
            IJobService jobService, 
            IWeekPlanService weeklyPlanService)
        {
            this.memberService = memberService;            
            this.managementService = managementService;
            this.jobService = jobService;
            this.weeklyPlanService = weeklyPlanService;
        }

        [HttpPost]
        [Route("syncMembers")]
        public void SyncMembers(object syncMembers)
        {
            var activeUserIds = jobService.GetUserIdsFromActiveJobs();
            memberService.SyncMembers(syncMembers, activeUserIds);
        }

        [HttpPost]
        [Route("syncMembersAvatars")]
        public void SyncMembersAvatars()
        {
            memberService.SyncMembersAvatars();
        }

        [HttpPost]
        [Route("syncManagements")]
        public void SyncManagements(object syncManagementDepartments)
        {
            managementService.SyncManagementDepartments(syncManagementDepartments);
        }

        [HttpGet]
        [Route("moveJobs")]
        public async Task MoveJobs(bool checkWeek = true)
        {
            await weeklyPlanService.MoveExpiredJobs(checkWeek);
        }
    }
}