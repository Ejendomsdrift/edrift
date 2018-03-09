using HistoryCore.Contract.Interfaces;
using HistoryCore.Models;
using Infrastructure.Helpers;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace HistoryCore.Implementation
{
    public class HistoryService : IHistoryService
    {
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IJobService jobService;
        private readonly IPathHelper pathHelper;
        private readonly IMemberService memberService;
        private readonly IDayAssignService dayAssignService;

        public HistoryService(
            IJobStatusLogService jobStatusLogService,
            IJobService jobService,
            IPathHelper pathHelper,
            IMemberService memberService,
            IDayAssignService dayAssignService)
        {
            this.jobStatusLogService = jobStatusLogService;
            this.jobService = jobService;
            this.pathHelper = pathHelper;
            this.memberService = memberService;
            this.dayAssignService = dayAssignService;
        }

        public async Task<IEnumerable<IHistoryModel>> GetChangeStatusHistory(Guid dayAssignId)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            IJob job = await jobService.GetJobById(dayAssign.JobId);

            IEnumerable<IHistoryModel> result = GetChangeStatusHistory(dayAssign, job);
            return result;
        }

        public async Task<IEnumerable<IHistoryModel>> GetCanceledHistory(Guid dayAssignId)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            IJob job = await jobService.GetJobById(dayAssign.JobId);

            IEnumerable<IHistoryModel> result = GetCancelStatusHistory(dayAssign, job);
            return result;
        }

        public IEnumerable<IHistoryModel> GetChangeStatusHistory(string address)
        {
            List<IJob> tenantJobs = jobService.GetTenantTasksByAddress(address).ToList();
            IEnumerable<string> jobIds = tenantJobs.Select(x => x.Id);
            IEnumerable<IDayAssign> dayAssigns = dayAssignService.GetByJobIds(jobIds);
            IEnumerable<IHistoryModel> result = GetChangeStatusHistory(dayAssigns, tenantJobs);
            return result;
        }

        private IEnumerable<IHistoryModel> GetChangeStatusHistory(IEnumerable<IDayAssign> dayAssigns, IEnumerable<IJob> jobs)
        {
            IList<IDayAssign> assigns = dayAssigns as IList<IDayAssign> ?? dayAssigns.ToList();
            IEnumerable<Guid> uniqueDayAssignIds = assigns.GroupBy(d => d.Id).Select(gda => gda.Key);
            IEnumerable<IJobStatusLogModel> jobStatusLogList = jobStatusLogService.GetLogsByDayAssignIds(uniqueDayAssignIds);
            IEnumerable<IHistoryModel> historyList = jobStatusLogList.Select(l => MapHistoryModelFromLog(l, dayAssigns, jobs));
            return historyList.OrderByDescending(hl => hl.ChangeStatusDate);
        }


        private IEnumerable<IHistoryModel> GetChangeStatusHistory(IDayAssign dayAssign, IJob job)
        {
            IEnumerable<IJobStatusLogModel> jobStatusLogList = jobStatusLogService.GetLogsByDayAssignId(dayAssign.Id);
            IEnumerable<IHistoryModel> historyList = jobStatusLogList.Select(l => GetHistoryModel(l, dayAssign, job));

            return historyList.OrderByDescending(hl => hl.ChangeStatusDate);
        }

        private IEnumerable<IHistoryModel> GetCancelStatusHistory(IDayAssign dayAssign, IJob job)
        {
            IEnumerable<JobStatus> statuses = new List<JobStatus>{ JobStatus.Pending, JobStatus.Rejected };
            IEnumerable<IJobStatusLogModel> jobStatusLogList = jobStatusLogService.GetLogsForDayAssignByStatuses(dayAssign.Id, statuses);
            IEnumerable<IHistoryModel> cancelingHistory = jobStatusLogList.Select(l => GetHistoryModel(l, dayAssign, job));

            return cancelingHistory.OrderByDescending(hl => hl.ChangeStatusDate);
        }

        private IHistoryModel MapHistoryModelFromLog(IJobStatusLogModel log, IEnumerable<IDayAssign> dayAssigns, IEnumerable<IJob> jobs)
        {
            IDayAssign dayAssign = dayAssigns.First(t => t.Id == log.DayAssignId);
            IJob job = jobs.First(j => j.Id == dayAssign.JobId);
            return GetHistoryModel(log, dayAssign, job);
        }

        private IHistoryModel GetHistoryModel(IJobStatusLogModel log, IDayAssign dayAssign, IJob job)
        {
            var summarizedReportedTime = GetSummarizedReportedTime(log);
            return new HistoryModel
            {
                JobCreationDate = job.CreationDate,
                JobStatus = log.StatusId,
                ChangeStatusComment = log.Comment,
                ChangeStatusDate = log.Date,
                Title = new string(job.Title.Take(200).ToArray()),
                JobComment = dayAssign.Comment,
                ResidentName = dayAssign.ResidentName,
                DayAssignId = dayAssign.Id,
                JobId = job.Id,
                JobHousingDepartmentId = dayAssign.DepartmentId,
                UserNameWhoChangedStatus = GetMemberName(log.MemberId),
                ReportedHours = summarizedReportedTime.Hours,
                ReportedMinutes = summarizedReportedTime.Minutes,
                Address = job.GetAddress(dayAssign.DepartmentId),
                UploadedFiles = GetUploadedFiles(dayAssign, log),
                CancellationReason = log.CancelingReason
            };
        }

        private List<IFileModel> GetUploadedFiles(IDayAssign dayAssign, IJobStatusLogModel log)
        {
            List<IFileModel> result = new List<IFileModel>();
            if (log.UploadedFileIds != null)
            {
                foreach (Guid uploadedFileId in log.UploadedFileIds)
                {
                    UploadFileModel uploadedFileModel =
                        dayAssign.UploadList.FirstOrDefault(l => l.FileId == uploadedFileId);
                    if (uploadedFileModel != null)
                    {
                        FileModel fileModel = new FileModel
                        {
                            FileName = uploadedFileModel.FileName,
                            FileUrl = GetUploadedFileLink(uploadedFileModel, dayAssign.Id)
                        };
                        result.Add(fileModel);
                    }
                }
            }

            return result;
        }

        private TimeSpan GetSummarizedReportedTime(IJobStatusLogModel log)
        {
            return log.TimeLogList.Aggregate(default(TimeSpan), (acc, timeLog) => acc + timeLog.SpentTime);
        }

        private string GetUploadedFileLink(UploadFileModel model, Guid dayAssignId)
        {
            if (model != null)
            {
                var extension = Path.GetExtension(model.Path);
                return pathHelper.GetDayAssignUploadsPath(dayAssignId, model.FileId, extension);
            }
            return string.Empty;
        }

        private string GetMemberName(Guid memberId)
        {
            IMemberModel member = memberService.GetById(memberId);
            return member.UserName;
        }
    }
}
