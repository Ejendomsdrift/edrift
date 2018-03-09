using System;
using System.Collections.Generic;
using MemberCore.Contract.Interfaces;
using CategoryCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IWeekPlanJobModel
    {
        string Id { get; set; }

        Guid JobAssignId { get; set; }

        bool IsBackLogJob { get; set; }

        IEnumerable<int> AllowedDays { get; set; }

        int WeekNumber { get; set; }

        int? ExpiredWeekNumber { get; set; }

        string Title { get; set; }

        JobStatus StatusId { get; set; }

        ICategoryModel Category { get; set; }

        JobTypeEnum JobType { get; set; }

        string JobTypeName { get; set; }

        string TenantTypeName { get; set; }

        string Address { get; set; }

        Guid CreatorId { get; set; }


        //properties below are from DayAssign model
        Guid? DayAssignId { get; set; }

        IEnumerable<IMemberModel> Users { get; set; }

        bool IsWeekEndJob { get; set; }

        DateTime? DayAssignDate { get; set; }

        Guid? GroupId { get; set; }

        string GroupName { get; set; }

        string DepartmentName { get; set; }

        bool IsAssignedToAllUsers { get; set; }

        int? WeekDay { get; set; }

        int Year { get; set; }

        Guid DayPerWeekId { get; set; }

        IChangeStatusInfo ChangeStatusInfo { get; set; }

        Guid? ExpiredDayAssignId { get; set; }

        Guid? TeamLeadId { get; set; }

        bool IsVirtualTicket { get; }

        decimal? Estimate { get; }

        decimal? SpentTime { get; }

        bool? IsUrgent { get; set; }

        bool IsCommentExistOnAnyStatus { get; set; }

        string TaskDisplayColor { get; set; }
    }
}
