using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class RejectedJobStatus: BaseJobStatus
    {
        public override void ChangeOrderStatus(IJobStatusStateContext context, JobStatus status)
        {
            switch (status)
            {
                case JobStatus.Assigned:
                    {
                        context.CurrentState = new AssignedJobStatus();
                        break;
                    }
                case JobStatus.Canceled:
                    {
                        context.CurrentState = new CancelJobStatus();
                        break;
                    }
                case JobStatus.InProgress:
                    {
                        context.CurrentState = new InProgressJobStatus();
                        break;
                    }
                case JobStatus.Pending:
                    {
                        context.CurrentState = new PendingJobStatus();
                        break;
                    }
                case JobStatus.Opened:
                    {
                        context.CurrentState = new OpenedJobStatus();
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
