using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeOperationalTaskAssignsEmployeesCommand : OperationalTaskCommand
    {
        public Guid GroupId { get; set; }

        public IEnumerable<Guid> AssignedEmployees { get; set; }

        public ChangeOperationalTaskAssignsEmployeesCommand(string id, Guid groupId, IEnumerable<Guid> assignedEmployees) : base(id)
        {
            GroupId = groupId;
            AssignedEmployees = assignedEmployees;
        }
    }
}
