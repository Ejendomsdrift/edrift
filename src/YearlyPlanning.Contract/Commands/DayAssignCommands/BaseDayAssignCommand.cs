using Infrastructure.Messaging;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class BaseDayAssignCommand: ICommand
    {
        public string Id { get; set; }

        protected BaseDayAssignCommand(string id)
        {
            Id = id;
        }
    }
}
