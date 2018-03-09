using System;
using StatusCore.Contract.Enums;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class ChangeDayAssignStatusCommand : DayAssignCommand
    {
        public JobStatus Status { get; set; }
        public Guid? CreatorId { get; set; }

        public ChangeDayAssignStatusCommand(string id, JobStatus status, Guid? creatorId) : base(id)
        {
            Status = status;
            CreatorId = creatorId;
        }
    }
}
