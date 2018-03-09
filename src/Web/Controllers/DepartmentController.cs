using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
    [RoutePrefix("api/department")]
    public class DepartmentController : ApiController
    {
        private readonly IManagementDepartmentService managementDepartmentService;
        private readonly IMemberService memberService;

        public DepartmentController(
            IManagementDepartmentService managementDepartmentService,
            IMemberService memberService)
        {
            this.managementDepartmentService = managementDepartmentService;
            this.memberService = memberService;
        }

        [HttpGet, Route("getHousingDepartments")]
        public IEnumerable<HousingDepartmentViewModel> GetHousingDepartments()
        {
            IMemberModel currentMember = memberService.GetCurrentUser();

            IEnumerable<IHousingDepartmentModel> housingDepartments = currentMember.IsAdmin()
                ? managementDepartmentService.GetAllHousingDepartments()
                : managementDepartmentService.GetHousingDepartments(currentMember.ActiveManagementDepartmentId.Value);
            
            var housingDepartmentsView = housingDepartments.Map<IEnumerable<HousingDepartmentViewModel>>();
            return housingDepartmentsView.OrderBy(x => x.Name);
        }

        [HttpGet, Route("getDepartmentInfoById")]
        public IHousingDepartmentModel GetDepartmentInfoById(Guid housingDepartmentId)
        {
            return managementDepartmentService.GetHousingDepartment(housingDepartmentId);            
        }
    }
}