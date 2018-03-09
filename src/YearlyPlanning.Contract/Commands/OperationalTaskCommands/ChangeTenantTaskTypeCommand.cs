using System;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeTenantTaskTypeCommand : OperationalTaskCommand
    {
        public TenantTaskTypeEnum Type { get; set; }

        public ChangeTenantTaskTypeCommand(Guid id, TenantTaskTypeEnum type) : base(id.ToString())
        {
            Type = type;
        }
    }
}
