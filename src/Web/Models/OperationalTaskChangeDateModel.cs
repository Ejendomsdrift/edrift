using System;

namespace Web.Models
{
    public class OperationalTaskChangeDateModel
    {
        public string JobId { get; set; }
        public Guid DayAssignId { get; set; }
        public DateTime Date { get; set; }
    }
}