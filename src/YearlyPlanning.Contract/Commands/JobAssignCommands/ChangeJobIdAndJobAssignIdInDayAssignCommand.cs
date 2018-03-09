using System;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobIdAndJobAssignIdInDayAssignCommand : JobAssignBaseCommand
    {
        public Guid JobAssignId { get; set; }
        public Guid DepartmentId { get; set; }
        public string NewJobId { get; set; }

        public ChangeJobIdAndJobAssignIdInDayAssignCommand(string jobId, Guid departmentId, Guid jobAssignId, string newJobId = null) : base(jobId)
        {
            JobAssignId = jobAssignId;
            DepartmentId = departmentId;
            NewJobId = newJobId ?? jobId;
        }
    }
}
