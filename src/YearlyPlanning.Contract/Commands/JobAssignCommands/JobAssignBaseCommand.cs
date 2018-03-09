using Infrastructure.Messaging;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class JobAssignBaseCommand : ICommand
    {
        public string Id { get; set; }

        protected JobAssignBaseCommand(string id)
        {
            Id = id;
        }

        protected JobAssignBaseCommand()
        {
        }
    }
}
