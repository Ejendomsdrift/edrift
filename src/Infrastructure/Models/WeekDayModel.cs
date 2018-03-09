using System;

namespace Infrastructure.Models
{
    public class WeekDayModel
    {
        public int DayNumber { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        public bool IsCurrent { get; set; }
    }
}