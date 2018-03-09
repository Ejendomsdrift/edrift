using AbsenceTemplatesCore.Contract.Interfaces;
using AutoMapper;
using FileStorage.Contract.Commands;
using FileStorage.Contract.Enums;
using GroupsContract.Interfaces;
using GroupsContract.Models;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using Infrastructure.Messaging;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Commands.JobCommands;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class JobService : IJobService
    {
        private readonly List<JobStatus> NotAllowedJobStatuses = new List<JobStatus> { JobStatus.Assigned, JobStatus.Pending, JobStatus.Opened, JobStatus.Rejected, JobStatus.Canceled, JobStatus.Completed, JobStatus.Expired };
        private readonly List<JobStatus> OpenedJobStatuses = new List<JobStatus> { JobStatus.Opened, JobStatus.Rejected };
        private readonly List<JobStatus> CompletedJobStatuses = new List<JobStatus> { JobStatus.Completed };
        private readonly List<JobStatus> MyTaskJobStatuses = new List<JobStatus> { JobStatus.InProgress, JobStatus.Paused, JobStatus.Assigned };

        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IJobProvider jobProvider;
        private readonly IManagementDepartmentService managementService;
        private readonly IMemberService memberService;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly IGroupService groupService;
        private readonly IAppSettingHelper appSettingHelper;
        private readonly IPathHelper pathHelper;
        private readonly IDayAssignService dayAssignService;
        private readonly IMessageBus messageBus;
        private readonly ITaskIdGenerator taskIdGenerator;
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IEmployeeAbsenceInfoService absenceInfoService;

        public JobService(IJobAssignProvider jobAssignProvider,
                          IJobProvider jobProvider,
                          IManagementDepartmentService managementService,
                          IMemberService memberService,
                          IDayAssignProvider dayAssignProvider,
                          IGroupService groupService,
                          IAppSettingHelper appSettingHelper,
                          IPathHelper pathHelper,
                          IDayAssignService dayAssignService,
                          IMessageBus messageBus,
                          ITaskIdGenerator taskIdGenerator,
                          IJobStatusLogService jobStatusLogService,
                          IEmployeeAbsenceInfoService absenceInfoService)

        {
            this.jobAssignProvider = jobAssignProvider;
            this.jobProvider = jobProvider;
            this.managementService = managementService;
            this.memberService = memberService;
            this.dayAssignProvider = dayAssignProvider;
            this.groupService = groupService;
            this.appSettingHelper = appSettingHelper;
            this.pathHelper = pathHelper;
            this.dayAssignService = dayAssignService;
            this.messageBus = messageBus;
            this.taskIdGenerator = taskIdGenerator;
            this.jobStatusLogService = jobStatusLogService;
            this.absenceInfoService = absenceInfoService;
        }

        public void ChangeJobAssignEstimate(string jobId, int estimateInMinutes, Guid housingDepartmentId)
        {
            List<Guid> jobAssignIdList = jobAssignProvider.ChangeEstimate(jobId, estimateInMinutes, housingDepartmentId);
            
            dayAssignService.UpdateDayAssignEstimate(jobAssignIdList, jobId, housingDepartmentId, estimateInMinutes);
        }

        public void ChangeAssignTeam(string jobId, Guid housingDepartmentId, List<Guid> userIdList, bool isAssignedToAllUsers, Guid? groupId, Guid teamLeadId)
        {
            List<Guid> jobAssignIdList = jobAssignProvider.ChangeAssignTeam(jobId, housingDepartmentId, userIdList, isAssignedToAllUsers, groupId, teamLeadId);

            dayAssignService.UpdateDayAssignTeam(jobAssignIdList, jobId, housingDepartmentId, isAssignedToAllUsers, groupId, teamLeadId, userIdList);
        }

        public Responsible GetAssignTeam(string jobId, Guid housingDepartmentId)
        {
            return jobAssignProvider.GetAssignTeam(jobId, housingDepartmentId);
        }

        public int GetJobAssignEstimate(string jobId, Guid housingDepartmentId)
        {
            return jobAssignProvider.GetEstimate(jobId, housingDepartmentId);
        }

        public int GetPreviousNotEmptyWeekNumber(IWeekPlanFilterModel filter)
        {
            int result = dayAssignService.GetPreviousNotEmptyWeekNumber(filter);
            return result;
        }

        public IEnumerable<IJob> GetByIds(IEnumerable<string> ids)
        {
            var jobs = jobProvider.Query.Where(j => ids.Contains(j.Id));
            return jobs;
        }

        public IEnumerable<string> GetAllIdsByParentId(string parentJobId)
        {
            var jobIds = jobProvider.Query.Where(j => j.ParentId == parentJobId).Select(j => j.Id);

            return jobIds;
        }

        public void UpdateAddresses(string jobId, List<JobAddress> addresses)
        {
            jobProvider.UpdateSingleProperty(jobId, x => x.AddressList, addresses);
        }

        public IFormattedJobAssign GetAllByJobIdFormatted(string jobId, Guid? managementDepartmentId = null)
        {
            var result = new FormattedJobAssign();
            var departments = Enumerable.Empty<IHousingDepartmentModel>();

            var assigns = GetAllByJobId(jobId, managementDepartmentId);

            if (managementDepartmentId.HasValue)
            {
                departments = managementService.GetHousingDepartments(managementDepartmentId.Value, assigns.SelectMany(x => x.HousingDepartmentIdList));
            }
            else
            {
                departments = managementService.GetHousingDepartments(assigns.SelectMany(x => x.HousingDepartmentIdList));
            }

            FillCorrectPathForJobAssignsUploads(assigns);
            result.Assigns = assigns.Where(a => !a.IsGlobal);
            result.GlobalAssign = assigns.FirstOrDefault(a => a.IsGlobal);
            result.AssignedDepartments = departments;
            return result;
        }

        public async Task CopyGlobalJobAssignToLocalIfLocalIntervalWasntChanged(Guid globalJobAssignId, string jobId, Guid housingDepartmentId)
        {
            IJob job = await GetJobById(jobId);
            JobAssign globalJobAssign = GetJobAssignById(globalJobAssignId);

            IEnumerable<RelationGroupModel> relationModelList = job.RelationGroupList.Where(x => x.HousingDepartmentId == housingDepartmentId).ToList();
            List<JobAssign> allJobAssigns = relationModelList.Any() ? GetJobAssignList(relationModelList.Select(x => x.RelationGroupId)) : jobAssignProvider.GetAllByJobId(jobId);
            IEnumerable<JobAssign> localJobAssignsWithoutUpdateIntervals = allJobAssigns.Where(ja => !ja.IsGlobal && !ja.IsLocalIntervalChanged);

            foreach (var jobAssign in localJobAssignsWithoutUpdateIntervals)
            {
                await messageBus.Publish(new JobAssignCopyCommonInfoCommand(jobAssign.Id, globalJobAssign.TillYear,
                        globalJobAssign.WeekList, globalJobAssign.DayPerWeekList, globalJobAssign.RepeatsPerWeek,
                        globalJobAssign.ChangedByRole, globalJobAssign.JobResponsibleList));
            }
        }

        public async Task CopyParentGlobalJobAssignToChildren(Guid globalJobAssignId, string jobId)
        {
            var childrenJobIds = jobProvider.Query.Where(i => i.ParentId == jobId).Select(i => i.Id).ToList();
            var jobAssignList = jobAssignProvider.Query
                                                 .Where(i => (i.Id == globalJobAssignId) || (i.IsGlobal && i.JobIdList.Any(j => childrenJobIds.Contains(j))))
                                                 .ToList();

            JobAssign globalJobAssign = jobAssignList.First(i => i.Id == globalJobAssignId);
            jobAssignList.Remove(globalJobAssign);

            foreach (var jobAssign in jobAssignList)
            {
                await messageBus.Publish(new JobAssignCopyCommonInfoCommand(jobAssign.Id, globalJobAssign.TillYear,
                        globalJobAssign.WeekList, globalJobAssign.DayPerWeekList, globalJobAssign.RepeatsPerWeek,
                        globalJobAssign.ChangedByRole, globalJobAssign.JobResponsibleList));
            }
        }

        public List<JobAssign> GetAllByJobId(string jobId, Guid? managementDepartmentId = null)
        {
            var assigns = jobAssignProvider.GetAllByJobId(jobId);

            if (managementDepartmentId.HasValue)
            {
                var departmentIds = managementService.GetHousingDepartments(managementDepartmentId.Value).Select(x => x.Id);
                assigns = FilterDepartmentsByManagement(assigns, departmentIds, jobId);
            }

            return assigns;
        }

        public IEnumerable<IHousingDepartmentModel> GetAssignedDepartments(string jobId)
        {
            var currentUser = memberService.GetCurrentUser();
            IJob job = GetJobById(jobId).Result;
            var assigns = jobAssignProvider.GetAllByJobId(jobId);
            var departments = Enumerable.Empty<IHousingDepartmentModel>();
            if (currentUser.IsAdmin())
            {
                departments = job.ParentId != null
                    ? managementService.GetHousingDepartments(job.RelationGroupList.Select(x => x.HousingDepartmentId))
                    : managementService.GetHousingDepartments(assigns.SelectMany(x => x.HousingDepartmentIdList));
            }
            else
            {
                departments = job.ParentId != null
                    ? managementService.GetHousingDepartments(currentUser.ActiveManagementDepartmentId.Value, job.RelationGroupList.Select(x => x.HousingDepartmentId))
                    : managementService.GetHousingDepartments(currentUser.ActiveManagementDepartmentId.Value, assigns.SelectMany(x => x.HousingDepartmentIdList));
            }

            return departments;
        }

        public IEnumerable<IHousingDepartmentModel> GetGroupedAssignedDepartments(string jobId)
        {
            IJob job = GetJobById(jobId).Result;
            List<IHousingDepartmentModel> departments = managementService.GetHousingDepartments(job.RelationGroupList.Select(x => x.HousingDepartmentId).Distinct()).ToList();

            if (job.ParentId == null)
            {
                FilterHousingDepartmentIfJobIsHiden(job.Id, departments);
            }

            return departments;
        }

        public IEnumerable<IHousingDepartmentModel> GetHousingDepartmentsForGroupingTasks(string jobId)
        {
            IEnumerable<Guid> assignedHousingDepartmentIds = GetAssignedHousingDepartmentIdList(jobId);
            IMemberModel currentUser = memberService.GetCurrentUser();

            if (currentUser.IsAdmin())
            {
                return GetHousingDepartmentListForAdminRole(assignedHousingDepartmentIds);
            }

            return GetHousingDepartmentListForCoordinatorRole(assignedHousingDepartmentIds, currentUser.ActiveManagementDepartmentId.Value);
        }

        public JobAssign GetJobAssignById(Guid id)
        {
            return jobAssignProvider.GetById(id);
        }

        public async Task<IJob> GetJobById(string id)
        {
            return await jobProvider.Get(id);
        }

        public JobAssign GetJobAsignByJobIdAndAssignedDepartment(string jobId, Guid assignedDepartmentId, bool isGlobal)
        {
            if (isGlobal)
            {
                return jobAssignProvider.GetGlobalAssignByJobId(jobId);
            }
            return jobAssignProvider.GetAssignByJobIdAndDepartmentId(jobId, assignedDepartmentId);
        }

        public IEnumerable<IJobHeaderModel> GetOpenedJobsHeaderList()
        {
            IMemberModel user = memberService.GetCurrentUser();
            var housingDepartments = user.ActiveManagementDepartmentId.HasValue ? managementService.GetHousingDepartments(user.ActiveManagementDepartmentId.Value).Select(d => d.Id) : null;
            var groupIds = groupService.GetAllByUserId(user.MemberId).Select(i => i.Id);
            List<IDayAssign> openedDayAssignList = dayAssignService.GetListForCurrentUser(housingDepartments, groupIds, OpenedJobStatuses);
            IEnumerable<string> openedJobIds = openedDayAssignList.Select(x => x.JobId);
            IEnumerable<IJob> hiddenJobs = jobProvider.GetHiddenByIds(openedJobIds);

            RemoveDayAssignsWithNotAllowedStatuses(hiddenJobs, openedDayAssignList, OpenedJobStatuses);

            return GetJobHeaderList(openedDayAssignList);
        }

        public IEnumerable<IJobHeaderModel> GetMyJobsHeaderList()
        {
            IMemberModel user = memberService.GetCurrentUser();
            var housingDepartments = user.ActiveManagementDepartmentId.HasValue ? managementService.GetHousingDepartments(user.ActiveManagementDepartmentId.Value).Select(d => d.Id) : null;
            List<IDayAssign> dayAssignList = dayAssignProvider.GetDayAssignsForMember(user.MemberId, true, housingDepartments, user.DaysAhead);
            List<IDayAssign> jobsWithoutPending = dayAssignList.Where(j => MyTaskJobStatuses.Contains(j.StatusId)).ToList();

            IEnumerable<string> openedJobIds = dayAssignList.Select(x => x.JobId);
            IEnumerable<IJob> hiddenJobs = jobProvider.GetHiddenByIds(openedJobIds);

            RemoveDayAssignsWithNotAllowedStatuses(hiddenJobs, jobsWithoutPending, NotAllowedJobStatuses);

            return GetJobHeaderList(jobsWithoutPending);
        }

        public IEnumerable<IJobHeaderModel> GetCompletedJobsHeaderList()
        {
            IMemberModel user = memberService.GetCurrentUser();
            var housingDepartments = user.ActiveManagementDepartmentId.HasValue ? managementService.GetHousingDepartments(user.ActiveManagementDepartmentId.Value).Select(d => d.Id) : null;

            if (housingDepartments.HasValue())
            {
                List<IDayAssign> dayAssignList = dayAssignProvider.GetDayAssignsForMember(user.MemberId, true, housingDepartments);
                IEnumerable<IDayAssign> completedJobs = dayAssignList.Where(j => CompletedJobStatuses.Contains(j.StatusId));
                return GetCompletedJobHeaderList(completedJobs);
            }

            return Enumerable.Empty<IJobHeaderModel>();
        }

        public IJobCounterModel GetJobCounters()
        {
            var result = new JobCounterModel();
            IMemberModel user = memberService.GetCurrentUser();
            var housingDepartments = user.ActiveManagementDepartmentId.HasValue ? managementService.GetHousingDepartments(user.ActiveManagementDepartmentId.Value).Select(d => d.Id) : null;
            var groupIds = groupService.GetAllByUserId(user.MemberId).Select(i => i.Id);

            List<IDayAssign> openedDayAssigns = dayAssignService.GetListForCurrentUser(housingDepartments, groupIds, OpenedJobStatuses);
            List<IDayAssign> dayAssignList = dayAssignProvider.GetDayAssignsForMemberByStatuses(user.MemberId, MyTaskJobStatuses, user.DaysAhead, housingDepartments);

            IEnumerable<string> openedJobIds = openedDayAssigns.Select(x => x.JobId);
            IEnumerable<string> jobIds = dayAssignList.Select(x => x.JobId);

            IEnumerable<IJob> hiddenOpenedJobs = jobProvider.GetHiddenByIds(openedJobIds);
            IEnumerable<IJob> hiddenJobs = jobProvider.GetHiddenByIds(jobIds);

            RemoveDayAssignsWithNotAllowedStatuses(hiddenOpenedJobs, openedDayAssigns, OpenedJobStatuses);
            RemoveDayAssignsWithNotAllowedStatuses(hiddenJobs, dayAssignList, NotAllowedJobStatuses);

            result.JanitorTasksCount = dayAssignList.Count;
            result.OpenedTasksCount = openedDayAssigns.Count;

            return result;
        }

        public IJobDetailsModel GetJobDetails(Guid dayAssignId)
        {
            DayAssign dayAssign = dayAssignProvider.Get(dayAssignId);
            Job job = jobProvider.Get(dayAssign.JobId).Result;

            RelationGroupModel relationModel = job.RelationGroupList.FirstOrDefault(x => x.HousingDepartmentId == dayAssign.DepartmentId);
            JobAssign globalJobAssign = relationModel != null ? GetGlobalJobAssignForGroupedTask(job) : jobAssignProvider.GetGlobalAssignByJobId(job.Id);
            JobAssign localJobAssign = GetLocalJobAssign(job.Id, dayAssign.DepartmentId);
            IGroupModel group = GetGroupModel(dayAssign.GroupId);

            globalJobAssign = GetCorrectJobAssign(globalJobAssign);
            localJobAssign = GetCorrectJobAssign(localJobAssign);

            return new JobDetailsModel
            {
                JobId = job.Id,
                DayAssignId = dayAssign.Id,
                NiceId = GetNiceId(job.Id),
                Title = job.Title,
                RelationGroupId = relationModel?.RelationGroupId,
                Address = job.GetAddress(dayAssign.DepartmentId),
                Date = dayAssign.Date,
                GlobalDescription = globalJobAssign == null ? string.Empty : globalJobAssign.Description,
                LocalDescription = localJobAssign == null ? string.Empty : localJobAssign.Description,
                JanitorUploadImageList = dayAssign.UploadList.Where(x => x.ContentType == UploadedContentEnum.Image).ToList(),
                JanitorUploadVideoList = dayAssign.UploadList.Where(x => x.ContentType == UploadedContentEnum.Video).ToList(),
                GroupId = dayAssign.GroupId,
                GroupName = group == null ? string.Empty : group.Name,
                Members = GetAssignedMembers(group, dayAssign),
                TeamLeadId = dayAssign.TeamLeadId,
                GlobalUploadList = globalJobAssign == null ? new List<UploadFileModel>() : globalJobAssign.UploadList,
                LocalUploadList = localJobAssign == null ? new List<UploadFileModel>() : localJobAssign.UploadList,
                JobType = job.JobTypeId,
                JobStatus = dayAssign.StatusId,
                AllowChangeStatus = dayAssignService.IsAllowChangeDayAssignStatus(dayAssign),
                Estimate = dayAssign.EstimatedMinutes,
                TenantTypeString = dayAssign.TenantType?.ToString() ?? string.Empty,
                ResidentName = dayAssign.ResidentName,
                ResidentPhone = dayAssign.ResidentPhone,
                Comment = dayAssign.Comment,
                IsUrgent = dayAssign.IsUrgent,
                AssignedHousingDepartmentName = GetAssignedHousingDepartmentName(dayAssign.DepartmentId)
            };
        }

        public IEnumerable<IJobRelatedByAddressModel> GetTenantJobsRelatedByAddress(string jobId)
        {
            List<IJobRelatedByAddressModel> jobsRelatedByaddressList = new List<IJobRelatedByAddressModel>();
            IJob job = jobProvider.Query.First(j => j.Id == jobId);

            if (!string.IsNullOrEmpty(job.FirstAddress))
            {
                Func<Job, bool> predicate = j => j.AddressList.Any(addr => addr.Address == job.FirstAddress) && j.JobTypeId == JobTypeEnum.Tenant && j.Id != jobId;
                IEnumerable<IJob> relatedByAddressTenantJobs = jobProvider.Query.Where(predicate);

                foreach (var relatedByaddressJob in relatedByAddressTenantJobs)
                {
                    IEnumerable<IDayAssign> dayAssigns = dayAssignProvider.GetDayAssignsByJobId(relatedByaddressJob.Id);
                    IDayAssign tenantDayAssign = dayAssigns.FirstOrDefault();

                    JobRelatedByAddressModel model = new JobRelatedByAddressModel
                    {
                        JobId = relatedByaddressJob.Id,
                        DayAssignId = tenantDayAssign.Id,
                        Date = tenantDayAssign.Date,
                        Address = relatedByaddressJob.FirstAddress,
                        Title = relatedByaddressJob.Title
                    };

                    jobsRelatedByaddressList.Add(model);
                }
            }
            return jobsRelatedByaddressList;
        }

        public Guid GetJobAssignId(string jobId, Guid departmentId)
        {
            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(jobId, departmentId);
            return jobAssign.Id;
        }

        public IEnumerable<Guid> GetUserIdsFromActiveJobs()
        {
            var completedTypes = new List<JobStatus> { JobStatus.Canceled, JobStatus.Completed, JobStatus.Expired };
            var dayAssignList = dayAssignProvider.Find(i => !completedTypes.Contains(i.StatusId));
            var result = new List<Guid>();

            foreach (var assign in dayAssignList)
            {
                if (assign.TeamLeadId.HasValue)
                {
                    result.Add(assign.TeamLeadId.Value);
                }

                result.AddRange(assign.UserIdList);
            }
            return result.Distinct();
        }

        public bool IsAllowedTaskGrouping(string jobId)
        {
            var job = jobProvider.Query.First(i => i.Id == jobId);
            return !job.IsHidden && job.JobTypeId == JobTypeEnum.Facility;
        }

        public async Task<string> CreateTaskRelationGroup(string jobId, Guid housingDepartmentId)
        {
            if (!IsAllowedCreationGroupedTask(jobId))
            {
                throw new Exception($"You can't create a relation group for task: {jobId}");
            }

            var createdJobId = await AddTaskToRelationGroup(jobId, housingDepartmentId);
            return createdJobId;
        }

        public async Task<string> AddTaskToRelationGroup(string jobId, Guid housingDepartmentId)
        {
            Job job = jobProvider.Query.First(i => i.Id == jobId);
            Job parentJob = GetParentJob(job);

            if (!IsAllowedTaskGrouping(parentJob))
            {
                throw new Exception($"You can't add a relation group for task: {jobId}");
            }

            var relationGroupModel = parentJob.RelationGroupList.FirstOrDefault(x => x.HousingDepartmentId == housingDepartmentId);

            if (relationGroupModel == null)
            {
                relationGroupModel = new RelationGroupModel
                {
                    HousingDepartmentId = housingDepartmentId,
                    RelationGroupId = Guid.NewGuid()
                };

                jobProvider.SaveRelationGroupId(jobId, relationGroupModel);

                var jobAssigns = jobAssignProvider.Query.Where(i => i.JobIdList.Contains(jobId)).ToList();
                var globalAssign = jobAssigns.First(i => i.IsGlobal);
                if (!globalAssign.HousingDepartmentIdList.HasValue())
                {
                    var localAssign = jobAssigns.First(i => !i.IsGlobal);
                    await messageBus.Publish(new AssignJobCommand(globalAssign.Id, localAssign.HousingDepartmentIdList.First()));
                }
            }

            var createdJobId = await CreateRelatedFacilityTask(parentJob, new List<RelationGroupModel> { relationGroupModel }, housingDepartmentId);
            return createdJobId;
        }

        public bool IsGroupedTask(string jobId)
        {
            var job = jobProvider.Query.First(i => i.Id == jobId);
            return job.RelationGroupList.Any();
        }

        public bool IsChildGroupedTask(string jobId)
        {
            var job = jobProvider.Query.First(i => i.Id == jobId);
            return job.ParentId != null;
        }

        public async Task SaveTitle(IdValueModel<string, string> model)
        {
            var tasks = new List<Task>();
            var jobIds = GetRelatedJobIds(model.Id);

            foreach (var jobId in jobIds)
            {
                tasks.Add(messageBus.Publish(new ChangeJobTitleCommand(jobId, model.Value)));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public async Task SaveCategory(IdValueModel<string, Guid> model)
        {
            var tasks = new List<Task>();
            var jobIds = GetRelatedJobIds(model.Id);

            foreach (var jobId in jobIds)
            {
                tasks.Add(messageBus.Publish(new ChangeJobCategoryCommand(jobId, model.Value)));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public List<JobAssign> GetJobAssignList(IEnumerable<Guid> relationGroupIdList)
        {
            var jobIds = jobProvider.Query.Where(i => i.RelationGroupList.Any(x => relationGroupIdList.Contains(x.RelationGroupId))).Select(i => i.Id).ToList();
            var assigns = jobAssignProvider.Query.Where(i => i.JobIdList.Any(j => jobIds.Contains(j))).ToList();
            FillCorrectPathForJobAssignsUploads(assigns);

            return assigns;
        }

        public IEnumerable<IdValueModel<string, string>> GetRelatedAddressListForHousingDepartment(IEnumerable<Guid> relationGroupIdList, Guid housingDepartmentId, bool isParent)
        {
            var jobAddressListModel = GetJobAddressList(relationGroupIdList);
            return jobAddressListModel.Select(x => GetJobToAddressRelationModel(x, housingDepartmentId, isParent)).ToList();
        }

        public async Task<JobAssign> CreateOrGetJobAssign(Guid assignId, string jobId, Guid housingDepartmentId)
        {
            var jobAssign = jobAssignProvider.GetById(assignId);
            return jobAssign ?? await CreateLocalJobAssignFromGlobal(jobId, housingDepartmentId);
        }

        public IEnumerable<string> GetHousingDepartmentAddressesOrAllAddressesForUserManagementDepartment(Guid? housingDepartmentId)
        {
            if (housingDepartmentId.HasValue)
            {
                IHousingDepartmentModel housingDepartment = managementService.GetHousingDepartment(housingDepartmentId.Value);
                return housingDepartment.AddressList.Where(x => x.HasValue()).Distinct();
            }

            IMemberModel currentUser = memberService.GetCurrentUser();
            IEnumerable<IHousingDepartmentModel> housingDepartmentList = currentUser.ActiveManagementDepartmentId.HasValue
                ? managementService.GetHousingDepartments(currentUser.ActiveManagementDepartmentId.Value)
                : Enumerable.Empty<IHousingDepartmentModel>();

            return housingDepartmentList.SelectMany(hd => hd.AddressList).Where(x => x.HasValue()).Distinct();
        }

        public IEnumerable<IJob> GetTenantTasksByAddress(string address)
        {
            IEnumerable<IJob> result = jobProvider.Query.Where(x => x.AddressList.Any(y => y.Address == address));
            return result;
        }

        public IEnumerable<string> GetAddressesForManagementDepartment(Guid? managementDepartmentId)
        {
            IEnumerable<IHousingDepartmentModel> housingDepartments = managementDepartmentId.HasValue
                    ? managementService.GetHousingDepartments(managementDepartmentId.Value)
                    : managementService.GetAllHousingDepartments();

            return housingDepartments.SelectMany(hd => hd.AddressList).Distinct().OrderBy(address => address);

        }

        public IEnumerable<IJob> GetJobsByJobType(JobTypeEnum jobTypeId)
        {
            return jobProvider.GetJobsByJobType(jobTypeId);
        }

        public Dictionary<Guid, IApproximateSpentTimeModel> GetMembersApproximateSpentTimeList(Guid dayAssignId, DateTime currentDate)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            IJob job = jobProvider.Query.First(j => j.Id == dayAssign.JobId);

            IEnumerable<Guid> assignedMemberIdList = GetAssignedMembersIdList(dayAssign);
            List<IJobStatusLogModel> statusLogList = jobStatusLogService.GetLogsByDayAssignIds(new List<Guid> { dayAssignId }).ToList();

            if (job.JobTypeId == JobTypeEnum.Other)
            {
                return GetMembersApproximateSpentTimeListForTenant(dayAssign, assignedMemberIdList, statusLogList);
            }

            return GetMembersApproximateSpentTimeListForTasks(assignedMemberIdList, currentDate, statusLogList);
        }

        public JobAssign GetGlobalJobAssignForGroupedTask(IJob job)
        {
            return job.RelationGroupId.HasValue
                    ? GetAllByJobId(job.Id).First(x => x.IsGlobal) // for old grouped task
                    : GetParentGlobalAssignForGroupedTask(job); // for new groupedTask
        }

        public JobAssign GetParentGlobalAssignForGroupedTask(IJob job)
        {
            string jobId = job.ParentId ?? job.Id;
            JobAssign globalJobAssign = jobAssignProvider.Query.First(x => x.JobIdList.Contains(jobId) && x.IsGlobal);
            FillCorrectPathForJobAssignsUploads(new List<JobAssign> { globalJobAssign });
            return globalJobAssign;
        }

        public IEnumerable<IJob> GetJobsByRelationGroupIds(IEnumerable<Guid> relationGroupIds)
        {
            return jobProvider.Query.Where(x => x.RelationGroupList.Any(y => relationGroupIds.Contains(y.RelationGroupId)));
        }

        public IEnumerable<IJob> GetChildJobsForHousingDepartment(string jobId, Guid housingDepartmentId)
        {
            IEnumerable<IJob> childJobs = jobProvider.Query.Where(x => x.ParentId == jobId && x.RelationGroupList.Any(y => y.HousingDepartmentId == housingDepartmentId));
            return childJobs;
        }

        public bool IsRelationListMatchJobAssignList(IJob job, IEnumerable<JobAssign> jobAssignList)
        {
            var currentUser = memberService.GetCurrentUser();
            List<Guid> relatedGroupIdList = job.RelationGroupList.Select(x => x.HousingDepartmentId).ToList();
            List<Guid> assignedHousingDepartmentIdList = jobAssignList.SelectMany(x => x.HousingDepartmentIdList).Distinct().ToList();

            if (!currentUser.IsAdmin())
            {
                var housingDepartmentIds = managementService.GetHousingDepartmentIds(currentUser.ActiveManagementDepartmentId.Value, relatedGroupIdList);
                assignedHousingDepartmentIdList.RemoveAll(x => !housingDepartmentIds.Contains(x));
            }

            if (relatedGroupIdList.Count == 0)
            {
                return true;
            }

            bool result = relatedGroupIdList.All(assignedHousingDepartmentIdList.Contains) && relatedGroupIdList.Count == assignedHousingDepartmentIdList.Count;
            return result;
        }

        public bool CheckIsAllChildGroupedTaskHided(IJob job)
        {
            if (job.ParentId != null)
            {
                return false;
            }

            List<IJob> childTaskList = GetJobsByRelationGroupIds(job.RelationGroupList.Select(x => x.RelationGroupId)).Where(x => x.ParentId != null).ToList();
            bool result = childTaskList.All(x => x.IsHidden);
            return result;
        }

        public bool IsPossibleToHideChildTask(string parentJobId, Guid housingDepartmentId)
        {
            return IsParentStillAssigned(parentJobId, housingDepartmentId) && !IsParentHidden(parentJobId);
        }

        public void FillCorrectPathForJobAssignsUploads(List<JobAssign> jobAssigns)
        {
            foreach (var jobAssign in jobAssigns)
            {
                SetCorrectPathForUploadList(jobAssign.UploadList, jobAssign.Id);
            }
        }

        private void RemoveDayAssignsWithNotAllowedStatuses(IEnumerable<IJob> hiddenJobs, List<IDayAssign> dayAssignList, IEnumerable<JobStatus> statusesForRemove)
        {
            foreach (var job in hiddenJobs)
            {
                List<IDayAssign> hiddenDayAssign = dayAssignList.Where(x => x.JobId == job.Id && statusesForRemove.Contains(x.StatusId)).ToList();
                if (hiddenDayAssign.HasValue())
                {
                    foreach (var dayAssign in hiddenDayAssign)
                    {
                        dayAssignList.Remove(dayAssign);
                    }
                }
            }
        }

        public List<IJob> GetByCategoryIds(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true)
        {
            var result = new List<IJob>();
            IMemberModel currentUser = memberService.GetCurrentUser();

            if (currentUser.IsAdmin())
            {
                List<Job> jobs = jobProvider.Get(categoryIds, includeGroupedTasks, includeHiddenTasks);
                result.AddRange(jobs);
            } 
            else
            {
                Guid managementDepartmentId = currentUser.ActiveManagementDepartmentId ?? Guid.Empty;
                IEnumerable<Guid> memberDepartmentIds = managementService.GetHousingDepartments(managementDepartmentId).Select(d => d.Id);
                List<Job> jobs = jobProvider.Get(categoryIds, memberDepartmentIds, includeGroupedTasks, includeHiddenTasks);

                result.AddRange(jobs);
            }
            

            return result;
        }

        private void FilterHousingDepartmentIfJobIsHiden(string jobId, List<IHousingDepartmentModel> departments)
        {
            IEnumerable<IJob> childJobs = jobProvider.GetByParentId(jobId);

            foreach (var childJob in childJobs)
            {
                if (childJob.IsHidden)
                {
                    Guid housingDepartmentId = childJob.RelationGroupList.First().HousingDepartmentId;
                    departments.RemoveAll(x => x.Id == housingDepartmentId);
                }
            }
        }

        private List<JobAssign> FilterDepartmentsByManagement(List<JobAssign> assigns, IEnumerable<Guid> allowedDepartments, string jobId)
        {
            return assigns.Where(x => x.HousingDepartmentIdList.Intersect(allowedDepartments).Any() || x.JobIdList.Contains(jobId) && x.IsGlobal).ToList();
        }

        private Job GetParentJob(Job job)
        {
            return job.ParentId == null ? job : jobProvider.Query.First(i => i.Id == job.ParentId);
        }

        private IEnumerable<IdValueModel<string, List<JobAddress>>> GetJobAddressList(IEnumerable<Guid> relationGroupIdList)
        {
            var data = jobProvider.Query.Where(i => i.RelationGroupList.Any(x => relationGroupIdList.Contains(x.RelationGroupId)) && !i.IsHidden)
                                        .Select(i => new IdValueModel<string, List<JobAddress>>
                                        {
                                            Id = i.Id,
                                            Value = i.AddressList
                                        })
                                         .ToList();
            return data;
        }

        private IdValueModel<string, string> GetJobToAddressRelationModel(IdValueModel<string, List<JobAddress>> model, Guid housingDepartmentId, bool isParent)
        {
            if (isParent)
            {
                JobAddress addressListForHousingDepartment = model.Value.FirstOrDefault(x => x.HousingDepartmentId == housingDepartmentId);

                return new IdValueModel<string, string>
                {
                    Id = model.Id,
                    Value = addressListForHousingDepartment != null ? addressListForHousingDepartment.Address : string.Empty
                };
            }
            else
            {
                return new IdValueModel<string, string>
                {
                    Id = model.Id,
                    Value = model.Value.Any(x => x.HousingDepartmentId == housingDepartmentId) ? model.Value.First(x => x.HousingDepartmentId == housingDepartmentId).Address : string.Empty
                };
            }
        }

        private bool IsAllowedTaskGrouping(IJob job)
        {
            return !job.IsHidden && job.JobTypeId == JobTypeEnum.Facility;
        }

        private IEnumerable<IJobHeaderModel> GetJobHeaderList(IEnumerable<IDayAssign> dayAssigns)
        {
            List<JobHeaderModel> jobHeaderList = GetJobHeaderModelList(dayAssigns);

            var tenantTaskList = jobHeaderList.Where(x => x.JobType == JobTypeEnum.Tenant).OrderBy(x => x.Date);
            var taskList = jobHeaderList.Where(x => x.JobType != JobTypeEnum.Tenant).OrderBy(x => x.Date);
            var result = tenantTaskList.Union(taskList).ToList();

            return result;
        }

        private IEnumerable<IJobHeaderModel> GetCompletedJobHeaderList(IEnumerable<IDayAssign> dayAssigns)
        {
            DateTime currentDate = DateTime.UtcNow;
            int previousWeekNumber = currentDate.GetPreviousWeekNumber();
            DateTime prewiousWeekMonday = CalendarHelper.GetFirstDayOfWeek(currentDate.Year, previousWeekNumber);
            IEnumerable<IDayAssign> filteredDayAssigns = dayAssigns.Where(x => x.Date >= prewiousWeekMonday);
            List<JobHeaderModel> jobHeaderList = GetJobHeaderModelList(filteredDayAssigns);
            return jobHeaderList.OrderByDescending(x => x.Date);
        }

        private Dictionary<Guid, IApproximateSpentTimeModel> GetMembersApproximateSpentTimeListForTenant(
            IDayAssign dayAssign, IEnumerable<Guid> assignedMemberIdList, List<IJobStatusLogModel> statusLogList)
        {
            Dictionary<Guid, IApproximateSpentTimeModel> approximateMembersSpentTimeList = assignedMemberIdList.ToDictionary(k => k, v => (IApproximateSpentTimeModel)new ApproximateSpentTimeModel());

            foreach (var memberId in assignedMemberIdList)
            {
                if (!absenceInfoService.IsMemberHasAbsenceForDate(memberId, dayAssign.Date.Value))
                {
                    int spentTime = dayAssign.EstimatedMinutes ?? default(int);
                    approximateMembersSpentTimeList[memberId] = GetMembersApproximateSpentTimeModel(spentTime);
                }
            }

            FillMembersTotalTime(approximateMembersSpentTimeList, statusLogList);
            return approximateMembersSpentTimeList;
        }

        private Dictionary<Guid, IApproximateSpentTimeModel> GetMembersApproximateSpentTimeListForTasks(
            IEnumerable<Guid> assignedMemberIdList, DateTime currentDate, List<IJobStatusLogModel> statusLogList)
        {
            Dictionary<Guid, IApproximateSpentTimeModel> approximateMembersSpentTimeList =
                assignedMemberIdList.ToDictionary(k => k, v => (IApproximateSpentTimeModel)new ApproximateSpentTimeModel());

            IJobStatusLogModel lastAssignedStatusLog = statusLogList.Last(x => x.StatusId == JobStatus.Assigned);
            int lastAssignedStatusLogIndex = statusLogList.IndexOf(lastAssignedStatusLog);
            int requireslogSequenceCount = statusLogList.Count - lastAssignedStatusLogIndex;
            List<IJobStatusLogModel> lastAssignedLogSequence = statusLogList.GetRange(lastAssignedStatusLogIndex, requireslogSequenceCount);

            if (lastAssignedLogSequence.All(x => x.StatusId != JobStatus.InProgress))
            {
                return approximateMembersSpentTimeList;
            }

            IDictionary<Guid, int> approximateSpentTimeModelList = GetApproximateSpentTimeModelList(lastAssignedLogSequence, currentDate, assignedMemberIdList);
            foreach (var spentTimeModel in approximateSpentTimeModelList)
            {
                approximateMembersSpentTimeList[spentTimeModel.Key] = GetMembersApproximateSpentTimeModel(spentTimeModel.Value);
            }

            FillMembersTotalTime(approximateMembersSpentTimeList, statusLogList);
            return approximateMembersSpentTimeList;
        }

        private void FillMembersTotalTime(Dictionary<Guid, IApproximateSpentTimeModel> approximateMembersSpentTimeList, List<IJobStatusLogModel> statusLogList)
        {
            foreach (var memberSpentTime in approximateMembersSpentTimeList)
            {
                var notEmptyLogList = statusLogList.Where(x => x.TimeLogList.Any());
                var timeLogListForMember = notEmptyLogList.SelectMany(x => x.TimeLogList.Where(y => y.MemberId == memberSpentTime.Key));
                var totalTime = timeLogListForMember.Sum(x => x.SpentTime);
                memberSpentTime.Value.TotalSpentHours = totalTime.Hours;
                memberSpentTime.Value.TotalSpentMinutes = totalTime.Minutes;
            }
        }

        private IEnumerable<Guid> GetAssignedMembersIdList(IDayAssign dayAssign)
        {
            if (!dayAssign.GroupId.HasValue)
            {
                IEnumerable<Guid> memberIdList = GetAssignedMembers(null, dayAssign).Select(x => x.MemberId);
                return memberIdList;
            }

            IGroupModel group = groupService.Get(dayAssign.GroupId.Value);
            return group.MemberIds;
        }

        private IDictionary<Guid, int> GetApproximateSpentTimeModelList(List<IJobStatusLogModel> statusLogList, DateTime currentDate, IEnumerable<Guid> assignedMemberIdList)
        {
            Dictionary<Guid, int> result = assignedMemberIdList.ToDictionary(k => k, v => 0);

            for (var i = 0; i < statusLogList.Count; i++)
            {
                if (i + 1 < statusLogList.Count)
                {
                    FillSpentTimeModelListForPairOfStatuses(statusLogList[i], statusLogList[i + 1], assignedMemberIdList, result);
                }
                else
                {
                    FillSpentTimeModelListForLastStatus(statusLogList[i], currentDate, assignedMemberIdList, result);
                }
            }

            return result;
        }

        private void FillSpentTimeModelListForPairOfStatuses(
            IJobStatusLogModel currentStatus, IJobStatusLogModel nextStatus, IEnumerable<Guid> assignedMemberIdList, Dictionary<Guid, int> approximateSpentTimeModelList)
        {
            IDictionary<Guid, int> timeModelBetweenTwoStatuses = GetTimeModelBetweenTwoStatuses(currentStatus, nextStatus, assignedMemberIdList);
            MergeDictionary(approximateSpentTimeModelList, timeModelBetweenTwoStatuses);
        }

        private void FillSpentTimeModelListForLastStatus(IJobStatusLogModel lastStatus, DateTime currentDate, IEnumerable<Guid> assignedMemberIdList, Dictionary<Guid, int> approximateSpentTimeModelList)
        {
            if (CalendarHelper.IsWeekend(lastStatus.Date) || lastStatus.StatusId != JobStatus.InProgress)
            {
                return;
            }

            IDictionary<Guid, int> timeModelBetweenTwoDates = GetTimeForLastStatus(lastStatus, currentDate, assignedMemberIdList);
            MergeDictionary(approximateSpentTimeModelList, timeModelBetweenTwoDates);
        }

        private Dictionary<Guid, int> GetTimeModelBetweenTwoStatuses(IJobStatusLogModel currentStatus, IJobStatusLogModel nextStatus, IEnumerable<Guid> assignedMemberIdList)
        {
            if (IsNotAllowedPairOfStatuses(currentStatus, nextStatus) || IsDatesNotValid(currentStatus.Date, nextStatus.Date))
            {
                return new Dictionary<Guid, int>();
            }

            Dictionary<Guid, int> timeBeteweenStatuses = GetEmployeesTimeForStatuses(currentStatus, nextStatus, assignedMemberIdList);
            return timeBeteweenStatuses;
        }

        private IDictionary<Guid, int> GetTimeForLastStatus(IJobStatusLogModel currentStatus, DateTime currentDate, IEnumerable<Guid> assignedMemberIdList)
        {
            var result = assignedMemberIdList.ToDictionary(k => k, v => 0);
            IDictionary<int, int> weekDaysWorkingMinutes = appSettingHelper.GetDictionaryAppSetting<int, int>(Constants.AppSetting.DaysWorkingMinutes);
            IDictionary<DateTime, int> dateWithWeekDayList = CalendarHelper.GetDateWithWeekDayListForPeriod(currentStatus.Date, currentDate);

            foreach (var dateWithWeekDay in dateWithWeekDayList.Where(x => x.Value != (int)DayOfWeek.Saturday && x.Value != (int)DayOfWeek.Sunday))
            {
                double workingHours = (weekDaysWorkingMinutes[dateWithWeekDay.Value] + Constants.Common.LunchTimeInMinutes) / (double)Constants.Common.MinutesInOneHour;
                Dictionary<Guid, int> totalMinutesModel = GetTotalMinutesForLastStatus(currentStatus, dateWithWeekDay.Key, currentDate, workingHours, assignedMemberIdList);

                MergeDictionary(result, totalMinutesModel);
            }

            return result;
        }

        private bool IsNotAllowedPairOfStatuses(IJobStatusLogModel currentStatus, IJobStatusLogModel nextStatus)
        {
            return currentStatus.StatusId != JobStatus.InProgress && nextStatus.StatusId != JobStatus.Paused;
        }

        private Dictionary<Guid, int> GetEmployeesTimeForStatuses(IJobStatusLogModel currentStatus, IJobStatusLogModel nextStatus, IEnumerable<Guid> assignedMemberIdList)
        {
            Dictionary<Guid, int> result = assignedMemberIdList.ToDictionary(k => k, v => 0);

            IDictionary<int, int> weekDaysWorkingMinutes = appSettingHelper.GetDictionaryAppSetting<int, int>(Constants.AppSetting.DaysWorkingMinutes);
            IDictionary<DateTime, int> dateWithWeekDayList = CalendarHelper.GetDateWithWeekDayListForPeriod(currentStatus.Date, nextStatus.Date);

            foreach (var dateWithWeekDay in dateWithWeekDayList.Where(x => x.Value != (int)DayOfWeek.Saturday && x.Value != (int)DayOfWeek.Sunday))
            {
                double workingHours = (weekDaysWorkingMinutes[dateWithWeekDay.Value] + Constants.Common.LunchTimeInMinutes) / (double)Constants.Common.MinutesInOneHour;
                Dictionary<Guid, int> totalMinutesModel = GetEmployeeTotalMinutes(currentStatus, nextStatus, dateWithWeekDay.Key, workingHours, assignedMemberIdList);

                MergeDictionary(result, totalMinutesModel);
            }

            return result;
        }

        private Dictionary<Guid, int> GetEmployeeTotalMinutes(
            IJobStatusLogModel currentStatus, IJobStatusLogModel nextStatus, DateTime selectedDate, double workingHours, IEnumerable<Guid> assignedMemberIdList)
        {
            var defaultUtcWorkingTimeModel = new DefaultUtcWorkingTimeModel(appSettingHelper, selectedDate, workingHours);

            if (CalendarHelper.IsDatesIsSameDay(currentStatus.Date, nextStatus.Date)) // from currentStatus start date to nextStatus start date
            {
                return GetMembersTimeBetweenTwoDates(currentStatus.Date, nextStatus.Date, assignedMemberIdList);
            }
            else if (CalendarHelper.IsDatesIsSameDay(selectedDate, currentStatus.Date)) // from currentStatus start date to end of the day
            {
                return GetMembersTimeBetweenTwoDates(currentStatus.Date, defaultUtcWorkingTimeModel.DefaultUtcWorkFinishTime, assignedMemberIdList);
            }
            else if (CalendarHelper.IsDatesIsSameDay(selectedDate, nextStatus.Date)) // from start of the day to nextStatus start date
            {
                return GetMembersTimeBetweenTwoDates(defaultUtcWorkingTimeModel.DefaultUtcWorkStartTime, nextStatus.Date, assignedMemberIdList);
            }

            return GetMembersTimeForDay(selectedDate, workingHours, assignedMemberIdList);
        }

        private Dictionary<Guid, int> GetTotalMinutesForLastStatus(IJobStatusLogModel lastStatus, DateTime selectedDate, DateTime currentDate, double workingHours, IEnumerable<Guid> assignedMemberIdList)
        {
            var defaultUtcWorkingTimeModel = new DefaultUtcWorkingTimeModel(appSettingHelper, selectedDate, workingHours);
            DateTime changeStatusDate = currentDate < defaultUtcWorkingTimeModel.DefaultUtcWorkFinishTime ? currentDate : defaultUtcWorkingTimeModel.DefaultUtcWorkFinishTime;

            if (CalendarHelper.IsDatesIsSameDay(lastStatus.Date, currentDate))
            {
                return GetMembersTimeBetweenTwoDates(lastStatus.Date, changeStatusDate, assignedMemberIdList);
            }
            else if (CalendarHelper.IsDatesIsSameDay(lastStatus.Date, defaultUtcWorkingTimeModel.DefaultUtcWorkFinishTime))
            {
                return GetMembersTimeBetweenTwoDates(lastStatus.Date, defaultUtcWorkingTimeModel.DefaultUtcWorkFinishTime, assignedMemberIdList);
            }
            else if (CalendarHelper.IsDatesIsSameDay(selectedDate, currentDate))
            {
                return GetMembersTimeBetweenTwoDates(defaultUtcWorkingTimeModel.DefaultUtcWorkStartTime, changeStatusDate, assignedMemberIdList);
            }

            return GetMembersTimeForDay(selectedDate, workingHours, assignedMemberIdList);
        }

        private Dictionary<Guid, int> GetMembersTimeBetweenTwoDates(DateTime firstDate, DateTime secondDate, IEnumerable<Guid> assignedMemberIdList)
        {
            var result = new Dictionary<Guid, int>();

            if (secondDate < firstDate) // It possible if janitor completed task before 8 am.
            {
                return result;
            }

            TimeSpan datesDifference = secondDate - firstDate;

            foreach (var memberId in assignedMemberIdList)
            {
                int totalMinutes = 0;
                if (!absenceInfoService.IsMemberHasAbsenceForDate(memberId, firstDate))
                {
                    totalMinutes = (int)datesDifference.TotalMinutes;
                }

                result.Add(memberId, totalMinutes);
            }

            return result;
        }

        private Dictionary<Guid, int> GetMembersTimeForDay(DateTime selectedDate, double workingHours, IEnumerable<Guid> assignedMemberIdList)
        {
            var result = new Dictionary<Guid, int>();
            int totalTime = 0;

            foreach (var memberId in assignedMemberIdList)
            {
                if (!absenceInfoService.IsMemberHasAbsenceForDate(memberId, selectedDate))
                {
                    totalTime = Convert.ToInt32(workingHours * Constants.Common.MinutesInOneHour);
                }

                result.Add(memberId, totalTime);
            }

            return result;
        }

        private void MergeDictionary(IDictionary<Guid, int> target, IDictionary<Guid, int> source)
        {
            foreach (var item in source)
            {
                target[item.Key] += item.Value;
            }
        }

        private ApproximateSpentTimeModel GetMembersApproximateSpentTimeModel(int totalMinutes)
        {
            int hours = totalMinutes / Constants.Common.MinutesInOneHour;
            int minutes = totalMinutes - (hours * Constants.Common.MinutesInOneHour);

            return new ApproximateSpentTimeModel
            {
                ApproximateSpentHours = hours,
                ApproximateSpentMinutes = minutes
            };
        }

        private bool IsDatesNotValid(DateTime firstDate, DateTime secondDate)
        {
            return CalendarHelper.IsDatesIsSameDay(firstDate, secondDate) && CalendarHelper.IsWeekend(firstDate);
        }

        private IEnumerable<Guid> GetAssignedHousingDepartmentIdList(string jobId)
        {
            var result = jobAssignProvider.Query.Where(i => i.JobIdList.Contains(jobId) && i.IsEnabled)
                                       .SelectMany(i => i.HousingDepartmentIdList)
                                       .ToList().Distinct();
            return result;
        }

        private IEnumerable<IHousingDepartmentModel> GetHousingDepartmentListForAdminRole(IEnumerable<Guid> assignedHousingDepartmentIds)
        {
            if (!assignedHousingDepartmentIds.HasValue())
            {
                return managementService.GetAllHousingDepartments();
            }

            return managementService.GetHousingDepartments(assignedHousingDepartmentIds);
        }

        private IEnumerable<IHousingDepartmentModel> GetHousingDepartmentListForCoordinatorRole(IEnumerable<Guid> assignedHousingDepartmentIds, Guid managementDepartmentId)
        {
            IManagementDepartmentModel managementDepartment = managementService.GetManagementDepartmentById(managementDepartmentId);
            IEnumerable<Guid> assignedHousingDepartmentIdsForCurrentManagementDepartment = assignedHousingDepartmentIds
                .Where(x => managementDepartment.HousingDepartmentList.Any(i => i.Id == x));

            if (!assignedHousingDepartmentIdsForCurrentManagementDepartment.HasValue())
            {
                return managementDepartment.HousingDepartmentList;
            }

            return managementService.GetHousingDepartments(assignedHousingDepartmentIdsForCurrentManagementDepartment);
        }

        private string GetAssignedHousingDepartmentName(Guid housingDepartmentId)
        {
            IHousingDepartmentModel housingDepartment = managementService.GetHousingDepartment(housingDepartmentId);
            return housingDepartment.DisplayName;
        }

        private async Task<JobAssign> CreateLocalJobAssignFromGlobal(string jobId, Guid housingDepartmentId)
        {
            var assignId = Guid.NewGuid();
            IJob job = await GetJobById(jobId);
            JobAssign globalAssign;

            if (job.ParentId != null && job.RelationGroupList.HasValue())
            {
                globalAssign = GetGlobalJobAssignForGroupedTask(job);
            }
            else
            {
                globalAssign = jobAssignProvider.GetGlobalAssignByJobId(jobId);
            }

            var command = Mapper.Map(globalAssign, new CreateJobAssignFromJobAssignCommand(assignId, new List<Guid> { housingDepartmentId }));
            command.JobIdList = new List<string> { jobId };
            await messageBus.Publish(command);
            await messageBus.Publish(new ChangeJobIdAndJobAssignIdInDayAssignCommand(jobId, housingDepartmentId, assignId));

            return GetJobAssignById(assignId);
        }

        private IEnumerable<string> GetRelatedJobIds(string jobId)
        {
            var job = jobProvider.Query.First(i => i.Id == jobId);

            if (job.ParentId != null)
            {
                job = jobProvider.Query.First(i => i.Id == job.ParentId);
            }

            if (!job.RelationGroupList.HasValue())
            {
                return new List<string> { job.Id };
            }

            IEnumerable<Guid> relationIdList = job.RelationGroupList.Select(x => x.RelationGroupId).ToList();
            var ids = jobProvider.Query.Where(i => i.RelationGroupList.Any(x => relationIdList.Contains(x.RelationGroupId))).Select(i => i.Id).ToList();
            return ids;
        }

        private async Task<string> CreateRelatedFacilityTask(Job job, List<RelationGroupModel> relationGroupList, Guid housingDepartmentId)
        {
            var jobId = await taskIdGenerator.Facility();
            var currentUser = memberService.GetCurrentUser();
            string parentId = job.ParentId ?? job.Id;
            await messageBus.Publish(new CreateJobCommand(jobId, job.CategoryId, job.Title, JobTypeEnum.Facility, currentUser.MemberId, currentUser.CurrentRole, null, relationGroupList, parentId));

            var globalJobAssign = jobAssignProvider.Query.First(i => i.IsGlobal && i.JobIdList.Contains(job.Id));
            globalJobAssign.JobIdList.Add(jobId);
            await messageBus.Publish(new ChangeJobAssignJobIdListCommand(globalJobAssign.Id, globalJobAssign.JobIdList));
            var localJobAssign = jobAssignProvider.Query.FirstOrDefault(i => !i.IsGlobal && i.JobIdList.Contains(job.Id) && i.HousingDepartmentIdList.Contains(housingDepartmentId));

            if (localJobAssign != null)
            {
                var newJobAssignId = Guid.NewGuid();
                var command = Mapper.Map(localJobAssign, new CreateJobAssignFromJobAssignCommand(newJobAssignId, localJobAssign.HousingDepartmentIdList, false));
                command.Description = localJobAssign.Description;
                command.JobIdList = new List<string> { jobId };
                await messageBus.Publish(command);

                if (localJobAssign.UploadList != null)
                {
                    foreach (var upload in localJobAssign.UploadList)
                    {
                        string absolutePathPart = HostingEnvironment.MapPath(Constants.Upload.UploadPath);
                        string fullPath = $"{absolutePathPart}\\{localJobAssign.Id}\\{upload.Path.TrimStart('/')}";
                        byte[] bytes = File.ReadAllBytes(fullPath);
                        Guid newUploadId = Guid.NewGuid();
                        await messageBus.Publish(new UploadForTaskInDepartment(newUploadId, bytes, upload.FileName, absolutePathPart, newJobAssignId, upload.UploaderId));
                        await messageBus.Publish(new ChangeDescription(newUploadId, upload.Description));
                    }
                }
            }

            return jobId;
        }

        private string GetNiceId(string jobId)
        {
            var taskPrefixZeroesCount = appSettingHelper.GetAppSetting<int>(Constants.AppSetting.TaskPrefixZeroesCount);

            var index = jobId.IndexOf('-');
            if (index != -1)
            {
                var jobIdentifier = jobId.Substring(0, index);
                var jobNumber = jobId.Substring(index + 1);
                if (jobNumber.Length > taskPrefixZeroesCount)
                {
                    return jobId;
                }

                var niceJobNumber = jobNumber.PadLeft(taskPrefixZeroesCount - jobNumber.Length, '0');
                return $"{jobIdentifier}-{niceJobNumber}";
            }
            else
            {
                return jobId;
            }
        }

        private IEnumerable<IMemberModel> GetAssignedMembers(IGroupModel group, IDayAssign dayAssign)
        {
            IEnumerable<Guid> memberIds = null;
            if (group != null && !dayAssign.UserIdList.HasValue())
            {
                memberIds = group.MemberIds;
            }
            else if (group != null && dayAssign.UserIdList.HasValue())
            {
                memberIds = group.MemberIds.Where(x => dayAssign.UserIdList.Contains(x));
            }
            else
            {
                memberIds = dayAssign.UserIdList;
            }

            if (memberIds.HasValue())
            {
                return memberService.GetAllowedMembersForJobByIds(memberIds);
            }

            var department = managementService.GetHousingDepartment(dayAssign.DepartmentId);
            var members = memberService.GetAllowedMembersForJobByDepartmentId(department.ManagementDepartmentId);
            return members.ToList();
        }

        private IGroupModel GetGroupModel(Guid? groupId)
        {
            return groupId.HasValue ? groupService.Get(groupId.Value) : null;
        }

        private JobAssign GetLocalJobAssign(string jobId, Guid housingDepartmentId)
        {
            JobAssign assign = jobAssignProvider.GetAssignByFilters(jobId, housingDepartmentId);

            return assign != null && assign.IsGlobal ? null : assign;
        }

        private JobAssign GetCorrectJobAssign(JobAssign jobAssign)
        {
            if (jobAssign != null)
            {
                SetCorrectPathForUploadList(jobAssign.UploadList, jobAssign.Id);
            }

            return jobAssign;
        }

        private void SetCorrectPathForUploadList(List<UploadFileModel> uploadList, Guid jobAssignId)
        {
            if (uploadList != null)
            {
                foreach (var upload in uploadList)
                {
                    var extension = Path.GetExtension(upload.Path);
                    upload.Path = pathHelper.GetJobAssignUploadsPath(jobAssignId, upload.FileId, extension);
                }
            }
        }

        private bool IsAllowedCreationGroupedTask(string jobId)
        {
            var job = jobProvider.Query.First(i => i.Id == jobId);
            return !job.IsHidden && !job.RelationGroupList.HasValue() && job.JobTypeId == JobTypeEnum.Facility;
        }

        private List<JobHeaderModel> GetJobHeaderModelList(IEnumerable<IDayAssign> dayAssigns)
        {
            var jobDetails = new List<JobHeaderModel>();
            List<IDayAssign> dayAssignList = dayAssigns.AsList();

            List<Job> jobList = jobProvider.GetByIds(dayAssignList.Select(x => x.JobId), true);
            IDictionary<JobTypeEnum, string> taskDisplayColorDictionary = appSettingHelper.GetFromJson<IDictionary<JobTypeEnum, string>>(Constants.AppSetting.TaskTypesDisplayColors);

            var jobStatusLogDictionary = jobStatusLogService.GetStatusLogModelList(dayAssignList.Select(x => x.Id), false).ToDictionary(x => x.DayAssignId, x => x.CancelingReason);

            foreach (var dayAssign in dayAssignList)
            {
                Job job = jobList.First(x => x.Id == dayAssign.JobId);

                var taskColor = job.Category != null ? job.Category.Color : taskDisplayColorDictionary[job.JobTypeId];

                RelationGroupModel relationModel = job.RelationGroupList.FirstOrDefault(x => x.HousingDepartmentId == dayAssign.DepartmentId);

                jobDetails.Add(new JobHeaderModel
                {
                    Id = dayAssign.Id,
                    Name = job.Id,
                    Date = dayAssign.Date,
                    Address = job.GetAddress(dayAssign.DepartmentId),
                    RelationGroupId = relationModel?.RelationGroupId,
                    Title = job.Title,
                    JobStatus = dayAssign.StatusId,
                    JobType = job.JobTypeId,
                    Color = taskColor,
                    Estimate = dayAssign.EstimatedMinutes,
                    IsUrgent = dayAssign.IsUrgent,
                    AssignedHousingDepartmentName = GetAssignedHousingDepartmentName(dayAssign.DepartmentId),
                    CancellingReason = jobStatusLogDictionary[dayAssign.Id]
                });
            }

            return jobDetails;
        }

        private bool IsParentStillAssigned(string parentJobId, Guid housingDepartmentId)
        {
            IEnumerable<JobAssign> parentJobAssigns = jobAssignProvider.GetAllByJobId(parentJobId);
            return parentJobAssigns.Any(x => x.HousingDepartmentIdList.Contains(housingDepartmentId));
        }

        private bool IsParentHidden(string parentJobId)
        {
            IJob parentJob = GetJobById(parentJobId).Result;
            return parentJob.IsHidden;
        }
    }
}
