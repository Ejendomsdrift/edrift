using Infrastructure.Messaging;

namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public abstract class JobCommand : ICommand
    {
        public string Id { get; set; }

        protected JobCommand(string id)
        {
            Id = id;
        }
    }
}