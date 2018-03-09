using Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Infrastructure.CustomAttributes;
using Web.Models;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [RoutePrefix("api/weekPlan")]
    public class WeekPlanController : ApiController
    {
        private readonly IWeekPlanService weeklyPlanService;
        private readonly IDayAssignService dayAssignService;

        public WeekPlanController(
            IWeekPlanService weeklyPlanService,
            IDayAssignService dayAssignService)
        {
            this.weeklyPlanService = weeklyPlanService;
            this.dayAssignService = dayAssignService;
        }

        [HttpPost, Route("getWeekTasks")]
        public IWeekPlanGridModel GetWeekTasks(WeekPlanTaskRequestViewModel model)
        {
            var filter = model.Map<IWeekPlanFilterModel>();
            return weeklyPlanService.GetWeekPlanGridModel(filter);
        }

        [PreventSpam]
        [HttpPost, Route("getJobsForWeek")]
        public IWeekJobsResultModel GetJobsForWeek(WeekPlanTaskRequestViewModel model)
        {
            var filter = model.Map<IWeekPlanFilterModel>();
            var result = weeklyPlanService.GetJobsForWeek(filter);
            return result;
        }

        [HttpPost, Route("getDayAssignIdsForMembers")]
        public IEnumerable<Guid> GetDayAssignIdsForMembers(WeekPlanTaskRequestViewModel model)
        {
            var filter = model.Map<ITaskDataFilterModel>();
            var ids = dayAssignService.GetDayAssignIds(filter);
            return ids;
        }
    }
}