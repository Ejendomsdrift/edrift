using System;

namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IEmployeeAbsenceInfoModel
    {
        Guid Id { get; set; } 

        Guid MemberId { get; set; }

        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        string Text { get; set; }

        Guid? AbsenceTemplateId { get; set; }
    }
}