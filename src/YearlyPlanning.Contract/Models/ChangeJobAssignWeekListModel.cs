using System.Collections.Generic;

namespace YearlyPlanning.Contract.Models
{
    public class ChangeJobAssignWeekListModel : ChangeJobAssignAbstractModel
    {
        public IEnumerable<WeekModel> WeekList { get; set; }
    }
}
