using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeTenantTaskResidentPhoneCommand : OperationalTaskCommand
    {
        public string ResidentPhone { get; set; }

        public ChangeTenantTaskResidentPhoneCommand(Guid id, string residentPhone) : base(id.ToString())
        {
            ResidentPhone = residentPhone;
        }
    }
}
