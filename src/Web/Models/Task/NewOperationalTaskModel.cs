using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Models.Task
{
    public class NewOperationalTaskModel
    {
        public Guid DepartmentId { get; set; }
        public Guid CreatorId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? TeamLeadId { get; set; }
        public IEnumerable<Guid> UserIdList { get; set; } = Enumerable.Empty<Guid>();
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public decimal Estimate { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
    }
}