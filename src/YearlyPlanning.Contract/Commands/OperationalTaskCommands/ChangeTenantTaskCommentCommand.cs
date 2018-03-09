using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeTenantTaskCommentCommand : OperationalTaskCommand
    {
        public string Comment { get; set; }

        public ChangeTenantTaskCommentCommand(Guid id, string comment) : base(id.ToString())
        {
            Comment = comment;
        }
    }
}
