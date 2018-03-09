using System.Collections.Generic;
using MemberCore.Contract.Interfaces;

namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IEmployeeAbsences
    {
        IMemberModel Member { get; set; }
        IEnumerable<IEmployeeAbsenceInfoModel> Absences { get; set; }
    }
}