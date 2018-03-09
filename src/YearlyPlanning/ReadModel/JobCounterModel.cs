using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.ReadModel
{
    public class JobCounterModel : IJobCounterModel
    {
        public int JanitorTasksCount { get; set; }
        public int OpenedTasksCount { get; set; }
    }
}
