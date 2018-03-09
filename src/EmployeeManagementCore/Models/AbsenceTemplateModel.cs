using System;
using AbsenceTemplatesCore.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class AbsenceTemplateModel: IAbsenceTemplateModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}
