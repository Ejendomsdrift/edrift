using System.Collections.Generic;
using AbsenceTemplatesCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class EmployeeAbsences : IEmployeeAbsences
    {
        public IMemberModel Member { get; set; }
        public IEnumerable<IEmployeeAbsenceInfoModel> Absences { get; set; }
    }
}
