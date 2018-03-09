using System;
using System.Collections.Generic;

namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IAbsenceTemplatesService
    {
        IEnumerable<IAbsenceTemplateModel> GetAll();
        IAbsenceTemplateModel GetById(Guid absenceTemplateId);
        IAbsenceTemplateModel Save(string templateText);
        void Delete(Guid absenceTemplateId);
    }
}