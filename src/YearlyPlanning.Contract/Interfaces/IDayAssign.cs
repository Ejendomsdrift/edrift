using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IDayAssign
    {
        Guid Id { get; set; }
        string JobId { get; set; }
        Guid JobAssignId { get; set; }
        Guid DepartmentId { get; set; }
        Guid? GroupId { get; set; }
        List<Guid> UserIdList { get; set; }
        DateTime? Date { get; set; }
        JobStatus StatusId { get; set; }
        int? EstimatedMinutes { get; set; }
        Guid? ExpiredDayAssignId { get; set; }
        int? ExpiredWeekNumber { get; set; }
        int WeekNumber { get; set; }
        Guid? TeamLeadId { get; set; }
        bool IsAssignedToAllUsers { get; set; }
        int? WeekDay { get; set; }
        Guid DayPerWeekId { get; set; }
        string Comment { get; set; }
        string ResidentName { get; set; }
        string ResidentPhone { get; set; }
        TenantTaskTypeEnum? TenantType { get; set; }
        List<UploadFileModel> UploadList { get; set; }
        int Year { get; set; }
        bool? IsUrgent { get; set; }
    }
}