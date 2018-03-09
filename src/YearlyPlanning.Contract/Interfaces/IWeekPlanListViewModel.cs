using System;
using MemberCore.Contract.Interfaces;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IWeekPlanListViewModel
    {
        string Id { get; }
        string Title { get; }
        DateTime? AssignDate { get; }
        Guid? DayAssignId { get; set; }
        JobStatus StatusId { get; }
        JobTypeEnum JobType { get; }
        IEnumerable<IMemberModel> Users { get; }
        string GroupName { get; set; }       
        string DepartmentName { get; set; }       
        string Address { get; set; }
        DateTime? TimeDate { get; set; }
        bool IsAssignedToAllUsers { get; set; }
        decimal? EstimatedTime { get; set; }
        decimal? SpentTime { get; set; }
        int? WeekDay { get; set; }
        int WeekNumber { get; set; }
        bool HasChangeStatusComment { get; set; }
        bool? IsUrgent { get; set; }
    }
}
