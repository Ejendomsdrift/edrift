using System;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeLockIntervalValueCommand : JobAssignBaseCommand
    {
        public bool IsLocked { get; set; }

        public ChangeLockIntervalValueCommand(Guid id, bool isLocked) : base(id.ToString())
        {
            IsLocked = isLocked;
        }
    }
}
