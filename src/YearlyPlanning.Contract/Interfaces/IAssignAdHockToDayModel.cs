using System.Collections.Generic;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IAssignAdHockToDayModel
    {
        string TaskId { get; set; }

        IEnumerable<int> DaysPerWeek { get; set; }

        int Week { get; set; }

        int Year { get; set; }
    }
}
