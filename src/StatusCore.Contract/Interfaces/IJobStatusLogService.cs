using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace StatusCore.Contract.Interfaces
{
    public interface IJobStatusLogService
    {
        void Save(Guid dayAssignId, JobStatus statusId, string comment, List<IMemberSpentTimeModel> timeLogList, Guid? cancelingReasonId, Guid[] uploadedFileIds, Guid? creatorId);
        IJobStatusLogModel GetLatestJobStatusLog(Guid dayAssignId);
        List<ITimeLogModel> GetUserSpentTime(Guid dayAssignId, IEnumerable<Guid> assignedMemberIds);
        IJobStatusLogModel GetLatestChangeStatusDataAndSummarizedSpentTime(Guid dayAssignId);
        IEnumerable<Guid> GetDayAssignIds(DateTime? startDate, DateTime? endDate, IEnumerable<JobStatus> statuses);
        IEnumerable<IJobStatusLogModel> GetLogsByDayAssignId(Guid dayAssignId);
        IEnumerable<IJobStatusLogModel> GetLogsByDayAssignIds(IEnumerable<Guid> dayAssignIds);
        IJobStatusLog StatusLogForJob(Guid dayAssignId, JobStatus status);
        IEnumerable<IJobStatusLogModel> GetStatusLogModelList(IEnumerable<Guid> dayAssignIds, bool showLastCompletedOrCanceledStatus);
        IEnumerable<IJobStatusLogModel> GetLogsForDayAssignByStatuses(Guid dayAssignId, IEnumerable<JobStatus> statuses);
    }
}