using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace Web.Models.Task
{
    public class OperationalTaskViewModel
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
        public decimal Estimate { get; set; }
        public List<UploadFileViewModel> Uploads { get; set; }
        public Guid DayAssignId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime Time { get; set; }
        public JobStatus StatusId { get; set; }
        public bool IsCanceled { get; set; }
    }
}