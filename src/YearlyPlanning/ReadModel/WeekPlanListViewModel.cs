using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using System;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.ReadModel
{
    public class WeekPlanListViewModel: IWeekPlanListViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime? AssignDate { get; set; }
        public Guid? DayAssignId { get; set; }
        public IEnumerable<IMemberModel> Users { get; set; }
        public string Address { get; set; }
        public DateTime? TimeDate { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public decimal? EstimatedTime { get; set; }
        public decimal? SpentTime { get; set; }
        public string GroupName { get; set; }
        public string DepartmentName { get; set; }
        public JobTypeEnum JobType { get; set; }
        public int? WeekDay { get; set; }
        public int WeekNumber { get; set; }
        public bool HasChangeStatusComment { get; set; }
        public bool? IsUrgent { get; set; }
        public JobStatus StatusId { get; set; }
    }
}
