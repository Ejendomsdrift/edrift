using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class CreateJobAssignFromJobAssignCommand : JobAssignBaseCommand
    {
        public List<Guid> HousingDepartmentIdList { get; private set; }

        public string Description { get; set; }

        public int TillYear { get; set; }

        public int RepeatsPerWeek { get; set; }

        public bool IsLocked { get; set; }

        public RoleType CreatedByRole { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public IEnumerable<WeekModel> WeekList { get; set; }

        public List<UploadFileModel> UploadList { get; set; }

        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public List<string> JobIdList { get; set; }

        public bool IsGlobal { get; set; }

        public bool IsEnabled { get; set; }

        public bool RewriteChangedByWeeks { get; set; }

        public List<Responsible> JobResponsibleLIst { get; set; }

        public CreateJobAssignFromJobAssignCommand() { }

        public CreateJobAssignFromJobAssignCommand(Guid id, List<Guid> departmentIds, bool rewriteChangedByWeeks = true) : base(id.ToString())
        {
            HousingDepartmentIdList = departmentIds;
            RewriteChangedByWeeks = rewriteChangedByWeeks;
        }
    }
}
