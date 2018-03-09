using Infrastructure.Extensions;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.Models;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation
{
    public class TaskChartModelBuilder : ITaskChartModelBuilder
    {
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IJobService jobService;

        public TaskChartModelBuilder(IJobStatusLogService jobStatusLogService, IJobService jobService)
        {
            this.jobStatusLogService = jobStatusLogService;
            this.jobService = jobService;

        }

        public IEnumerable<ITaskChartModel> Build(
            IDictionary<Guid, IEnumerable<Guid>> managementToHousingDepartmentsRelation,
            IEnumerable<IDayAssign> dayAssigns,
            IDictionary<Guid, int> categorySortPriority,
            bool showLastCompletedOrCanceledStatus)
        {
            var dayAssignList = dayAssigns
                .AsList();

            var jobsDictionary = jobService
                .GetByIds(dayAssignList.Select(d => d.JobId))
                .ToDictionary(job => job.Id);

            var result = GetTaskChartModelListForTask(
                dayAssignList,
                showLastCompletedOrCanceledStatus,
                managementToHousingDepartmentsRelation,
                jobsDictionary,
                categorySortPriority);
            return result;
        }

        private IEnumerable<ITaskChartModel> GetTaskChartModelListForTask(IEnumerable<IDayAssign> dayAssigns, bool showLastCompletedOrCanceledStatus,
            IDictionary<Guid, IEnumerable<Guid>> managementToHousingDepartmentsRelation, Dictionary<string, IJob> jobsDictionary,
            IDictionary<Guid, int> categorySortPriority)
        {
            var result = new List<ITaskChartModel>();
            var dayAssignList = dayAssigns.AsList();

            Dictionary<Guid, IJobStatusLogModel> logDictionary = jobStatusLogService
                .GetStatusLogModelList(dayAssignList.Select(d => d.Id), showLastCompletedOrCanceledStatus)
                .ToDictionary(log => log.DayAssignId);

            foreach (var dayAssign in dayAssignList)
            {
                IJobStatusLogModel log;
                logDictionary.TryGetValue(dayAssign.Id, out log);

                if(log == null || log.TotalSpentTime == default(decimal))
                {
                    continue;
                }

                IEnumerable<string> cancelingReasons = GetCancelingReasons(log);

                var taskChartModel = GetTaskChartModel(
                    dayAssign,
                    managementToHousingDepartmentsRelation,
                    jobsDictionary[dayAssign.JobId],
                    log.TotalSpentTime, cancelingReasons,
                    categorySortPriority);

                result.Add(taskChartModel);
            }

            return result;
        }

        private ITaskChartModel GetTaskChartModel(IDayAssign dayAssign, IDictionary<Guid, IEnumerable<Guid>> managementToHousingDepartmentsRelation, IJob job,
            decimal spentTime, IEnumerable<string> cancelingReasons, IDictionary<Guid, int> categorySortPriority)
        {
            var managementId = managementToHousingDepartmentsRelation.First(pair => pair.Value.Contains(dayAssign.DepartmentId)).Key;

            var model = new TaskChartModel
            {
                TaskId = dayAssign.Id,
                JobStatus = dayAssign.StatusId,
                HousingDepartmentId = dayAssign.DepartmentId,
                ManagementDepartmentId = managementId,
                JobType = job.JobTypeId,
                SpentTime = spentTime,
                TenantType = dayAssign.TenantType.ToString(),
                IsOverdue = IsTaskOverdue(dayAssign),
                CategoryId = job.CategoryId,
                CategorySortPriority = IsCategoryValid(categorySortPriority, job.CategoryId) ? categorySortPriority[job.CategoryId] : default(int),
                CancelingReasons = cancelingReasons
            };

            return model;
        }

        private bool IsTaskOverdue(IDayAssign dayAssign)
        {
            if (!dayAssign.Date.HasValue)
            {
                return false;
            }
            var dayAssignDate = dayAssign.Date.Value;
            var expectedToOverdueAfter = dayAssignDate.AddDays(1);
            var result = dayAssign.StatusId != JobStatus.Completed && DateTime.UtcNow > expectedToOverdueAfter;
            return result;
        }

        private bool IsCategoryValid(IDictionary<Guid, int> categorySortPriority, Guid categoryId)
        {
            return categorySortPriority.Any() && categoryId != Guid.Empty;
        }

        private IEnumerable<string> GetCancelingReasons(IJobStatusLogModel log)
        {
            string result = log?.CancelingReason ?? string.Empty;

            return new []{ result };
        }

        private IEnumerable<string> GetCancelingReasons(IJobStatusLogModel parentLog, IEnumerable<IJobStatusLogModel> childLogs)
        {
            if (parentLog == null)
            {
                return Enumerable.Empty<string>();
            }

            var logs = new List<IJobStatusLogModel>();
            logs.Add(parentLog);
            logs.AddRange(childLogs);

            IEnumerable<string> cancelingReasons = logs.Where(x => x.CancelingReason != null).Select(x => x.CancelingReason).Distinct();

            return cancelingReasons;
        }
    }
}