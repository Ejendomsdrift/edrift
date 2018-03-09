using System;

namespace Web.Models
{
    public class JobFilterViewModel
    {
        public string JobId { get; set; }
        public Guid HousingDepartmentId { get; set; }
        public Guid? DayAssignId { get; set; }
        public int CurrentWeekDay { get; set; }
        public int WeekNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}