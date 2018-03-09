using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class WeekPlanJobModel: IWeekPlanJobModel
    {
        public string Id { get; set; }

        public Guid JobAssignId { get; set; }

        public bool IsBackLogJob { get; set; }        

        public IEnumerable<int> AllowedDays { get; set; }

        public int WeekNumber { get; set; }

        public int? ExpiredWeekNumber { get; set; }

        public string Title { get; set; }

        public JobStatus StatusId { get; set; }

        public ICategoryModel Category { get; set; }

        public JobTypeEnum JobType { get; set; }

        public string JobTypeName { get; set; }

        public string TenantTypeName { get; set; }

        public string Address { get; set; }

        public Guid CreatorId { get; set; }


        //properties below are from DayAssign model
        public Guid? DayAssignId { get; set; }

        public IEnumerable<IMemberModel> Users { get; set; }

        public bool IsWeekEndJob { get; set; }

        public DateTime? DayAssignDate { get; set; }

        public Guid? GroupId { get; set; }

        public string GroupName { get; set; }

        public string DepartmentName { get; set; }

        public bool IsAssignedToAllUsers { get; set; }

        public int? WeekDay { get; set; }

        public int Year { get; set; }

        public Guid DayPerWeekId { get; set; }

        public IChangeStatusInfo ChangeStatusInfo { get; set; }

        public Guid? ExpiredDayAssignId { get; set; }

        public Guid? TeamLeadId { get; set; }

        public bool IsVirtualTicket => !DayAssignId.HasValue;

        public decimal? Estimate { get; set; }

        public decimal? SpentTime { get; set; }

        public bool? IsUrgent { get; set; }

        public bool IsCommentExistOnAnyStatus { get; set; }

        public string TaskDisplayColor { get; set; }
    }
}
