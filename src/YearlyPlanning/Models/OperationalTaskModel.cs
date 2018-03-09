using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Models
{
    public class OperationalTaskModel: IOperationalTaskModel
    {
        public string Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string JobId { get; set; }
        public Guid JobAssignId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? TeamLeadId { get; set; }
        public IEnumerable<Guid> UserIdList { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public Guid CreatorId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public Guid CategoryId { get; set; }
        public IEnumerable<int> DaysPerWeek { get; set; }
        public decimal Estimate { get; set; }
        public ICategoryModel Category { get; set; }
        public List<UploadFileModel> Uploads { get; set; }
        public Guid DayAssignId { get; set; }
        public JobStatus StatusId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public string Comment { get; set; }
        public string ResidentName { get; set; }
        public string ResidentPhone { get; set; }
        public TenantTaskTypeEnum? Type { get; set; }
        public bool? IsUrgent { get; set; }
    }
}
