using System;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IAbsenceDataModel
    {
        Guid AbsenceId { get; set; }
        Guid? ReasonId { get; set; }
        string Reason { get; set; }
        decimal SpentTime { get; set; } 
    }
}