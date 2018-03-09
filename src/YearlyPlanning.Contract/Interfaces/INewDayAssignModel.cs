using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface INewDayAssignModel
    {
        Guid Id { get; set; }
        string JobId { get; set; }
        Guid DepartmentId { get; set; }
        Guid JobAssignId { get; set; }
        Guid? DayAssignId { get; set; }
        bool IsJobOpen { get; set; }
        Guid? GroupId { get; set; }
        string GroupName { get; set; }
        List<Guid> UserIdList { get; set; }
        Guid? TeamLeadId { get; set; }
        bool IsAssignedToAllUsers { get; set; }
        DateTime? Date { get; set; }
        int? WeekDay { get; set; }
        int? CurrentWeekDay { get; set; }
        int WeekNumber { get; set; }
        int EstimatedMinutes { get; set; }
        Guid? DayPerWeekId { get; set; }
        Guid? ExpiredDayAssignId { get; set; }
        int? ExpiredWeekNumber { get; set; }
        string Comment { get; set; }
        string ResidentName { get; set; }
        string ResidentPhone { get; set; }
        TenantTaskTypeEnum? TenantType { get; set; }
        List<UploadFileModel> UploadList { get; set; }
        int Year { get; set; }
        bool IsUrgent { get; set; }
        bool IsUnassignedAll { get; set; }
        Guid? CreatorId { get; set; }
        Guid? CancellationReasonId { get; set; }
    }
}
