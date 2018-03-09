using System;
using System.Collections.Generic;
using System.Web.Http;
using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Models;
using Infrastructure.Extensions;
using MemberCore.Contract.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    [RoutePrefix("api/employeesManagement")]
    public class EmployeesManagementController : ApiController
    {
        private readonly IEmployeeAbsenceInfoService absenceInfoService;
        private readonly IMemberService memberService;

        public EmployeesManagementController(
            IEmployeeAbsenceInfoService absenceInfoService,
            IMemberService memberService)
        {
            this.absenceInfoService = absenceInfoService;
            this.memberService = memberService;
        }

        [HttpGet, Route("GetAbsencesForCurrentManagementDepartment")]
        public IEnumerable<IEmployeeAbsences> GetAbsencesForCurrentManagementDepartment()
        {
            var currentUser = memberService.GetCurrentUser();
            var result = absenceInfoService.GetByManagementDeparmentId(currentUser.ActiveManagementDepartmentId.Value);
            return result;
        }

        [HttpPost, Route("AddAbsenceForMember")]
        public IAbsenceCreationResult AddAbsenceForMember(NewEmployeeAbsenceInfoModel absenceInfoBlank)
        {
            var absenceInfo = absenceInfoBlank.Map<EmployeeAbsenceInfoModel>();
            return absenceInfoService.Add(absenceInfo);
        }

        [HttpPost, Route("DeleteAbsence")]
        public void DeleteAbsence(Guid absenceId)
        {
            absenceInfoService.Delete(absenceId);
        }

        [HttpPost, Route("DeleteAllAbsencesForMember")]
        public void DeleteAllAbsencesForMember(Guid memberId)
        {
            absenceInfoService.DeleteAllAbsencesForMember(memberId);
        }
    }
}