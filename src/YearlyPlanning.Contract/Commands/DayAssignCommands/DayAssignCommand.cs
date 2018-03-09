using Infrastructure.Messaging;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public abstract class DayAssignCommand : ICommand
    {
        public string Id { get; set; }
        protected DayAssignCommand() { }
        protected DayAssignCommand(string id)
        {
            Id = id;
        }
    }
}