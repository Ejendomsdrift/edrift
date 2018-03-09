using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class CompletedJobStatus: BaseJobStatus
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
                default:
                    {
                        throw new NotImplementedException("Not allowed status");
                    }
            }
        }
    }
}
