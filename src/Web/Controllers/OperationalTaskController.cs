using AutoMapper;
using Infrastructure.Extensions;
using MemberCore.Contract.Interfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Core.Hubs;
using Web.Models;
using Web.Models.Task;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace Web.Controllers
{
    [RoutePrefix("api/operationalTask")]
    public class OperationalTaskController : ApiController
    {
        private readonly IOperationalTaskService operationalTaskService;
        private readonly IMemberService memberService;
        private IHubContext janitorHubs;
        private IHubContext managementHubs;

        public OperationalTaskController(IOperationalTaskService operationalTaskService, IMemberService memberService)
        {
            this.operationalTaskService = operationalTaskService;
            this.memberService = memberService;
            this.janitorHubs = GlobalHost.ConnectionManager.GetHubContext<JanitorHub>();
            this.managementHubs = GlobalHost.ConnectionManager.GetHubContext<ManagementHub>();
        }

        [HttpPost, Route("createAdHocTask")]
        public async Task<CreateOperationalTaskViewModel> CreateAdHocTask(NewAdHocTaskModel model)
        {
            var mappedModel = model.Map<IOperationalTaskModel>();
            var result = await operationalTaskService.CreateAdHocTask(mappedModel);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpPost, Route("createJanitorAdHocTask")]
        public async Task<CreateOperationalTaskViewModel> CreateJanitorAdHocTask(NewAdHocTaskModel model)
        {
            CreateOperationalTaskViewModel result = await CreateAdHocTask(model);
            if (model.TeamLeadId.HasValue)
            {
                var assignedMemberModel = model.Map<MemberAssignModel>();
                assignedMemberModel.DayAssignId = result.DayAssignId;
                await AssignEmployees(assignedMemberModel);
            }
            return result;
        }

        [HttpPost, Route("createTenantTask")]
        public async Task<CreateOperationalTaskViewModel> CreateTenantTask(NewTenantTaskModel model)
        {
            var mappedModel = model.Map<IOperationalTaskModel>();
            var result = await operationalTaskService.CreateTenantTask(mappedModel);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpPost, Route("createJanitorTenantTask")]
        public async Task<CreateOperationalTaskViewModel> CreateJanitorTenantTask(NewTenantTaskModel model)
        {
            CreateOperationalTaskViewModel result = await CreateTenantTask(model);
            if (model.TeamLeadId.HasValue)
            {
                var assignedMemberModel = model.Map<MemberAssignModel>();
                assignedMemberModel.DayAssignId = result.DayAssignId;
                await AssignEmployees(assignedMemberModel);
            }
            return result;
        }

        [HttpPost, Route("createOtherTask")]
        public async Task<CreateOperationalTaskViewModel> CreateOtherTask(NewOtherTaskModel model)
        {
            var mappedModel = model.Map<IOperationalTaskModel>();
            var result = await operationalTaskService.CreateOtherTask(mappedModel);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpPost, Route("createJanitorOtherTask")]
        public async Task<CreateOperationalTaskViewModel> CreateJanitorOtherTask(NewOtherTaskModel model)
        {
            CreateOperationalTaskViewModel result = await CreateOtherTask(model);
            if (model.TeamLeadId.HasValue)
            {
                var assignedMemberModel = model.Map<MemberAssignModel>();
                assignedMemberModel.DayAssignId = result.DayAssignId;
                await AssignEmployees(assignedMemberModel);
            }
            return result;
        }

        [HttpGet, Route("getAdHocTask")]
        public async Task<AdHocViewModel> GetAdHocTask(Guid dayAssignId)
        {
            var adHocTask = await operationalTaskService.GetTask(dayAssignId, JobTypeEnum.AdHock);
            var result = Mapper.Map<AdHocViewModel>(adHocTask);
            foreach (var upload in result.Uploads)
            {
                upload.Uploader = GetUploaderModel(upload.UploaderId);
            }
            return result;
        }

        [HttpGet, Route("getTenantTask")]
        public async Task<TenantViewModel> GetTenantTask(Guid dayAssignId)
        {
            var tenantTask = await operationalTaskService.GetTask(dayAssignId, JobTypeEnum.Tenant);
            var result = Mapper.Map<TenantViewModel>(tenantTask);
            foreach (var upload in result.Uploads)
            {
                upload.Uploader = GetUploaderModel(upload.UploaderId);
            }
            return result;
        }

        [HttpGet, Route("getOtherTask")]
        public async Task<OtherTaskViewModel> GetOtherTask(Guid dayAssignId)
        {
            var otherTask = await operationalTaskService.GetTask(dayAssignId, JobTypeEnum.Other);
            var result = Mapper.Map<OtherTaskViewModel>(otherTask);
            foreach (var upload in result.Uploads)
            {
                upload.Uploader = GetUploaderModel(upload.UploaderId);
            }
            return result;
        }

        [HttpPost, Route("changeTenantTaskType")]
        public async Task ChangeTenantTaskType(IdValueModel<Guid, TenantTaskTypeEnum> model)
        {
            await operationalTaskService.ChangeTenantTaskType(model.Id, model.Value);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeTenantTaskUrgency")]
        public async Task ChangeTenantTaskUrgency(IdValueModel<Guid, bool> model)
        {
            await operationalTaskService.ChangeTenantTaskUrgency(model.Id, model.Value);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeTaskDate")]
        public async Task ChangeTenantTaskDate(OperationalTaskChangeDateModel model)
        {
            await operationalTaskService.ChangeTaskDate(model.JobId, model.DayAssignId, model.Date);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeTaskTime")]
        public async Task ChangeTenantTaskTime(IdValueModel<Guid, DateTime> model)
        {
            await operationalTaskService.ChangeTaskTime(model.Id, model.Value);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeResidentName")]
        public async Task ChangeResidentName(IdValueModel<Guid, string> model)
        {
            await operationalTaskService.ChangeResidentName(model);
        }

        [HttpPost, Route("changeResidentPhone")]
        public async Task ChangeResidentPhone(IdValueModel<Guid, string> model)
        {
            await operationalTaskService.ChangeResidentPhone(model);
        }

        [HttpPost, Route("changeTenantTaskComment")]
        public async Task ChangeTenantTaskComment(IdValueModel<Guid, string> model)
        {
            await operationalTaskService.ChangeTenantTaskComment(model);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeCategory")]
        public async Task ChangeCategory(IdValueModel<string, Guid> model)
        {
            await operationalTaskService.ChangeCategory(model);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeEstimate")]
        public async Task ChangeEstimate(IdValueModel<Guid, int> model)
        {
            await operationalTaskService.ChangeEstimate(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeTitle")]
        public async Task ChangeTitle(IdValueModel<string, string> model)
        {
            await operationalTaskService.ChangeTitle(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshCompletedTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeDescription")]
        public async Task ChangeDescription(IdValueModel<string, string> model)
        {
            await operationalTaskService.ChangeDescription(model);
        }

        [HttpPost, Route("assignEmployees")]
        public async Task AssignEmployees(MemberAssignModel model)
        {
            await operationalTaskService.AssignEmployees(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("unassignEmployees")]
        public async Task UnassignEmployees(MemberAssignModel model)
        {
            await operationalTaskService.UnassignEmployees(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpGet, Route("getTaskCreationInfo")]
        public async Task<ITaskCreationInfo> GetTaskCreationInfo(string jobId)
        {
            var result = await operationalTaskService.GetTaskCreationInfo(jobId);
            return result;
        }

        [HttpGet, Route("getUploads")]
        public IEnumerable<UploadFileViewModel> GetUploads(Guid jobAssignId)
        {
            IEnumerable<UploadFileModel> uploads = operationalTaskService.GetUploads(jobAssignId);
            var result = Mapper.Map<IEnumerable<UploadFileViewModel>>(uploads);

            return result;
        }

        private MemberViewModel GetUploaderModel(Guid uploaderId)
        {
            IMemberModel member = memberService.GetById(uploaderId);
            return new MemberViewModel
            {
                Avatar = member.Avatar,
                Name = member.Name
            };
        }
    }
}