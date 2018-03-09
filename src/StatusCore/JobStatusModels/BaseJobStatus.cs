using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public abstract class BaseJobStatus
    {
        public JobStatus CurrentStatus { get; set; }

        public abstract void ChangeOrderStatus(IJobStatusStateContext context, Contract.Enums.JobStatus status);
    }
}
