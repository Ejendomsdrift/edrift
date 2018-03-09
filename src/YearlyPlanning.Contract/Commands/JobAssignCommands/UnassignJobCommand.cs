using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class UnassignJobCommand : JobAssignBaseCommand
    {
        public IEnumerable<Guid> HousingDepartmentIds { get; set; }

        public UnassignJobCommand(Guid id, IEnumerable<Guid> housingDepartmentIds) : base(id.ToString())
        {
            HousingDepartmentIds = housingDepartmentIds;
        }
    }
}
