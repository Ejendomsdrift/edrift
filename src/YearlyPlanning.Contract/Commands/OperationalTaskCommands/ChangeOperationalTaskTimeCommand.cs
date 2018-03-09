using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskTimeCommand : OperationalTaskCommand
    {
        public DateTime Time { get; set; }

        public ChangeOperationalTaskTimeCommand(Guid id, DateTime time) : base(id.ToString())
        {
            Time = time;
        }
    }
}
