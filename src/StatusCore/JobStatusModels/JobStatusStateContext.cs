using StatusCore.Contract.Enums;

namespace StatusCore.JobStatusModels
{
    public class JobStatusStateContext: IJobStatusStateContext
    {
        public BaseJobStatus CurrentState { get; set; }

        public JobStatusStateContext(JobStatus jobStatus)
        {
            JobStatusFactory factory = new JobStatusFactory();            
            CurrentState = factory.Create(jobStatus);
        }

        public void MoveToNextJobStatus(JobStatus destinationStatus)
        {
            CurrentState.ChangeOrderStatus(this, destinationStatus);
            CurrentState.CurrentStatus = destinationStatus;
        }
    }
}
