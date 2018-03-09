﻿using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class InProgressJobStatus: BaseJobStatus
    {
        public override void ChangeOrderStatus(IJobStatusStateContext context, JobStatus status)
        {
            switch (status)
            {
                case JobStatus.Rejected:
                    {
                        context.CurrentState = new RejectedJobStatus();
                        break;
                    }
                case JobStatus.Canceled:
                    {
                        context.CurrentState = new CancelJobStatus();
                        break;
                    }
                case JobStatus.Paused:
                    {
                        context.CurrentState = new PausedJobStatus();
                        break;
                    }
                case JobStatus.Completed:
                    {
                        context.CurrentState = new CompletedJobStatus();
                        break;
                    }
                case JobStatus.Pending:
                    {
                        context.CurrentState = new PendingJobStatus();
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
