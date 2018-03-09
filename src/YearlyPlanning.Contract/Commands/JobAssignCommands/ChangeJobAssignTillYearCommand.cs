using System;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignTillYearCommand : JobAssignBaseCommand
    {
        public int TillYear { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public bool IsLocalIntervalChanged { get; set; }

        public ChangeJobAssignTillYearCommand(Guid id, int tillYear, ChangedByRole changedByRole, bool isLocalIntervalChanged) : base(id.ToString())
        {
            TillYear = tillYear;
            ChangedByRole = changedByRole;
            IsLocalIntervalChanged = isLocalIntervalChanged;
        }
    }
}