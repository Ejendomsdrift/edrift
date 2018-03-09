using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class UnassignOperationalTaskEmployeesCommand : OperationalTaskCommand
    {
        public string TaskId { get; set; }

        public Guid GroupId { get; set; }

        public IEnumerable<Guid> Employees { get; set; }

        public UnassignOperationalTaskEmployeesCommand(string id, Guid groupId, IEnumerable<Guid> employees) : base(id)
        {
            GroupId = groupId;
            Employees = employees;
        }
    }
}
