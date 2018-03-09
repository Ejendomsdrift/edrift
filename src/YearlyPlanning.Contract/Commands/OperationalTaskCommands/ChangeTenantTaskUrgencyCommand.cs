using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeTenantTaskUrgencyCommand : OperationalTaskCommand
    {
        public bool IsUrgent { get; set; }

        public ChangeTenantTaskUrgencyCommand(Guid id, bool isUrgent) : base(id.ToString())
        {
            IsUrgent = isUrgent;
        }
    }
}
