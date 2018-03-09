using System;
using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class ExpiredJobStatus : BaseJobStatus
    {
        public override void ChangeOrderStatus(IJobStatusStateContext context, JobStatus status)
        {
            switch (status)
            {
                default:
                    {
                        throw new NotImplementedException("Not allowed status");
                    }
            }
        }
    }
}
