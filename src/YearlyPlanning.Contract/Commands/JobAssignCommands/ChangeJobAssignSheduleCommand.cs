using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class ChangeJobAssignSheduleCommand : JobAssignBaseCommand
    {
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }
        public int RepeatsPerWeek { get; set; }
        public ChangedByRole ChangedBy { get; set; }
        public bool IsLocalIntervalChanged { get; set; }

        public ChangeJobAssignSheduleCommand(Guid id, IEnumerable<DayPerWeekModel> dayPerWeekList, int repeatsPerWeek, ChangedByRole changedBy, bool isLocalIntervalChanged) : base(id.ToString())
        {
            DayPerWeekList = dayPerWeekList;
            RepeatsPerWeek = repeatsPerWeek;
            ChangedBy = changedBy;
            IsLocalIntervalChanged = isLocalIntervalChanged;
        }
    }
}
