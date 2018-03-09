using System;
using System.Collections.Generic;
using MongoRepository.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public class DayAssign : IDayAssign, IEntity
    {
        public Guid Id { get; set; }
        public string JobId { get; set; }
        public Guid JobAssignId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? GroupId { get; set; }
        public List<Guid> UserIdList { get; set; } = new List<Guid>();
        public DateTime? Date { get; set; }
        public JobStatus StatusId { get; set; }
        public int? EstimatedMinutes { get; set; }
        public Guid? ExpiredDayAssignId { get; set; }
        public int WeekNumber { get; set; }
        public int? ExpiredWeekNumber { get; set; }
        public Guid? TeamLeadId { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public int? WeekDay { get; set; }
        public Guid DayPerWeekId { get; set; }
        public string Comment { get; set; }
        public string ResidentName { get; set; }
        public string ResidentPhone { get; set; }
        public TenantTaskTypeEnum? TenantType { get; set; }
        public List<UploadFileModel> UploadList { get; set; } = new List<UploadFileModel>();      
        public int Year { get; set; }
        public bool? IsUrgent { get; set; }
    }
}
