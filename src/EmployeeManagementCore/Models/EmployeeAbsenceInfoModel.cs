using System;
using AbsenceTemplatesCore.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class EmployeeAbsenceInfoModel : IEmployeeAbsenceInfoModel
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Text { get; set; }

        public Guid? AbsenceTemplateId { get; set; }
    }
}