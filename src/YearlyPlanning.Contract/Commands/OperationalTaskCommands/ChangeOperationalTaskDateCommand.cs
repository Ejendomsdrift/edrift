using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskDateCommand : OperationalTaskCommand
    {
        public DateTime Date { get; set; }
        public int WeekDay { get; set; }
        public int Year { get; set; }

        public ChangeOperationalTaskDateCommand(Guid id, DateTime date, int weekDay) : base(id.ToString())
        {
            Date = date;
            WeekDay = weekDay;
            Year = date.Year;
        }
    }
}
