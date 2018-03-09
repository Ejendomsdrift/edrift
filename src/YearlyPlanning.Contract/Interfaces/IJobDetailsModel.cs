using System;
using System.Collections.Generic;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJobDetailsModel
    {
        string JobId { get; set; }
        Guid DayAssignId { get; set; }
        string NiceId { get; set; }
        string Title { get; set; }
        Guid? GroupId { get; set; }
        string GroupName { get; set; }
        IEnumerable<IMemberModel> Members { get; set; }
        Guid? TeamLeadId { get; set; }
        DateTime? Date { get; set; }
        string Address { get; set; }
        string GlobalDescription { get; set; }
        string LocalDescription { get; set; }
        List<UploadFileModel> GlobalUploadList { get; set; }
        List<UploadFileModel> LocalUploadList { get; set; }
        List<UploadFileModel> JanitorUploadImageList { get; set; }
        List<UploadFileModel> JanitorUploadVideoList { get; set; }
        JobTypeEnum JobType { get; set; }
        JobStatus JobStatus { get; set; }
        bool AllowChangeStatus { get; set; }
        int? Estimate { get; set; }
        string TenantTypeString { get; set; }
        string ResidentPhone { get; set; }
        string ResidentName { get; set; }
        string Comment { get; set; }
        bool? IsUrgent { get; set; }
        string AssignedHousingDepartmentName { get; set; }
    }
}