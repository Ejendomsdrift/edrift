using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace CancellingTemplatesCore.Contract.Interfaces
{
    public interface ICancelingTemplateModel
    {
        Guid Id { get; set; }
        string Text { get; set; }
        IEnumerable<JobTypeEnum> JobTypeList { get; set; }
        bool IsCoordinatorReason { get; set; }
    }
}