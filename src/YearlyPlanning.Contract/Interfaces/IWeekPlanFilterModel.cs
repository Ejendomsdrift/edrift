using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IWeekPlanFilterModel
    {
        Guid? HousingDepartmentId { get; set; }
        int Week { get; set; }
        int Year { get; set; }
        JobStateType JobState { get; set; }
        IEnumerable<Guid> MemberIds { get; set; }

        IWeekPlanFilterModel Clone();
    }
}
