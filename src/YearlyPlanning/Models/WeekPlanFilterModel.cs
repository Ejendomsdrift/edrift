using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class WeekPlanFilterModel : IWeekPlanFilterModel
    {
        public Guid? HousingDepartmentId { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public JobStateType JobState { get; set; }
        public IEnumerable<Guid> MemberIds { get; set; }

        public IWeekPlanFilterModel Clone()
        {
            return (WeekPlanFilterModel)MemberwiseClone();
        }
    }
}
