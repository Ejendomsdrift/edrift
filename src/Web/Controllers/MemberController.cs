using System;
using MemberCore.Contract.Interfaces;
using System.Collections.Generic;
using System.Web.Http;
using Web.Models;
using System.Linq;
using Infrastructure.Extensions;
using MemberCore.Contract.Enums;
using Microsoft.AspNet.SignalR;
using Web.Core.Hubs;

namespace Web.Controllers
{
    [RoutePrefix("api/member")]
    public class MemberController : ApiController
    {
        private readonly IMemberService memberService;
        private IHubContext janitorHubs;
        private IHubContext managementHubs;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
            this.janitorHubs = GlobalHost.ConnectionManager.GetHubContext<JanitorHub>();
            this.managementHubs = GlobalHost.ConnectionManager.GetHubContext<ManagementHub>();
        }

        [HttpGet, Route("getMembers")]
        public IEnumerable<IMemberModel> GetMembers()
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            if (currentUser.ActiveManagementDepartmentId.HasValue)
            {
                return memberService.GetEmployeesByManagementDepartment(currentUser.ActiveManagementDepartmentId.Value);
            }
            return memberService.GetEmployees();
        }

        [HttpGet, Route("getAllowedMembersForJob")]
        public IEnumerable<IMemberModel> GetAllowedMembersForJob()
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            if (currentUser.ActiveManagementDepartmentId.HasValue)
            {
                return memberService.GetAllowedMembersForJobByDepartmentId(currentUser.ActiveManagementDepartmentId.Value);
            }

            return memberService.GetAllowedMembersForJob();
        }

        [HttpGet, Route("getCurrentUser")]
        public IMemberModel GetCurrentUser()
        {
            return memberService.GetCurrentUser();
        }

        [HttpGet, Route("getCurrentEmployee")]
        public MemberSettingsViewModel GetCurrentEmployee()
        {
            var member = memberService.GetCurrentUser();
            var departments = memberService.GetUserManagementDepartments(member.MemberId).Select(d => d.Map<ManagementViewModel>());
            return new MemberSettingsViewModel
            {
                MemberModel = member,
                Departments = departments
            };
        }

        [HttpGet, Route("getCurrentUserContext")]
        public CurrentUserContextViewModel GetCurrentUserContext()
        {
            var result = memberService.GetCurrentUserContext();
            var mappedResult = result.Map<CurrentUserContextViewModel>();
            return mappedResult;
        }

        [HttpPost, Route("switchMemberToNextAvailableRole")]
        public void SwitchMemberToNextAvailableRole(Guid memberId)
        {
            var newRole = memberService.SwitchMemberToNextAvailableRole(memberId);
            switch (newRole)
            {
                case RoleType.Coordinator:
                    janitorHubs.Clients.User(memberId.ToString()).refreshPage();
                    break;

                case RoleType.Janitor:
                    managementHubs.Clients.User(memberId.ToString()).refreshPage();
                    break;
            }
        }

        [HttpPost, Route("updateEmployeeSettings")]
        public void UpdateEmployeeSettings(MemberSettingsUpdateModel model)
        {
            memberService.UpdateMember(model.DaysAhead, model.Department);
        }

        [HttpPost, Route("saveManagementDepartment")]
        public void SaveManagementDepartment(Guid managementDepartmentId)
        {
            memberService.SaveManagementDepartment(managementDepartmentId);
        }
    }
}