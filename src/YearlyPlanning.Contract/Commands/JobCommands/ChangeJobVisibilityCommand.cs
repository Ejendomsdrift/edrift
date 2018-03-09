namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public class ChangeJobVisibilityCommand: JobCommand
    {
        public bool IsHidden { get; set; }
        public ChangeJobVisibilityCommand(string id, bool isHidden): base(id)
        {
            IsHidden = isHidden;
        }
    }
}
