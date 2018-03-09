using System;

namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IAbsenceTemplateModel
    {
        Guid Id { get; set; }
        string Text { get; set; }
    }
}