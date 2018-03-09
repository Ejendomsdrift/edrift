using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignAllWeeksCommand : JobAssignBaseCommand
    {
        public IEnumerable<WeekModel> WeekList { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public ChangeJobAssignAllWeeksCommand(Guid id, ChangedByRole changedByRole, IEnumerable<WeekModel> weekList) : base(id.ToString())
        {
            WeekList = weekList;
            ChangedByRole = changedByRole;
        }
    }
}
