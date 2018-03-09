using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignJobIdListCommand : JobAssignBaseCommand
    {
        public List<string> JobIdList { get; set; }

        public ChangeJobAssignJobIdListCommand(Guid id, List<string> jobIds) : base(id.ToString())
        {
            JobIdList = jobIds;
        }
    }
}
