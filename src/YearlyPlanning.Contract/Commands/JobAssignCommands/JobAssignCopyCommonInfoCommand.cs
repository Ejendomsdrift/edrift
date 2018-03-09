using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobAssignCommands
{
    public class JobAssignCopyCommonInfoCommand: JobAssignBaseCommand
    {
        public int TillYear { get; set; }

        public IEnumerable<WeekModel> WeekList { get; set; }

        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public int RepeatsPerWeek { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public List<Responsible> JobResponsibleList { get; set; }

        public JobAssignCopyCommonInfoCommand(Guid id, 
            int tillYear, 
            IEnumerable<WeekModel> weekList, 
            IEnumerable<DayPerWeekModel> dayPerWeekList,
            int repeatsPerWeek,
            ChangedByRole changedByRole,
            List<Responsible> jobResponsibleList) : base(id.ToString())
        {
            TillYear = tillYear;
            WeekList = weekList;
            DayPerWeekList = dayPerWeekList;
            RepeatsPerWeek = repeatsPerWeek;
            ChangedByRole = changedByRole;
            JobResponsibleList = jobResponsibleList;
        }
    }
}
