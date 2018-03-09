using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeTenantTaskResidentNameCommand : OperationalTaskCommand
    {
        public string ResidentName { get; set; }

        public ChangeTenantTaskResidentNameCommand(Guid id, string residentName) : base(id.ToString())
        {
            ResidentName = residentName;
        }
    }
}
