
namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IAbsenceCreationResult
    {
        bool IsSucceeded { get; set; }
        IEmployeeAbsenceInfoModel Absence { get; set; }
    }
}
