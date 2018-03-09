using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class TaskDataFilterModel : ITaskDataFilterModel
    {
        public int? Week { get; set; }

        public int? BiggerThanWeek { get; set; }

        public int? StartWeek { get; set; }

        public int? Year { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<Guid> MemberIds { get; set; }

        public IEnumerable<Guid> HousingDepartments { get; set; }

        public JobStateType? JobState { get; set; }

        public IEnumerable<JobStatus> JobStatuses { get; set; }
    }
}
