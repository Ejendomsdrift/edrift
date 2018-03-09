using System;
using Statistics.Contract.Interfaces;

namespace Statistics.Core.Models
{
    public class TimePeriod : ITimePeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
