using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using StatusCore.JobStatusModels;
using StatusCore.Models;
using YearlyPlanning.Contract.Commands.DayAssignCommands;

namespace StatusCore.Services
{
    public class JobStatusService : IJobStatusService
    {
        private readonly IMessageBus messageBus;
        private readonly IJobStatusLogService jobStatusLogService;

        public JobStatusService(IMessageBus messageBus, IJobStatusLogService jobStatusLogService)
        {
            this.messageBus = messageBus;
            this.jobStatusLogService = jobStatusLogService;
        }

        public async Task Opened(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Opened);
        }

        public async Task Cancel(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Canceled);
        }

        public async Task Cancel(Guid dayAssignId, JobStatus jobStatus, Guid? cancellationReasonId)
        {            
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Canceled, cancelingReasonId: cancellationReasonId);
        }

        public async Task Pending(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Pending);
        }

        public async Task Pending(Guid dayAssignId, JobStatus jobStatus, string changeStatusComment,
            List<IMemberSpentTimeModel> members, Guid? selectedCancelingId, Guid[] uploadedFileIds)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Pending, null, changeStatusComment, members, selectedCancelingId, uploadedFileIds);
        }

        public async Task InProgress(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.InProgress);
        }

        public async Task Paused(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Paused);
        }

        public async Task<IMoveToStatusResultModel> Completed(Guid dayAssignId, JobStatus jobStatus,
            string changeStatusComment, List<IMemberSpentTimeModel> members, Guid[] uploadedFileIds)
        {
            IMoveToStatusResultModel moveToStatusResultModel = new MoveToStatusResultModel();
            try
            {
                moveToStatusResultModel.CurrentStatus = jobStatus.ToString();
                moveToStatusResultModel.ResultStatus = JobStatus.Completed.ToString();
                await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Completed, null, changeStatusComment, members, null, uploadedFileIds);

                moveToStatusResultModel.IsSuccessful = true;
            }
            catch (NotImplementedException)
            {
                moveToStatusResultModel.IsSuccessful = false;
            }

            return moveToStatusResultModel;
        }

        public async Task Assigned(Guid dayAssignId, JobStatus jobStatus)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Assigned);
        }

        public async Task Rejected(Guid dayAssignId, JobStatus jobStatus, string changeStatusComment, List<IMemberSpentTimeModel> members, Guid? selectedCancelingId, Guid[] uploadedFileIds)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Rejected, null, changeStatusComment, members, selectedCancelingId, uploadedFileIds);
        }

        public async Task Expired(Guid dayAssignId, JobStatus jobStatus, Guid? creatorId)
        {
            await MoveJobToStatus(dayAssignId, jobStatus, JobStatus.Expired, creatorId);
        }

        private async Task MoveJobToStatus(Guid dayAssignId, JobStatus jobStatus, JobStatus newStatus, Guid? creatorId = null,
            string changeStatusComment = null, List<IMemberSpentTimeModel> members = null, Guid? cancelingReasonId = null, Guid[] uploadedFileIds = null)
        {
            var currentStatus = GetNextStatus(jobStatus, newStatus);
            await messageBus.Publish(new ChangeDayAssignStatusCommand(dayAssignId.ToString(), currentStatus, creatorId));
            jobStatusLogService.Save(dayAssignId, currentStatus, changeStatusComment, members, cancelingReasonId, uploadedFileIds, creatorId);
        }

        private JobStatus GetNextStatus(JobStatus currentJobStatus, JobStatus stateType)
        {
            var jobStatusContext = new JobStatusStateContext(currentJobStatus);
            jobStatusContext.MoveToNextJobStatus(stateType);
            return jobStatusContext.CurrentState.CurrentStatus;
        }

    }
}
