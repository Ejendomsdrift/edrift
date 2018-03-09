using Infrastructure.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Web.Core.Attributes;
using Web.Models;
using YearlyPlanning.ReadModel;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [CompressFilter, PreventSpam]
    [RoutePrefix("api/yearPlan")]
    public class YearPlanController : ApiController
    {
        private readonly IYearlyPlanService yearlyPlanService;

        public YearPlanController(IYearlyPlanService yearlyPlanService)
        {
            this.yearlyPlanService = yearlyPlanService;
        }

        [HttpGet, Route("getDepartmentYearPlan")]
        public DepartmentYearPlanViewModel GetDepartmentYearPlan()
        {
            return yearlyPlanService.GetYearPlanCategories();
        }

        [HttpGet, Route("getYearPlanWeekData")]
        public IDictionary<string, List<YearPlanWeekData>> GetYearPlanWeekData(Guid departmentId, int year)
        {
            return yearlyPlanService.GetYearPlanWeekData(departmentId, year);
        }

        [HttpPost, Route("getAllDepartmentsYearPlan")]
        public IEnumerable<YearPlanItemViewModel> GetAllDepartmentsYearPlan(YearPlanViewModel model)
        {
            IEnumerable<YearPlanItemViewModel> result = yearlyPlanService.GetYearPlanDepartments(model.JobId, model.Year);
            return result;
        }

        [HttpGet, Route("getAllData")]
        public HousingDepartmentYearPlanModel GetAllData(Guid departmentId, int year)
        {
            return yearlyPlanService.GetAllData(departmentId, year);
        }
    }
}