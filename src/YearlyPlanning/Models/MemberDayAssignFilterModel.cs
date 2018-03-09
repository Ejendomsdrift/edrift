using System;
using System.Collections.Generic;

namespace YearlyPlanning.Models
{
    public class MemberDayAssignFilterModel
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public int? Day { get; set; }
        public bool WithEstimatedMinutes { get; set; }
        public bool WithDate { get; set; }
        public IEnumerable<Guid> HousingDepartmentIds { get; set; }
    }
}
