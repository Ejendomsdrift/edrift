using System;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IChangeStatusModel
    {
        bool IsSuccessful { get; set; }
        Guid DayAssignId { get; set; }
    }
}
