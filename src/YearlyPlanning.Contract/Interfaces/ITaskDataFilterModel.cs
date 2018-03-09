using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface ITaskDataFilterModel
    {
        int? Week { get; set; }

        int? BiggerThanWeek { get; set; }

        int? StartWeek { get; set; }

        int? Year { get; set; }

        DateTime? EndDate { get; set; }

        IEnumerable<Guid> MemberIds { get; set; }

        IEnumerable<Guid> HousingDepartments { get; set; }

        JobStateType? JobState { get; set; }

        IEnumerable<JobStatus> JobStatuses { get; set; }
    }
}
