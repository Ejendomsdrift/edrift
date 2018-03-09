using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeAdHocTaskDayPerWeekCommand : OperationalTaskCommand
    {
        public IEnumerable<int> DaysPerWeek { get; set; }

        public ChangeAdHocTaskDayPerWeekCommand(string id, IEnumerable<int> daysPerWeek) : base(id)
        {
            DaysPerWeek = daysPerWeek;
        }
    }
}
