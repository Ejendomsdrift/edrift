using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IOperationalTaskModel
    {
        string Id { get; set; }
        Guid DepartmentId { get; set; }
        string JobId { get; set; }
        Guid JobAssignId { get; set; }
        Guid? GroupId { get; set; }
        Guid? TeamLeadId { get; set; }
        IEnumerable<Guid> UserIdList { get; set; }
        string Title { get; set; }
        string Address { get; set; }
        string Description { get; set; }
        bool IsAssignedToAllUsers { get; set; }
        Guid CreatorId { get; set; }
        decimal Estimate { get; set; }
        ICategoryModel Category { get; set; }
        Guid CategoryId { get; set; }
        List<UploadFileModel> Uploads { get; set; }
        Guid DayAssignId { get; set; }
        JobStatus StatusId { get; set; }
        DateTime? Date { get; set; }
        DateTime? Time { get; set; }
        string Comment { get; set; }
        string ResidentName { get; set; }
        string ResidentPhone { get; set; }
        bool? IsUrgent { get; set; }
        TenantTaskTypeEnum? Type { get; set; }
    }
}
