using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class JobStatusFactory
    {
        public BaseJobStatus Create(JobStatus orderStatus)
        {
            switch (orderStatus)
            {
                case JobStatus.Completed:
                    {
                        return new CompletedJobStatus();

                    }
                case JobStatus.Canceled:
                    {
                        return new CancelJobStatus();
                    }
                case JobStatus.InProgress:
                    {
                        return new InProgressJobStatus();
                    }
                case JobStatus.Opened:
                    {
                        return new OpenedJobStatus();
                    }
                case JobStatus.Paused:
                    {
                        return new PausedJobStatus();
                    }
                case JobStatus.Pending:
                    {
                        return new PendingJobStatus();
                    }
                case JobStatus.Assigned:
                    {
                        return new AssignedJobStatus();
                    }
                case JobStatus.Rejected:
                    {
                        return new RejectedJobStatus();
                    }
                case JobStatus.Expired:
                    {
                        return new ExpiredJobStatus();
                    }
                default:
                    {
                        throw new NotImplementedException("Unsupported order status");
                    }
            }
        }
    }
}
