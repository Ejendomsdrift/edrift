using System;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class AssignJobCommand : JobAssignBaseCommand
    {
        public Guid HousingDepartmentId { get; set; }

        public AssignJobCommand(Guid id, Guid housingDepartmentId) : base(id.ToString())
        {
            HousingDepartmentId = housingDepartmentId;
        }
    }
}