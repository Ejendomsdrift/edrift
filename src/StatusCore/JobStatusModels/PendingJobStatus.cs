using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class PendingJobStatus : BaseJobStatus
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
                case JobStatus.Opened:
                    {
                        context.CurrentState = new OpenedJobStatus();
                        break;
                    }
                case JobStatus.Canceled:
                    {
                        context.CurrentState = new CancelJobStatus();
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
