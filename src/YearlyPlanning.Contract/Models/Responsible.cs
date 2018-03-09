using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Models
{
    public class Responsible
    {
        public string JobId { get; set; }
        public Guid HousingDepartmentId { get; set; }
        public int EstimateInMinutes { get; set; }
        public List<Guid> UserIdList { get; set; }
        public bool IsAssignedToAllUsers { get; set; }
        public Guid? GroupId { get; set; }
        public Guid TeamLeadId { get; set; }
    }
}
