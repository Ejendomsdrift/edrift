using System;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Models
{
    public class AbsenceDataModel : IAbsenceDataModel
    {
        public Guid AbsenceId { get; set; }
        public Guid? ReasonId { get; set; }
        public string Reason { get; set; }
        public decimal SpentTime { get; set; } // in hours
    }
}
