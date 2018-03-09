using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Models.Task
{
    public class JobViewModel
    {
        public IJob Job { get; set; }
        public Guid JobAssignId { get; set; }
        public IEnumerable<Guid> AssignedHousingDepartmentsIdList { get; set; }
        public bool IsAllAssignedHousingDepartmentAreGrouped { get; set; }
        public bool IsAllChildGroupedTaskHided { get; set; }
        public bool IsPossibleToHideChildTask { get; set; }
    }
}