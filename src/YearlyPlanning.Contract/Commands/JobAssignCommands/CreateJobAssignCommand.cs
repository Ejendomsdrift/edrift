using System;
using MemberCore.Contract.Enums;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class CreateJobAssignCommand: JobAssignBaseCommand
    {
        public string JobId { get; set; }

        public RoleType CreatedByRole { get; set; }

        public int TillYear { get; set; }

        public CreateJobAssignCommand(Guid id, string jobId, RoleType createdByRole, int tillYear) : base(id.ToString())
        {
            JobId = jobId;
            CreatedByRole = createdByRole;
            TillYear = tillYear;
        }

        public CreateJobAssignCommand()
        {
        }
    }
}
