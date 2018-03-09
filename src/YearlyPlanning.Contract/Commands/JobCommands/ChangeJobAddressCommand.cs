using System;

namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public class ChangeJobAddressCommand: JobCommand
    {
        public string Address { get; set; }

        public Guid HousingDepartmentId { get; set; }

        public ChangeJobAddressCommand(string id, Guid housingDepartmentId, string address): base(id)
        {
            Address = address;
            HousingDepartmentId = housingDepartmentId;
        }
    }
}
