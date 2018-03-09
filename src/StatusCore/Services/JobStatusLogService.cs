using CancellingTemplatesCore.Contract.Interfaces;
using Infrastructure.Extensions;
using MemberCore.Contract.Interfaces;
using MongoRepository.Contract.Interfaces;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using StatusCore.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StatusCore.Services
{
    public class JobStatusLogService : IJobStatusLogService
    {
        private readonly IMemberService memberService;
        private readonly ICancelingTemplatesService cancelingTemplatesService;
        private readonly IRepository<JobStatusLog> jobStatusLogRepository;

        public JobStatusLogService(
            IMemberService memberService,
            ICancelingTemplatesService cancelingTemplatesService,
            IRepository<JobStatusLog> jobStatusLogRepository)
        {
            this.memberService = memberService;
            this.cancelingTemplatesService = cancelingTemplatesService;
            this.jobStatusLogRepository = jobStatusLogRepository;
        }

        public void Save(Guid dayAssignId, JobStatus statusId, string comment, List<IMemberSpentTimeModel> timeLogList, Guid? cancelingReasonId, Guid[] uploadedFileIds, Guid? creatorId)
        {
            IMemberModel currentUser = creatorId == null ? memberService.GetCurrentUser() : memberService.GetById(creatorId.Value);
            IJobStatusLogModel previousStatusLog = GetLatestJobStatusLog(dayAssignId);

            JobStatusLog jobStatusLog = new JobStatusLog
            {
                DayAssignId = dayAssignId,
                MemberId = currentUser.MemberId,
                StatusId = statusId,
                Date = DateTime.Now,
                Comment = comment,
                PreviousStatusId = previousStatusLog?.Id ?? Guid.Empty,
                TimeLogList = timeLogList?.Select(GetMemberTimeModel) ?? new List<TimeLog>(),
                CancellingId = cancelingReasonId,
                UploadedFileIds = uploadedFileIds
            };

            jobStatusLogRepository.Save(jobStatusLog);
        }

        public IEnumerable<Guid> GetDayAssignIds(DateTime? startDate, DateTime? endDate, IEnumerable<JobStatus> statuses)
        {
            Expression<Func<JobStatusLog, bool>> filter = log => 
                statuses.Contains(log.StatusId) &&
                log.TimeLogList.Any(timeLog => timeLog.SpentTime > default(TimeSpan));

            if (startDate.HasValue)
            {
                filter = filter.And(log => startDate <= log.Date);
            }

            if (endDate.HasValue)
            {
                filter = filter.And(log => endDate >= log.Date);
            }

            var logs = jobStatusLogRepository.Query.Where(filter);
            var result = logs.Select(x => x.DayAssignId);

            return result;
        }

        public IJobStatusLog StatusLogForJob(Guid dayAssignId, JobStatus status)
        {
            return jobStatusLogRepository.Query.FirstOrDefault(log => log.DayAssignId == dayAssignId && log.StatusId == status).Map<IJobStatusLog>();
        }

        public IJobStatusLogModel GetLatestJobStatusLog(Guid dayAssignId)
        {
            IEnumerable<JobStatusLog> jobStatusLogList = jobStatusLogRepository.Query.Where(js => js.DayAssignId == dayAssignId);

            JobStatusLog jobStatusLog = jobStatusLogList.OrderByDescending(js => js.Date).FirstOrDefault();

            if (jobStatusLog == null)
            {
                return null;
            }

            var result = jobStatusLog.Map<IJobStatusLogModel>();

            result.CancelingReason = GetCancelingReason(jobStatusLog);

            return result;
        }

        public List<ITimeLogModel> GetUserSpentTime(Guid dayAssignId, IEnumerable<Guid> assignedMemberIds)
        {
            List<ITimeLogModel> result = new List<ITimeLogModel>();
            IEnumerable<JobStatusLog> jobStatusLogList = jobStatusLogRepository.Query.Where(js => js.DayAssignId == dayAssignId);
            IEnumerable<TimeLog> allSpentTimeForUsers = jobStatusLogList.SelectMany(js => js.TimeLogList).ToList();

            foreach (var assignedMemberId in assignedMemberIds)
            {
                TimeLogModel resultTimeModel = new TimeLogModel();
                resultTimeModel.MemberId = assignedMemberId;


                var allSpentUserTimeList = allSpentTimeForUsers.Where(userTime => userTime.MemberId == assignedMemberId);

                foreach (var userTime in allSpentUserTimeList)
                {
                    resultTimeModel.SpentTime += userTime.SpentTime;
                }

                result.Add(resultTimeModel);
            }

            return result;
        }

        public IJobStatusLogModel GetLatestChangeStatusDataAndSummarizedSpentTime(Guid dayAssignId)
        {
            IEnumerable<JobStatusLog> jobStatusLogList = jobStatusLogRepository.Query.Where(js => js.DayAssignId == dayAssignId);

            IEnumerable<TimeLog> allSpentTimeForUsers = jobStatusLogList.SelectMany(js => js.TimeLogList).ToList();

            JobStatusLog jobStatusLog = jobStatusLogList.OrderByDescending(js => js.Date).FirstOrDefault();
            if (jobStatusLog == null)
            {
                return null;
            }

            jobStatusLog.TimeLogList = allSpentTimeForUsers;

            var result = jobStatusLog.Map<IJobStatusLogModel>();

            result.CancelingReason = GetCancelingReason(jobStatusLog);

            return result;
        }

        public IEnumerable<IJobStatusLogModel> GetStatusLogModelList(IEnumerable<Guid> dayAssignIds, bool showLastCompletedOrCanceledStatus)
        {
            var dayAssignIdList = dayAssignIds.AsList();

            if (!dayAssignIdList.HasValue())
            {
                return Enumerable.Empty<IJobStatusLogModel>();
            }

            var jobStatusLogModelList = new ConcurrentBag<IJobStatusLogModel>();

            List<JobStatusLog> jobStatusLogList = GetJobStatusLogList(dayAssignIdList);
            List<IGrouping<Guid, JobStatusLog>> jobStatuses = jobStatusLogList.GroupBy(i => i.DayAssignId).ToList();
            IEnumerable<Guid> cancelReasonIds = jobStatusLogList.Where(i => i.CancellingId.HasValue).Select(i => i.CancellingId.Value);
            List<ICancelingTemplateModel> cancelReasons = cancelingTemplatesService.GetByIds(cancelReasonIds).ToList();

            Parallel.ForEach(jobStatuses, dayAssignStatuses =>
            {
                JobStatusLog jobStatusLog = GetJobStatusLog(dayAssignStatuses, showLastCompletedOrCanceledStatus);

                if (jobStatusLog == null)
                {
                    return;
                }

                jobStatusLog.TimeLogList = dayAssignStatuses.SelectMany(js => js.TimeLogList).ToList();

                var model = jobStatusLog.Map<IJobStatusLogModel>();
                model.IsCommentExistInAnyStatus = dayAssignStatuses.Any(y => y.Comment.IsNotNullOrEmpty());

                if (jobStatusLog.CancellingId.HasValue)
                {
                    model.CancelingReason = cancelReasons.FirstOrDefault(i => i.Id == jobStatusLog.CancellingId)?.Text;
                }

                jobStatusLogModelList.Add(model);
            });

            return jobStatusLogModelList;
        }

        public IEnumerable<IJobStatusLogModel> GetLogsByDayAssignId(Guid dayAssignId)
        {
            return GetLogsByDayAssignIds(new[] { dayAssignId });
        }

        public IEnumerable<IJobStatusLogModel> GetLogsByDayAssignIds(IEnumerable<Guid> dayAssignIds)
        {
            var result = new List<IJobStatusLogModel>();
            IEnumerable<JobStatusLog> jobStatusLogs = jobStatusLogRepository.Query.Where(js => dayAssignIds.Contains(js.DayAssignId)).ToList();

            List<JobStatusLog> cancelingLogList = jobStatusLogs.Where(x => x.CancellingId.HasValue).ToList();
            Dictionary<Guid, string> reasons = GetCancelingReasons(cancelingLogList);

            foreach (var statusLog in jobStatusLogs)
            {
                var mappedModel = statusLog.Map<IJobStatusLogModel>();
                mappedModel.CancelingReason = statusLog.CancellingId.HasValue ? reasons[statusLog.Id] : string.Empty;
                mappedModel.CancelingId = statusLog.CancellingId;
                result.Add(mappedModel);
            }

            return result;
        }

        public IEnumerable<IJobStatusLogModel> GetLogsForDayAssignByStatuses(Guid dayAssignId, IEnumerable<JobStatus> statuses)
        {
            List<IJobStatusLogModel> result = new List<IJobStatusLogModel>();
            IEnumerable<JobStatusLog> jobStatusLogList = jobStatusLogRepository.Query.Where(js => js.DayAssignId == dayAssignId && statuses.Contains(js.StatusId)).ToList();

            foreach (var statusLog in jobStatusLogList)
            {
                var mappedModel = statusLog.Map<IJobStatusLogModel>();
                mappedModel.CancelingReason = GetCancelingReason(statusLog);
                result.Add(mappedModel);
            }

            return result;
        }

        private List<JobStatusLog> GetJobStatusLogList(IEnumerable<Guid> dayAssignIds)
        {
            List<JobStatusLog> jobStatusLogList = jobStatusLogRepository.Query
                    .Where(js => dayAssignIds.Contains(js.DayAssignId))
                    .OrderByDescending(i => i.Date)
                    .ToList();

            return jobStatusLogList;
        }

        private JobStatusLog GetJobStatusLog(IGrouping<Guid, JobStatusLog> dayAssignStatuses, bool showLastCompletedOrCanceledStatus)
        {
            if (showLastCompletedOrCanceledStatus)
            {
                return dayAssignStatuses.FirstOrDefault(x => x.StatusId == JobStatus.Completed || x.StatusId == JobStatus.Canceled);
            }

            return dayAssignStatuses.FirstOrDefault();
        }

        private TimeLog GetMemberTimeModel(IMemberSpentTimeModel memberSpentTime)
        {
            TimeSpan hours = TimeSpan.FromHours(memberSpentTime.SpentHours);
            TimeSpan minutes = TimeSpan.FromMinutes(memberSpentTime.SpentMinutes);
            return new TimeLog
            {
                MemberId = memberSpentTime.MemberId,
                SpentTime = hours.Add(minutes)
            };
        }

        private string GetCancelingReason(JobStatusLog jobStatusLog)
        {
            if (!jobStatusLog.CancellingId.HasValue)
            {
                return null;
            }

            var cancelingTemplate = cancelingTemplatesService.GetById(jobStatusLog.CancellingId.Value);
            return cancelingTemplate?.Text;
        }

        private Dictionary<Guid, string> GetCancelingReasons(List<JobStatusLog> logs)
        {
            var result = new Dictionary<Guid, string>();
            IEnumerable<Guid> cancelingIds = logs.Select(x => x.CancellingId.Value).Distinct();
            List<ICancelingTemplateModel> templates = cancelingTemplatesService.GetByIds(cancelingIds).ToList();

            foreach (var log in logs)
            {
                ICancelingTemplateModel template = templates.FirstOrDefault(t => t.Id == log.CancellingId);

                result.Add(log.Id, template?.Text);
            }

            return result;
        }
    }
}
