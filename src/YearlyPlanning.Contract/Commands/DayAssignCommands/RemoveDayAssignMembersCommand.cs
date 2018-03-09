using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class RemoveDayAssignMembersCommand: DayAssignCommand
    {
        public string TaskId { get; set; }

        public List<Guid> UserIdList { get; set; }

        public RemoveDayAssignMembersCommand(string id, List<Guid> userIdList) : base(id)
        {
            UserIdList = userIdList;
        }
    }
}
