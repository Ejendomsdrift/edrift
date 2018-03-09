using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignWeeksCommand : JobAssignBaseCommand
    {
        public IEnumerable<WeekModel> WeekList { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public bool IsLocalIntervalChanged { get; set; }

        public ChangeJobAssignWeeksCommand(Guid id, IEnumerable<WeekModel> weekList, ChangedByRole changedByRole, bool isLocalIntervalChanged) : base(id.ToString())
        {
            WeekList = weekList;
            ChangedByRole = changedByRole;
            IsLocalIntervalChanged = isLocalIntervalChanged;
        }
    }
}
