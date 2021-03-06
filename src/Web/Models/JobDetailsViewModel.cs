﻿using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using Web.Models.Task;
using YearlyPlanning.Contract.Enums;

namespace Web.Models
{
    public class JobDetailsViewModel
    {
        public string JobId { get; set; }
        public Guid DayAssignId { get; set; }
        public string NiceId { get; set; }
        public string Title { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<Group.MemberViewModel> Members { get; set; }
        public Guid? TeamLeadId { get; set; }
        public DateTime? Date { get; set; }
        public string Address { get; set; }
        public string GlobalDescription { get; set; }
        public string LocalDescription { get; set; }
        public List<UploadFileViewModel> GlobalUploadList { get; set; }
        public List<UploadFileViewModel> LocalUploadList { get; set; }
        public List<UploadFileViewModel> JanitorUploadImageList { get; set; }
        public List<UploadFileViewModel> JanitorUploadVideoList { get; set; }
        public JobTypeEnum JobType { get; set; }
        public JobStatus JobStatus { get; set; }
        public bool AllowChangeStatus { get; set; }
        public int? Estimate { get; set; }
        public string TenantTypeString { get; set; }
        public string ResidentPhone { get; set; }
        public string ResidentName { get; set; }
        public string Comment { get; set; }
        public bool? IsUrgent { get; set; }
        public string AssignedHousingDepartmentName { get; set; }
    }
}