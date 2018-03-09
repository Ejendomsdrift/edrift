using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class SaveDaysPerWeekCommand : JobAssignBaseCommand
    {
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public SaveDaysPerWeekCommand(Guid id, IEnumerable<DayPerWeekModel> dayPerWeekList) : base(id.ToString())
        {
            DayPerWeekList = dayPerWeekList;
            ChangedByRole = ChangedByRole.None;
        }

        public SaveDaysPerWeekCommand(Guid id, IEnumerable<DayPerWeekModel> dayPerWeekList, ChangedByRole changedByRole) : base(id.ToString())
        {
            DayPerWeekList = dayPerWeekList;
            ChangedByRole = changedByRole;
        }
    }
}
