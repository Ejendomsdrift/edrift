namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public class ChangeJobTitleCommand : JobCommand
    {
        public string Title { get; set; }

        public ChangeJobTitleCommand(string id, string title) : base(id)
        {
            Title = title;
        }
    }
}