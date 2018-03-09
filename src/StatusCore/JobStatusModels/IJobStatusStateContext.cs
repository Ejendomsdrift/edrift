using StatusCore.Contract.Enums;


namespace StatusCore.JobStatusModels
{
    public interface IJobStatusStateContext
    {
        BaseJobStatus CurrentState { get; set; }

        void MoveToNextJobStatus(JobStatus destinationStatus);
    }
}
