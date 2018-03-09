using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StatusCore.Contract.Enums;

namespace StatusCore.Contract.Interfaces
{
    public interface IJobStatusService
    {
        Task Opened(Guid dayAssignId, JobStatus jobStatus);

        Task Cancel(Guid dayAssignId, JobStatus jobStatus);

        Task Cancel(Guid dayAssignId, JobStatus jobStatus, Guid? cancellationReasonId);

        Task Pending(Guid dayAssignId, JobStatus jobStatus);

        Task Pending(Guid dayAssignId, JobStatus jobStatus, string changeStatusComment, List<IMemberSpentTimeModel> members, Guid? selectedCancellingId, Guid[] uploadedFileIds);

        Task InProgress(Guid dayAssignId, JobStatus jobStatus);

        Task Paused(Guid dayAssignId, JobStatus jobStatus);

        Task<IMoveToStatusResultModel> Completed(Guid dayAssignId, JobStatus jobStatus, string changeStatusComment, List<IMemberSpentTimeModel> members, Guid[] uploadedFileIds);

        Task Assigned(Guid dayAssignId, JobStatus jobStatus);

        Task Rejected(Guid dayAssignId, JobStatus jobStatus, string changeStatusComment, List<IMemberSpentTimeModel> members, Guid? selectedCancellingId, Guid[] uploadedFileIds);

        Task Expired(Guid dayAssignId, JobStatus jobStatus, Guid? creatorrId);
    }
}
