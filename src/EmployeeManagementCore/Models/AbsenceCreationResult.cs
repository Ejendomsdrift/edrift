using AbsenceTemplatesCore.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class AbsenceCreationResult: IAbsenceCreationResult
    {
        public bool IsSucceeded { get; set; }
        public IEmployeeAbsenceInfoModel Absence { get; set; }
    }
}