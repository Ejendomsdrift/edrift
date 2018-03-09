using System;

namespace Statistics.Contract.Interfaces
{
    public interface ITimePeriod
    {
        DateTime EndDate { get; set; }
        DateTime StartDate { get; set; }
    }
}