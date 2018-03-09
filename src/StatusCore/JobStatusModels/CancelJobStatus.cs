using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class CancelJobStatus: BaseJobStatus
    {
        public override void ChangeOrderStatus(IJobStatusStateContext context, JobStatus status)
        {
            throw new NotImplementedException("Not allowed status");
        }
    }
}
