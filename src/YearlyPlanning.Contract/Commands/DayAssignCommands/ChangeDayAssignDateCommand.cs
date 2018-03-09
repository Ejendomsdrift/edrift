using System;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class ChangeDayAssignDateCommand : DayAssignCommand
    {
        public DateTime? Date { get; set; }

        public int? WeekDay { get; set; }

        public ChangeDayAssignDateCommand() { }
        public ChangeDayAssignDateCommand(string id) : base(id)
        {
        }
    }
}