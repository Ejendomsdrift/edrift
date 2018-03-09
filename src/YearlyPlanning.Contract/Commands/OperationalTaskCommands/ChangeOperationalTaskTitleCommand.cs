namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskTitleCommand : OperationalTaskCommand
    {
        public string Title { get; set; }

        public ChangeOperationalTaskTitleCommand(string id, string title) : base(id)
        {
            Title = title;
        }
    }
}
