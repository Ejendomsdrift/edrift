using System;

namespace YearlyPlanning.Contract.Models
{
    public class CreateOperationalTaskViewModel
    {
        public string Id { get; set; }
        public Guid DayAssignId { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
