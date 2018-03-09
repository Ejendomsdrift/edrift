using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class CreateOperationalTaskCommand : OperationalTaskCommand
    {
        public int Year { get; set; }

        public int Week { get; set; }

        public Guid CategoryId { get; set; }

        public Guid DepartmentId { get; set; }

        public IEnumerable<int> DaysPerWeek { get; set; }

        public string Title { get; set; }

        public IEnumerable<Guid> AssignedEmployees { get; set; }

        public Guid GroupId { get; set; }

        public CreateOperationalTaskCommand(string id, int year, int week, Guid categoryId, Guid departmentId, 
            IEnumerable<int> daysPerWeek, string title, Guid groupId, IEnumerable<Guid> assignedEmployees) : base(id)
        {
            Year = year;
            Week = week;
            CategoryId = categoryId;
            DaysPerWeek = daysPerWeek;
            Title = title;
            GroupId = groupId;
            AssignedEmployees = assignedEmployees;
            DepartmentId = departmentId;
        }
    }
}
