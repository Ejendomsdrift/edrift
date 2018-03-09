using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Models
{
    public class WeekPlanParamsModel
    {
        public Job Job { get; set; }

        public JobAssign JobAssign { get; set; }

        public int WeekNumber { get; set; }

        public int Year { get; set; }

        public Guid? DepartmentId { get; set; }

        public bool IsAllowGetVirtualTickets { get; set; }

        public IEnumerable<IJobStatusLogModel> JobStatusLogs { get; set; }
    }
}
