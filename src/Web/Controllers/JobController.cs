using AutoMapper;
using FileStorage.Contract.Enums;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using Microsoft.AspNet.SignalR;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using StatusCore.Contract.Enums;
using Web.Core.Hubs;
using Web.Models;
using Web.Models.Task;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Commands.JobCommands;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [RoutePrefix("api/job")]
    public class JobController : ApiController
    {
        private readonly IMessageBus messageBus;
        private readonly ITaskIdGenerator taskIdGenerator;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly IJobService jobService;
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IMemberService memberService;
        private readonly IDayAssignService dayAssignService;
        private readonly IJobStatusService jobStatusService;
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IWeekPlanService weekPlanService;
        private readonly IGuideCommentService guideCommentService;
        private IHubContext janitorHubs;
        private IHubContext managementHubs;

        public JobController(IMessageBus messageBus,
            ITaskIdGenerator taskIdGenerator,
            IDayAssignProvider dayAssignProvider,
            IJobService jobService,
            IJobAssignProvider jobAssignProvider,
            IMemberService memberService,
            IDayAssignService dayAssignService,
            IJobStatusService jobStatusService,
            IJobStatusLogService jobStatusLogService,
            IWeekPlanService weekPlanService,
            IGuideCommentService guideCommentService)
        {
            this.messageBus = messageBus;
            this.taskIdGenerator = taskIdGenerator;
            this.dayAssignProvider = dayAssignProvider;
            this.jobService = jobService;
            this.jobAssignProvider = jobAssignProvider;
            this.memberService = memberService;
            this.dayAssignService = dayAssignService;
            this.jobStatusService = jobStatusService;
            this.jobStatusLogService = jobStatusLogService;
            this.weekPlanService = weekPlanService;
            this.guideCommentService = guideCommentService;
            this.janitorHubs = GlobalHost.ConnectionManager.GetHubContext<JanitorHub>();
            this.managementHubs = GlobalHost.ConnectionManager.GetHubContext<ManagementHub>();
        }

        [HttpGet, Route("getJob")]
        public JobViewModel GetJob(string id, Guid? housingDepartmentId)
        {
            IJob job = jobService.GetJobById(id).Result;
            List<JobAssign> jobAssignList = jobAssignProvider.GetAllByJobId(id);
            bool isAllChildGroupedTaskHided = housingDepartmentId.HasValue && jobService.CheckIsAllChildGroupedTaskHided(job);
            bool isPossibleToHideChildTask = housingDepartmentId.HasValue && job.ParentId != null && jobService.IsPossibleToHideChildTask(job.ParentId, housingDepartmentId.Value);

            var result = new JobViewModel
            {
                Job = job,
                AssignedHousingDepartmentsIdList = jobAssignList.SelectMany(x => x.HousingDepartmentIdList),
                IsAllAssignedHousingDepartmentAreGrouped = jobService.IsRelationListMatchJobAssignList(job, jobAssignList),
                IsAllChildGroupedTaskHided = isAllChildGroupedTaskHided,
                IsPossibleToHideChildTask = isPossibleToHideChildTask
            };

            return result;
        }

        [HttpGet, Route("getJobAssigns")]
        public async Task<FormattedJobAssignViewModel> GetJobAssigns(string jobId, UploadedContentEnum? contentType)
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            IJob job = await jobService.GetJobById(jobId);
            var result = new FormattedJobAssignViewModel();

            if (job.RelationGroupList.Any() && job.ParentId != null)
            {
                Guid housingDepartmentId = job.RelationGroupList.First().HousingDepartmentId;
                IEnumerable<Guid> relationGroupIdList = job.RelationGroupList.Select(x => x.RelationGroupId).ToList();
                JobAssign globalGroupedJobAssign = jobService.GetGlobalJobAssignForGroupedTask(job);
                List<JobAssign> jobAssigns = jobService.GetJobAssignList(relationGroupIdList);
                result.GlobalAssign = globalGroupedJobAssign.Map<JobAssignViewModel>();
                result.Assigns = jobAssigns.Where(i => !i.IsGlobal && i.HousingDepartmentIdList.Contains(housingDepartmentId)).Select(i => i.Map<JobAssignViewModel>()).ToList();
                result.AddressList = jobService.GetRelatedAddressListForHousingDepartment(relationGroupIdList, housingDepartmentId, isParent: false);
                result.IsGroupedJob = true;
                result.IsChildJob = job.ParentId != null;
            }
            else
            {
                IFormattedJobAssign jobAssign = jobService.GetAllByJobIdFormatted(jobId, currentUser.IsAdmin() ? null : currentUser.ActiveManagementDepartmentId);
                result = Mapper.Map<FormattedJobAssignViewModel>(jobAssign);
            }

            result.CurrentUser = currentUser;
            result.GlobalAssign.UploadList = GetUploadListModel(result.GlobalAssign.UploadList, contentType);
            
            foreach (var assign in result.Assigns)
            {
                assign.UploadList = GetUploadListModel(assign.UploadList, contentType);
            }

            return result;
        }

        [HttpGet, Route("getJobDepartments")]
        public JobDepartmentsViewModel GetJobDepartments(string jobId)
        {
            IEnumerable<IHousingDepartmentModel> departments = jobService.GetAssignedDepartments(jobId);
            IEnumerable<IHousingDepartmentModel> groupedDepartments = jobService.GetGroupedAssignedDepartments(jobId);
            IJob job = jobService.GetJobById(jobId).Result;

            List<HousingDepartmentViewModel> assignedDepartments = departments.Map<IEnumerable<HousingDepartmentViewModel>>().ToList();

            foreach (var assignedDepartment in assignedDepartments)
            {
                if (job.ParentId == null && job.RelationGroupList.Any(x => x.HousingDepartmentId == assignedDepartment.Id))
                {
                    IEnumerable<IJob> childGroupedJobs = jobService.GetChildJobsForHousingDepartment(job.Id, assignedDepartment.Id).ToList();
                    assignedDepartment.IsDisabled = childGroupedJobs.Any(x => !x.IsHidden);
                }
            }

            return new JobDepartmentsViewModel
            {
                AssignedDepartments = assignedDepartments,
                GroupedDepartments = groupedDepartments.Map<IEnumerable<HousingDepartmentViewModel>>()
            };
        }

        [HttpGet, Route("getOpenedJobsHeaderList")]
        public IEnumerable<JobHeaderViewModel> GetOpenedJobsHeaderList()
        {
            return Mapper.Map<IEnumerable<JobHeaderViewModel>>(jobService.GetOpenedJobsHeaderList());
        }

        [HttpGet, Route("getMyJobsHeaderList")]
        public IEnumerable<JobHeaderViewModel> GetMyJobsHeaderList()
        {
            return Mapper.Map<IEnumerable<JobHeaderViewModel>>(jobService.GetMyJobsHeaderList());
        }

        [HttpGet, Route("getJobGuideComments")]
        public IEnumerable<IGuideCommentModel> GetJobGuideComments(string jobId)
        {
            return guideCommentService.GetGuideJobCommentsByJobId(jobId);
        }

        [HttpPost, Route("saveOrUpdateGuideComment")]
        public void SaveOrUpdateGuideComment(GuidCommentViewModel model)
        {
            IGuideCommentModel guideCommentModel = Mapper.Map<IGuideCommentModel>(model);
            guideCommentService.SaveOrUpdateGuideComment(guideCommentModel);
        }

        [HttpGet, Route("removeGuideComment")]
        public void RemoveGuideComment(Guid commentId)
        {
            guideCommentService.RemoveGuideComment(commentId);
        }

        [HttpGet, Route("getJobCounters")]
        public JobCounterViewModel GetJobCounters()
        {
            return Mapper.Map<JobCounterViewModel>(jobService.GetJobCounters());
        }

        [HttpGet, Route("getClosedJobsHeaderList")]
        public IEnumerable<JobHeaderViewModel> GetClosedJobsHeaderList()
        {
            return Mapper.Map<IEnumerable<JobHeaderViewModel>>(jobService.GetCompletedJobsHeaderList());
        }

        [HttpGet, Route("getJobDetailsByDayAssignId")]
        public JobDetailsViewModel GetJobDetailsByDayAssignId(Guid dayAssignId)
        {
            JobDetailsViewModel jobDetails = Mapper.Map<JobDetailsViewModel>(jobService.GetJobDetails(dayAssignId));
            jobDetails.GlobalUploadList = GetUploadListModel(jobDetails.GlobalUploadList, null);
            jobDetails.LocalUploadList = GetUploadListModel(jobDetails.LocalUploadList, null);
            jobDetails.JanitorUploadImageList = GetJanitorUploads(jobDetails.JanitorUploadImageList, jobDetails.DayAssignId);
            jobDetails.JanitorUploadImageList = GetUploadListModel(jobDetails.JanitorUploadImageList, UploadedContentEnum.Image);
            jobDetails.JanitorUploadVideoList = GetJanitorUploads(jobDetails.JanitorUploadVideoList, jobDetails.DayAssignId);
            jobDetails.JanitorUploadVideoList = GetUploadListModel(jobDetails.JanitorUploadVideoList, UploadedContentEnum.Video);
            FillSpentTime(jobDetails);
            return jobDetails;
        }

        [HttpGet, Route("getTenantJobsRelatedByAddress")]
        public IEnumerable<IJobRelatedByAddressModel> GetTenantJobsRelatedByAddress(string jobId)
        {
            return jobService.GetTenantJobsRelatedByAddress(jobId);
        }

        [HttpGet, Route("startJob")]
        public async Task StartJob(Guid dayAssignId)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            await jobStatusService.InProgress(dayAssignId, dayAssign.StatusId);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("unassignJob")]
        public async Task UnassignJob(ChangeJobStatusModel model)
        {
            await dayAssignService.UnassignJob(model.DayAssignId, model.ChangeStatusComment, model.Members.ToList<IMemberSpentTimeModel>(), model.NewJobStatus, model.SelectedCancellingId, model.UploadedFileIds);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpGet, Route("pauseJob")]
        public async Task PauseJob(Guid dayAssignId)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            await jobStatusService.Paused(dayAssignId, dayAssign.StatusId);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("completeJob")]
        public async Task<IMoveToStatusResultModel> CompleteJob(ChangeJobStatusModel model)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(model.DayAssignId);
            var result = await jobStatusService.Completed(model.DayAssignId, dayAssign.StatusId, model.ChangeStatusComment, model.Members.ToList<IMemberSpentTimeModel>(), model.UploadedFileIds);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshCompletedTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpGet, Route("getUploadedFile")]
        public JanitorUploadsModel GetUploadedFile(Guid dayAssignId)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            foreach (var upload in dayAssign.UploadList)
            {
                upload.Path = weekPlanService.GetUploadedFileLink(upload, dayAssign.Id);
            }

            return new JanitorUploadsModel
            {
                Images = dayAssign.UploadList.Where(x => x.ContentType == UploadedContentEnum.Image),
                Videos = dayAssign.UploadList.Where(x => x.ContentType == UploadedContentEnum.Video)
            };
        }

        [HttpGet, Route("assignJob")]
        public async Task AssignJob(Guid dayAssignId)
        {
            await dayAssignService.AssignJob(dayAssignId);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpGet, Route("reopenJob")]
        public async Task ReopenJob(Guid dayAssignId)
        {
            await dayAssignService.ReopenJob(dayAssignId);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshCompletedTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("create")]
        public async Task<string> Create(NewFacilityTaskModel model)
        {
            var currentUser = memberService.GetCurrentUser();
            var jobId = await taskIdGenerator.Facility();
            await messageBus.Publish(new CreateJobCommand(jobId, model.CategoryId, model.Title, JobTypeEnum.Facility, currentUser.MemberId, currentUser.CurrentRole, null, null, null));

            var jobAssignId = Guid.NewGuid();
            int globalTillYear = 0;
            await messageBus.Publish(new CreateJobAssignCommand(jobAssignId, jobId, currentUser.CurrentRole, globalTillYear));

            return jobId;
        }

        [HttpPost, Route("assign")]
        public async Task Assign(FacilityTaskAssignDepartmentModel model)
        {
            await AssignDepartments(model.JobId, model.AssignedDepartmentIds);
            await UnassignDepartments(model.JobId, model.UnassignedDepartmentIds);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("saveCategory")]
        public async Task SaveCategory(IdValueModel<string, Guid> model)
        {
            await jobService.SaveCategory(model);
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("saveDescription")]
        public async Task SaveDescription(ChangeJobAssignDescriptionModel model)
        {
            JobAssign jobAssign = await jobService.CreateOrGetJobAssign(model.AssignId, model.JobId, model.DepartmentId);
            await messageBus.Publish(new ChangeJobAssignDescriptionCommand(jobAssign.Id, model.Description));
        }

        [HttpPost, Route("saveTillYear")]
        public async Task<JobAssignViewModel> SaveTillYear(ChangeJobAssignTillYearModel model)
        {
            JobAssign jobAssign = await jobService.CreateOrGetJobAssign(model.AssignId, model.JobId, model.DepartmentId);
            bool isLocalIntervalChanged = !jobAssign.IsGlobal;
            await messageBus.Publish(new ChangeJobAssignTillYearCommand(jobAssign.Id, model.TillYear, model.ChangedByRole, isLocalIntervalChanged));
            jobAssign.TillYear = model.TillYear;

            if (jobAssign.IsGlobal)
            {
                await jobService.CopyGlobalJobAssignToLocalIfLocalIntervalWasntChanged(jobAssign.Id, model.JobId, model.DepartmentId);
                await jobService.CopyParentGlobalJobAssignToChildren(jobAssign.Id, model.JobId);
            }

            return jobAssign.Map<JobAssignViewModel>();
        }

        [HttpPost, Route("saveTitle")]
        public async Task SaveTitle(IdValueModel<string, string> model)
        {
            await jobService.SaveTitle(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshCompletedTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("saveWeekList")]
        public async Task<JobAssignViewModel> SaveWeekList(ChangeJobAssignWeekListModel model)
        {
            JobAssign jobAssign = await jobService.CreateOrGetJobAssign(model.AssignId, model.JobId, model.DepartmentId);

            if (!jobAssign.IsGlobal)
            {
                foreach (var week in model.WeekList)
                {
                    week.IsDisabled = week.ChangedBy == WeekChangedBy.Administrator;
                }
            }

            bool isLocalIntervalChanged = !jobAssign.IsGlobal;
            await messageBus.Publish(new ChangeJobAssignWeeksCommand(jobAssign.Id, model.WeekList, model.ChangedByRole, isLocalIntervalChanged));
            jobAssign.WeekList = model.WeekList;
            if (jobAssign.IsGlobal)
            {
                await jobService.CopyGlobalJobAssignToLocalIfLocalIntervalWasntChanged(jobAssign.Id, model.JobId, model.DepartmentId);
                await jobService.CopyParentGlobalJobAssignToChildren(jobAssign.Id, model.JobId);
            }

            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();

            return jobAssign.Map<JobAssignViewModel>();
        }

        [HttpPost, Route("saveJobShedule")]
        public async Task<JobAssignViewModel> SaveJobShedule(ChangeJobSheduleModel model)
        {
            JobAssign jobAssign = await jobService.CreateOrGetJobAssign(model.AssignId, model.JobId, model.DepartmentId);
            FillCorrectDayPerWeekModel(model.DayPerWeekList, jobAssign);
            bool isLocalIntervalChanged = !jobAssign.IsGlobal;
            await messageBus.Publish(new ChangeJobAssignSheduleCommand(jobAssign.Id, model.DayPerWeekList, model.RepeatsPerWeek, model.ChangedByRole, isLocalIntervalChanged));
            jobAssign.DayPerWeekList = model.DayPerWeekList;
            jobAssign.RepeatsPerWeek = model.RepeatsPerWeek;

            if (jobAssign.IsGlobal)
            {
                await jobService.CopyGlobalJobAssignToLocalIfLocalIntervalWasntChanged(jobAssign.Id, model.JobId, model.DepartmentId);
                await jobService.CopyParentGlobalJobAssignToChildren(jobAssign.Id, model.JobId);
            }

            return jobAssign.Map<JobAssignViewModel>();
        }

        [HttpPost, Route("lockInterval")]
        public async Task<JobAssignViewModel> LockInterval(ChangeJobAssignLockIntervalModel model)
        {
            JobAssign jobAssign = await jobService.CreateOrGetJobAssign(model.AssignId, model.JobId, model.DepartmentId);
            await messageBus.Publish(new ChangeLockIntervalValueCommand(jobAssign.Id, model.IsLocked));
            jobAssign.IsLocked = model.IsLocked;
            return jobAssign.Map<JobAssignViewModel>();
        }

        [HttpPost, Route("hide")]
        public async Task HideTask(IdValueModel<string, bool> model)
        {
            await messageBus.Publish(new ChangeJobVisibilityCommand(model.Id, model.Value));
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
        }

        [HttpPost, Route("changeJobAssignEstimate")]
        public void ChangeJobAssignEstimate(Responsible model)
        {
            jobService.ChangeJobAssignEstimate(model.JobId, model.EstimateInMinutes, model.HousingDepartmentId);
        }

        [HttpPost, Route("changeJobAssignTeam")]
        public void ChangeJobAssignTeam(Responsible model)
        {
            jobService.ChangeAssignTeam(model.JobId, model.HousingDepartmentId, model.UserIdList, model.IsAssignedToAllUsers, model.GroupId, model.TeamLeadId);
        }

        [HttpGet, Route("getJobAssignTeam")]
        public Responsible GetJobAssignTeam(string jobId, Guid housingDepartmentId)
        {
            return jobService.GetAssignTeam(jobId, housingDepartmentId);
        }

        [HttpGet, Route("getJobAssignEstimate")]
        public int GetJobAssignEstimate(string jobId, Guid housingDepartmentId)
        {
            return jobService.GetJobAssignEstimate(jobId, housingDepartmentId);
        }

        [HttpPost, Route("changeDayAssignEstimate")]
        public async Task<Guid> ChangeDayAssignEstimate(NewDayAssignViewModel model)
        {
            var dayAssign = model.DayAssignId.HasValue ? dayAssignProvider.Get(model.DayAssignId.Value) : null;
            if (dayAssign == null)
            {
                var createResult = await dayAssignService.CreateDayAssign(model);
                managementHubs.Clients.All.refreshWeekPlanGridTasks();
                managementHubs.Clients.All.refreshWeekPlanListTasks();
                return createResult;
            }
            else
            {
                model.Id = model.DayAssignId.Value;
                await messageBus.Publish(model.Map<ChangeDayAssignEstimatedMinutesCommand>());
                var changeResult = model.DayAssignId.Value;
                managementHubs.Clients.All.refreshWeekPlanGridTasks();
                managementHubs.Clients.All.refreshWeekPlanListTasks();
                return changeResult;
            }
        }

        [HttpPost, Route("changeDayAssignDate")]
        public async Task<Guid> ChangeDayAssignDate(NewDayAssignViewModel model)
        {
            var job = await jobService.GetJobById(model.JobId);

            if (job.JobTypeId != JobTypeEnum.Facility)
            {
                var dayPerWeekList = new List<DayPerWeekModel>();
                if (model.WeekDay.HasValue)
                {
                    var dayPerWeekId = Guid.NewGuid();
                    dayPerWeekList.Add(new DayPerWeekModel { Id = dayPerWeekId, WeekDay = model.WeekDay.Value });
                    model.DayPerWeekId = dayPerWeekId;
                }
                await messageBus.Publish(new SaveDaysPerWeekCommand(model.JobAssignId, dayPerWeekList));
            }

            var result = await dayAssignService.ChangeDayAssignDate(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }


        [HttpPost, Route("assignsMembersGroup")]
        public async Task<IChangeStatusModel> AssignsMembersGroup(NewDayAssignViewModel model)
        {
            var result = await dayAssignService.AssignsMembersGroup(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpPost, Route("getWeekTask")]
        public async Task<JobViewModel> GetWeekTask(JobFilterViewModel model)
        {
            IJob task = await jobService.GetJobById(model.JobId);

            var jobAssignId = Guid.Empty;
            var existingAssignId = model.DayAssignId.HasValue ? model.DayAssignId.Value : Guid.Empty;
            var dayAssign = dayAssignProvider.Get(existingAssignId);
            if (dayAssign != null)
            {
                JobAssign jobAssign = jobAssignProvider.GetById(dayAssign.JobAssignId);
                task.Assigns.Add(jobAssign);
                jobAssignId = dayAssign.JobAssignId;
                task.DayAssigns.Add(dayAssign);
            }
            else
            {
                jobAssignId = jobAssignProvider.GetAssignByJobIdAndDepartmentId(model.JobId, model.HousingDepartmentId).Id;
                //Create new dayAssign and set status "Assigned"
                var newDayAssignModel = new NewDayAssignViewModel
                {
                    JobId = model.JobId,
                    DepartmentId = model.HousingDepartmentId,
                    JobAssignId = jobAssignId,
                    CurrentWeekDay = model.CurrentWeekDay,
                    WeekNumber = model.WeekNumber,
                    Date = model.Date
                };

                var createResult = await dayAssignService.CreateDayAssignWithEstimate(newDayAssignModel);

                if (createResult != Guid.Empty)
                {
                    var newDayAssign = dayAssignProvider.Get(createResult);
                    await jobStatusService.Assigned(createResult, newDayAssign.StatusId);
                    task.DayAssigns.Add(newDayAssign);
                }
            }

            return new JobViewModel { Job = task, JobAssignId = jobAssignId };
        }

        [HttpPost, Route("dayAssignCancel")]
        public async Task<Guid> DayAssignCancel(NewDayAssignViewModel model)
        {
            var result = await dayAssignService.CancelJob(model);
            janitorHubs.Clients.All.refreshOpenTasks();
            janitorHubs.Clients.All.refreshJanitorTasks();
            managementHubs.Clients.All.refreshWeekPlanGridTasks();
            managementHubs.Clients.All.refreshWeekPlanListTasks();
            return result;
        }

        [HttpGet, Route("getCurrentHousingDepartmentAddresses")]
        public IEnumerable<string> GetCurrentHousingDepartmentAddresses(Guid? housingDepartmentId)
        {
            var result = jobService.GetHousingDepartmentAddressesOrAllAddressesForUserManagementDepartment(housingDepartmentId);
            return result;
        }

        [HttpGet, Route("getLocationModel")]
        public async Task<JobLocationViewModel> GetLocationModel(string jobId, Guid? housingDepartmentId) 
        {
            var job = await jobService.GetJobById(jobId);
            bool isGroupedJob = job.RelationGroupList.Any() && job.ParentId != null;
            var model = new JobLocationViewModel { IsGroupedJob = isGroupedJob };

            if (isGroupedJob)
            {
                bool isParent = job.ParentId == null;
                RelationGroupModel relationGroupModel = job.RelationGroupList.FirstOrDefault(x => x.HousingDepartmentId == housingDepartmentId);
                model.GroupedJobHousingDepartmentId = relationGroupModel?.HousingDepartmentId;
                IEnumerable<Guid> relationGroupIdList = job.RelationGroupList.Select(x => x.RelationGroupId);
                // housingDepartmentId can be null only for not grouped task, if admin during task creation doesn't choose any HD in department picker
                var groupedTasks = jobService.GetRelatedAddressListForHousingDepartment(relationGroupIdList, housingDepartmentId.Value, isParent);  
                model.GroupedTasks = groupedTasks.Select(i => new IdValueModel<string, string> { Id = i.Id, Value = i.Value });
                foreach (var task in groupedTasks)
                {
                    if (model.GroupedJobHousingDepartmentId.HasValue)
                    {
                        var addresses = new Dictionary<Guid, string>();
                        addresses.Add(model.GroupedJobHousingDepartmentId.Value, task.Value);
                        model.AddressList.Add(task.Id, addresses);
                    }
                }
            }
            else
            {
                var departments = jobService.GetAssignedDepartments(jobId);
                model.Departments = departments.Select(i => new IdValueModel<Guid, string> { Id = i.Id, Value = i.DisplayName });
                model.AddressList.Add(jobId, job.AddressList.ToDictionary(k => k.HousingDepartmentId, v => v.Address));
            }

            return model;
        }

        [HttpPost, Route("saveAddress")]
        public async Task SaveAddress(string jobId, IdValueModel<Guid, string> model)
        {
            await messageBus.Publish(new ChangeJobAddressCommand(jobId, model.Id, model.Value));
        }

        [HttpGet, Route("isAllowedTaskGrouping")]
        public bool IsAllowedTaskGrouping(string jobId)
        {
            return jobService.IsAllowedTaskGrouping(jobId);
        }

        [HttpGet, Route("addTaskToRelationGroup")]
        public Task<string> AddTaskToRelationGroup(string jobId, Guid housingDepartmentId)
        {
            return jobService.AddTaskToRelationGroup(jobId, housingDepartmentId);
        }

        [HttpGet, Route("createTaskRelationGroup")]
        public Task<string> CreateTaskRelationGroup(string jobId, Guid housingDepartmentId)
        {
            return jobService.CreateTaskRelationGroup(jobId, housingDepartmentId);
        }

        [HttpGet, Route("isGroupedTask")]
        public bool IsGroupedTask(string jobId)
        {
            return jobService.IsGroupedTask(jobId);
        }

        [HttpGet, Route("isChildGroupedTask")]
        public bool IsChildGroupedTask(string jobId)
        {
            return jobService.IsChildGroupedTask(jobId);
        }

        [HttpGet, Route("getHousingDepartmentsForGroupingTasks")]
        public IEnumerable<HousingDepartmentViewModel> GetHousingDepartmentsForGroupingTasks(string jobId)
        {
            IEnumerable<IHousingDepartmentModel> departments = jobService.GetHousingDepartmentsForGroupingTasks(jobId);
            IEnumerable<IHousingDepartmentModel> groupedDepartments = jobService.GetGroupedAssignedDepartments(jobId);
            IEnumerable<HousingDepartmentViewModel> housingDepartmentsView = departments.Map<IEnumerable<HousingDepartmentViewModel>>();
            foreach (var housingDepartment in housingDepartmentsView)
            {
                if (groupedDepartments.Any(x => x.Id == housingDepartment.Id))
                {
                    housingDepartment.IsDisabled = true;
                }
            }

            return housingDepartmentsView;
        }

        [HttpPost, Route("getApproximateSpentTime")]
        public List<IdValueModel<Guid, IApproximateSpentTimeModel>> GetApproximateSpentTime(IdValueModel<Guid, DateTime> model)
        {
            IDictionary<Guid, IApproximateSpentTimeModel> membersApproximateSpentTimeList = jobService.GetMembersApproximateSpentTimeList(model.Id, model.Value);

            var result = membersApproximateSpentTimeList.Select(spentTimeModel =>
                new IdValueModel<Guid, IApproximateSpentTimeModel>
                {
                    Id = spentTimeModel.Key,
                    Value = spentTimeModel.Value
                });

            return result.ToList();
        }

        private async Task AssignDepartments(string jobId, IEnumerable<Guid> departmentIds)
        {
            var assigns = jobAssignProvider.GetAllByJobId(jobId, true).OrderBy(x => x.HousingDepartmentIdList.Count);
            var globalAssign = assigns.First(x => x.IsGlobal);
            foreach (var departmentId in departmentIds)
            {
                var jobAssign = assigns.FirstOrDefault(x => x.HousingDepartmentIdList.Contains(departmentId));

                if (jobAssign != null)
                {
                    await messageBus.Publish(new AssignJobCommand(jobAssign.Id, departmentId));
                }

                await messageBus.Publish(new AssignJobCommand(globalAssign.Id, departmentId));
            }
        }

        private async Task UnassignDepartments(string jobId, IEnumerable<Guid> departmentIds)
        {
            var childIdList = jobService.GetAllIdsByParentId(jobId).ToList();
            var departmentIdList = departmentIds.AsList();
            var childAssigns = jobAssignProvider.GetAllJobAssignByFilters(childIdList, departmentIdList);
            if (childAssigns.HasValue())
            {
                foreach (var childAssign in childAssigns)
                {
                    await messageBus.Publish(new UnassignJobCommand(childAssign.Id, departmentIdList));
                }
            }

            var assigns = jobAssignProvider.GetAllByJobId(jobId).Where(x => x.HousingDepartmentIdList.Any(departmentIdList.Contains));
            foreach (var assign in assigns)
            {
                await messageBus.Publish(new UnassignJobCommand(assign.Id, departmentIdList));
            }

        }

        private List<UploadFileViewModel> GetJanitorUploads(IEnumerable<UploadFileViewModel> uploadList, Guid dayAssignId)
        {
            var result = new List<UploadFileModel>();
            foreach (var upload in uploadList)
            {
                var mappedUpload = upload.Map<UploadFileModel>();
                mappedUpload.Path = weekPlanService.GetUploadedFileLink(mappedUpload, dayAssignId);
                result.Add(mappedUpload);
            }

            return result.Map<List<UploadFileViewModel>>();
        }

        private List<UploadFileViewModel> GetUploadListModel(List<UploadFileViewModel> uploadList, UploadedContentEnum? contentType)
        {
            var result = new List<UploadFileViewModel>();
            if (!uploadList.HasValue())
            {
                return result;
            }

            foreach (var upload in uploadList)
            {
                if (contentType.HasValue && !IsValidContentType(upload.ContentType, contentType.Value))
                {
                    continue;
                }

                upload.Uploader = GetUploaderModel(upload.UploaderId);
                result.Add(upload);
            }

            return result;
        }

        private void FillSpentTime(JobDetailsViewModel jobDetails)
        {
            List<Guid> assignedMemberIds = jobDetails.Members.Select(m => m.MemberId).ToList();
            List<ITimeLogModel> assignedMembersSpentTimeList = jobStatusLogService.GetUserSpentTime(jobDetails.DayAssignId, assignedMemberIds);

            foreach (var member in jobDetails.Members)
            {
                ITimeLogModel spentTimeModel = assignedMembersSpentTimeList.FirstOrDefault(l => l.MemberId == member.MemberId);
                if (spentTimeModel != null)
                {
                    member.SpentHours = (int)spentTimeModel.SpentTime.TotalHours;
                    member.SpentMinutes = spentTimeModel.SpentTime.Minutes;
                    member.HasSpentTime = member.SpentHours > 0 || member.SpentMinutes > 0;
                }
            }
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

        private void FillCorrectDayPerWeekModel(IEnumerable<DayPerWeekModel> model, JobAssign jobAssign)
        {
            foreach (var dayPerWeek in model)
            {
                DayPerWeekModel existingDayPerWeek =
                    jobAssign.DayPerWeekList.FirstOrDefault(dpw => dpw.WeekDay == dayPerWeek.WeekDay);
                dayPerWeek.Id = existingDayPerWeek?.Id ?? Guid.NewGuid();

            }
        }

        private bool IsValidContentType(UploadedContentEnum fileContent, UploadedContentEnum? type)
        {
            return !type.HasValue ||
                   fileContent == type ||
                   (type == UploadedContentEnum.Media && fileContent.In(UploadedContentEnum.Image, UploadedContentEnum.Video));
        }
    }
}