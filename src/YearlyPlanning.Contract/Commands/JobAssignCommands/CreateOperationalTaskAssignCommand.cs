using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class CreateOperationalTaskAssignCommand: JobAssignBaseCommand
    {
        public int TillYear { get; set; }
        public IEnumerable<WeekModel> WeekList { get; set; }
        public Guid DepartmentId { get; set; }
        public int RepeatsPerWeek { get; set; }
        public string Description { get; set; }
        public List<string> JobIdList { get; set; } = new List<string>();
        public bool IsEnabled { get; set; }
        public RoleType CreatedByRole { get; set; }

        public CreateOperationalTaskAssignCommand(Guid id, List<string> jobIds, RoleType createdByRole, int weekNumber) : base(id.ToString())
        {
            JobIdList = jobIds;
            CreatedByRole = createdByRole;
            WeekList = new List<WeekModel> {new WeekModel {Number = weekNumber, ChangedBy = WeekChangedBy.Coordinator}};
        }

        public CreateOperationalTaskAssignCommand() { }
                   
    }
}
