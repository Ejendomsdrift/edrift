using CategoryCore.Contract.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using Statistics.Core.Models;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Interfaces;
using Translations.Models;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation
{
    public class TasksInfoBuilder : ITasksInfoBuilder
    {
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IJobService jobService;
        private readonly ICategoryService categoryService;
        private readonly ITranslationService translationService;
        private readonly IMemberService memberService;
        private readonly IManagementDepartmentService managementDepartmentService;

        public TasksInfoBuilder(
            IJobService jobService,
            IJobStatusLogService jobStatusLogService,
            ICategoryService categoryService,
            ITranslationService translationService,
            IMemberService memberService,
            IManagementDepartmentService managementDepartmentService)
        {
            this.jobService = jobService;
            this.jobStatusLogService = jobStatusLogService;
            this.categoryService = categoryService;
            this.translationService = translationService;
            this.memberService = memberService;
            this.managementDepartmentService = managementDepartmentService;
        }

        public List<TaskInfo> GetTaskInfoList(IEnumerable<IDayAssign> dayAssigns, bool showLastCompletedOrCanceledStatus, bool onlyWithTime = false)
        {
            List<IDayAssign> dayAssignList = dayAssigns.ToList();
            IEnumerable<string> jobIds = dayAssignList.Select(x => x.JobId);
            IEnumerable<Guid> dayAssignIds = dayAssignList.Select(x => x.Id);

            List<IJob> jobList = jobService.GetByIds(jobIds).ToList();
            List<IJobStatusLogModel> jobLogList = jobStatusLogService.GetStatusLogModelList(dayAssignIds, showLastCompletedOrCanceledStatus).ToList();
            IDictionary<Guid, ICategoryModel> categoryDictionary = categoryService.GetAll().ToDictionary(c => c.Id);

            List<TaskInfo> result = GetFilledTaskInfoList(dayAssignList, jobList, jobLogList, categoryDictionary, onlyWithTime);
            return result;
        }

        public IEnumerable<TaskInfo> BuildByTaskProcessedGrouping(IDictionary<string, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus)
        {
            var result = Build(groupedTasks, (taskInfoModel, groupingValue) => taskInfoModel.IsProcessedGroup = groupingValue,
                showLastCompletedOrCanceledStatus);
            return result;
        }

        public IEnumerable<TaskInfo> BuildByCategoryGrouping(IDictionary<Guid, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus)
        {
            var categories = categoryService.GetAll().ToDictionary(c => c.Id, c => c.Name);
            var result = Build(groupedTasks, (taskInfoModel, groupingValue) => taskInfoModel.CategoryName = categories[groupingValue],
                showLastCompletedOrCanceledStatus);
            return result;
        }

        public IEnumerable<TaskInfo> BuildByIsOverdueGrouping(IDictionary<string, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus)
        {
            var result = Build(groupedTasks, (taskInfoModel, groupingValue) => taskInfoModel.IsOverdue = groupingValue,
                showLastCompletedOrCanceledStatus);
            return result;
        }

        public IEnumerable<TaskInfo> BuildByTenantTypeGrouping(IDictionary<string, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus)
        {
            var result = Build(groupedTasks, (taskInfoModel, groupingValue) => taskInfoModel.TenantType = groupingValue,
                showLastCompletedOrCanceledStatus);
            return result;
        }

        private List<TaskInfo> GetFilledTaskInfoList(List<IDayAssign> dayAssignList, List<IJob> jobList, List<IJobStatusLogModel> jobLogList, IDictionary<Guid, ICategoryModel> categoryDictionary, bool onlyWithTime = false)
        {
            var result = new List<TaskInfo>();

            IEnumerable<Guid> creatorIds = jobList.Select(x => x.CreatorId).Distinct();
            IDictionary<Guid, string> creatorNames = memberService.GetMemberNames(creatorIds);

            foreach (var dayAssign in dayAssignList)
            {
                IJob job = jobList.First(x => x.Id == dayAssign.JobId);
                IJobStatusLogModel log = jobLogList.FirstOrDefault(x => x.DayAssignId == dayAssign.Id);
                IJobStatusLogModel cancelLog = jobLogList.FirstOrDefault(x => x.DayAssignId == dayAssign.Id && x.StatusId == JobStatus.Canceled);
                IHousingDepartmentModel assignedHousingDepartment = managementDepartmentService.GetHousingDepartment(dayAssign.DepartmentId);
                string categoryPath = job.CategoryId != Guid.Empty ?
                    categoryService.GetFullPathString(categoryDictionary[job.CategoryId], Constants.Common.CategoryPathDelimeter, categoryDictionary)
                    : string.Empty;
                string address = job.AddressList.Any(x => x.HousingDepartmentId == dayAssign.DepartmentId)
                    ? job.AddressList.Last(x => x.HousingDepartmentId == dayAssign.DepartmentId).Address
                    : string.Empty;

                JobStatus status = log?.StatusId ?? JobStatus.Pending;

                result.Add(new TaskInfo
                {
                    Id = job.Id,
                    CategoryName = categoryPath,
                    Title = job.Title,
                    CompletitionDate = log != null ? GetLogDate(log.StatusId, JobStatus.Completed, log) : string.Empty,
                    CancelingDate = log != null ? GetLogDate(log.StatusId, JobStatus.Canceled, log) : string.Empty,
                    CancelingReason = cancelLog != null ? cancelLog.CancelingReason : string.Empty,
                    StatusName = status.GetStatusLabel(translationService),
                    SpentTime = log?.TotalSpentTime ?? default(decimal),
                    CreatorName = creatorNames[job.CreatorId],
                    TenantType = dayAssign.TenantType.HasValue ? GetTenantTaskTypeTranslationsKey(dayAssign.TenantType.Value) : string.Empty,
                    TenantTypeEnum = dayAssign.TenantType,
                    TaskType = job.JobTypeId.GetTaskTypeLabel(translationService),
                    HousingDepartmentName = $"{assignedHousingDepartment.SyncDepartmentId} {assignedHousingDepartment.Name}",
                    Address = address,
                    OriginalTaskId = job.ParentId
                });
            }

            return result;
        }

        private string GetTenantTaskTypeTranslationsKey(TenantTaskTypeEnum tenantTaskType)
        {
            IDictionary<TenantTaskTypeEnum, string> typeKeys = EnumExtensions.GetAllLocalizationKeys<TenantTaskTypeEnum>();
            IDictionary<string, string> translationList = translationService.Get(typeKeys.Select(i => i.Value), LanguageKey.Default);
            string key = typeKeys[tenantTaskType];
            return translationList[key];
        }

        private IEnumerable<TaskInfo> Build<T>(IDictionary<T, IEnumerable<IDayAssign>> groupedTasks, Action<TaskInfo, T> groupingFiller,
            bool showLastCompletedOrCanceledStatus)
        {
            IEnumerable<TaskInfo> result =
                groupedTasks.SelectMany(pair => BuildTaskInfo(pair.Value, m => groupingFiller(m, pair.Key), showLastCompletedOrCanceledStatus));
            return result;
        }

        private List<TaskInfo> BuildTaskInfo(IEnumerable<IDayAssign> dayAssigns, Action<TaskInfo> groupingFiller, bool showLastCompletedOrCanceledStatus)
        {
            var result = new List<TaskInfo>();
            var assigns = dayAssigns as IList<IDayAssign> ?? dayAssigns.ToList();
            IEnumerable<Guid> dayAssignsIds = assigns.Select(d => d.Id);
            IEnumerable<string> jobIds = assigns.Select(d => d.JobId);

            IDictionary<string, IJob> jobsDictionary = jobService.GetByIds(jobIds).ToDictionary(job => job.Id);

            IDictionary<Guid, IJobStatusLogModel> jobLatestStatusesDictionary = jobStatusLogService
                .GetStatusLogModelList(dayAssignsIds, showLastCompletedOrCanceledStatus)
                .ToDictionary(log => log.DayAssignId);

            foreach (var dayAssign in assigns)
            {
                IJobStatusLogModel log;
                jobLatestStatusesDictionary.TryGetValue(dayAssign.Id, out log);
                TaskInfo taskInfo = GetTaskInfo(dayAssign, groupingFiller, jobsDictionary[dayAssign.JobId], log);
                result.Add(taskInfo);
            }

            return result;
        }

        private TaskInfo GetTaskInfo(IDayAssign dayAssign, Action<TaskInfo> groupingFiller, IJob job, IJobStatusLogModel log)
        {
            JobStatus status = log?.StatusId ?? JobStatus.Pending;

            var model = new TaskInfo
            {
                Id = job.Id,
                Title = job.Title,
                TaskType = job.JobTypeId.GetTaskTypeLabel(translationService),
                StatusName = status.GetStatusLabel(translationService),
                SpentTime = log?.TotalSpentTime ?? default(decimal),
                CompletitionDate = log != null ? GetLogDate(log.StatusId, JobStatus.Completed, log) : string.Empty,
                RejectionDate = log != null ? GetLogDate(log.StatusId, JobStatus.Canceled, log) : string.Empty,
                CreatorName = memberService.GetById(job.CreatorId).Name,
                TenantTypeEnum = dayAssign.TenantType
            };
            groupingFiller(model);
            return model;
        }

        private string GetLogDate(JobStatus currentStatus, JobStatus status, IJobStatusLogModel log)
        {
            return currentStatus == status ? log.Date.ToString(Constants.DateTime.CsvDateFormat) : string.Empty;
        }
    }
}