namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJobCounterModel
    {
        int JanitorTasksCount { get; set; }
        int OpenedTasksCount { get; set; }
    }
}