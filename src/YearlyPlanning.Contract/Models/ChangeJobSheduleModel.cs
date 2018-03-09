using System.Collections.Generic;

namespace YearlyPlanning.Contract.Models
{
    public class ChangeJobSheduleModel : ChangeJobAssignAbstractModel
    {
        public int RepeatsPerWeek { get; set; }
        public IEnumerable<DayPerWeekModel> DayPerWeekList { get; set; }
    }
}
