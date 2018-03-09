using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace Web.Models
{
    public class NewDayAssignViewModel: INewDayAssignModel
    {
        public Guid Id { get; set; }
        public string JobId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid JobAssignId { get; set; }
        public Guid? DayAssignId { get; set; }
        public bool IsJobOpen { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Guid> UserIdList { get; set; }
        public Guid? TeamLeadId { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public DateTime? Date { get; set; }
        public int? WeekDay { get; set; }
        public int? CurrentWeekDay { get; set; }
        public int WeekNumber { get; set; }
        public int EstimatedMinutes { get; set; }
        public Guid? DayPerWeekId { get; set; }
        public Guid? ExpiredDayAssignId { get; set; }
        public int? ExpiredWeekNumber { get; set; }
        public string Comment { get; set; }
        public string ResidentName { get; set; }
        public string ResidentPhone { get; set; }
        public TenantTaskTypeEnum? TenantType { get; set; }
        public List<UploadFileModel> UploadList { get; set; }
        public int Year { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsUnassignedAll { get; set; }
        public Guid? CreatorId { get; set; }
        public JobStatus JobStatus { get; set; }
        public Guid? CancellationReasonId { get; set; }
    }
}