namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskEstimateCommand : OperationalTaskCommand
    {
        public decimal Estimate { get; set; }

        public ChangeOperationalTaskEstimateCommand(string id, decimal estimate) : base(id)
        {
            Estimate = estimate;
        }
    }
}
