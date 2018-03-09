namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class ChangeDayAssignEstimatedMinutesCommand : DayAssignCommand
    {
        public int EstimatedMinutes { get; set; }


        public ChangeDayAssignEstimatedMinutesCommand() { }
        public ChangeDayAssignEstimatedMinutesCommand(string id) : base(id)
        {
        }
    }
}