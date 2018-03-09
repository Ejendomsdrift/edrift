using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class AssignedJobStatus : BaseJobStatus
    {
        public override void ChangeOrderStatus(IJobStatusStateContext context, JobStatus status)
        {
            switch (status)
            {
                case JobStatus.Opened:
                    {
                        context.CurrentState = new OpenedJobStatus();
                        break;
                    }
                case JobStatus.Rejected:
                    {
                        context.CurrentState = new RejectedJobStatus();
                        break;
                    }
                case JobStatus.InProgress:
                    {
                        context.CurrentState = new CancelJobStatus();
                        break;
                    }
                case JobStatus.Completed:
                    {
                        context.CurrentState = new CompletedJobStatus();
                        break;
                    }
                case JobStatus.Canceled:
                    {
                        context.CurrentState = new CancelJobStatus();
                        break;
                    }
                case JobStatus.Pending:
                    {
                        context.CurrentState = new PendingJobStatus();
                        break;
                    }
                case JobStatus.Expired:
                    {
                        context.CurrentState = new ExpiredJobStatus();
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("Not allowed status");
                    }
            }
        }
    }
}
