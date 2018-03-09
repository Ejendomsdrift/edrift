using System;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJobRelatedByAddressModel
    {
        string JobId { get; set; }

        Guid DayAssignId { get; set; }

        DateTime? Date { get; set; }

        string Address { get; set; }

        string Title { get; set; }
    }
}
