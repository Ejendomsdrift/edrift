using System;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignDescriptionCommand : JobAssignBaseCommand
    {
        public string Description { get; set; }

        public ChangeJobAssignDescriptionCommand(Guid id, string description) : base(id.ToString())
        {
            Description = description;
        }
    }
}