using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignSaveDaysPerWeekEvent : EventBase
    {
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }

        public ChangedByRole ChangedByRole { get; set; }
    }
}
