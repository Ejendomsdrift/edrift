using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Models
{
    public class AssignAdHockToDayModel: IAssignAdHockToDayModel
    {
        public string TaskId { get; set; }

        public IEnumerable<int> DaysPerWeek { get; set; }

        public int Week { get; set; }

        public int Year { get; set; }
    }
}