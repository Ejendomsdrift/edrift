using Infrastructure.Messaging;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public abstract class OperationalTaskCommand : ICommand
    {
        public string Id { get; set; }

        protected OperationalTaskCommand(string id)
        {
            Id = id;
        }
    }
}
