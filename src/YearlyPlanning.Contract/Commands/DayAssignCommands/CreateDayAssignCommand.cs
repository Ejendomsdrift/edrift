using System;
using YearlyPlanning.Contract.Enums;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class CreateDayAssignCommand : DayAssignCommand
    {
        public string JobId { get; set; }
        public Guid JobAssignId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? GroupId { get; set; }
        public List<Guid> UserIdList { get; set; }
        public DateTime? Date { get; set; }
        public JobStatus StatusId { get; set; }
        public int? EstimatedMinutes { get; set; }
        public string GroupName { get; set; }
        public int WeekNumber { get; set; }
        public string Address { get; set; }
        public Guid? TeamLeadId { get; set; }
        public int? WeekDay { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public Guid DayPerWeekId { get; set; }
        public string Comment { get; set; }
        public string ResidentName { get; set; }
        public string ResidentPhone { get; set; }
        public TenantTaskTypeEnum? Type { get; set; }
        public Guid? ExpiredDayAssignId { get; set; }
        public int? ExpiredWeekNumber { get; set; }
        public bool? IsUrgent { get; set; }
        public Guid? CreatorId { get; set; }

        public CreateDayAssignCommand() { }
        public CreateDayAssignCommand(Guid dayPerWeekId)
        {
            DayPerWeekId = dayPerWeekId;
        }

        public CreateDayAssignCommand(Guid id, string jobId, Guid jobAssignId, DateTime? date, bool? isUrgent = null) : base(id.ToString())
        {
            JobId = jobId;
            JobAssignId = jobAssignId;
            Date = date;
            IsUrgent = isUrgent;
        }
    }
}