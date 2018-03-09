namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskDescriptionCommand : OperationalTaskCommand
    {
        public string Description { get; set; }

        public ChangeOperationalTaskDescriptionCommand(string id, string description) : base(id)
        {
            Description = description;
        }
    }
}
