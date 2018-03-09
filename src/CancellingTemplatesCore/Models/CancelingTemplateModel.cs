using System;
using System.Collections.Generic;
using System.Linq;
using CancellingTemplatesCore.Contract.Interfaces;
using YearlyPlanning.Contract.Enums;

namespace CancellingTemplatesCore.Models
{
    public class CancelingTemplateModel : ICancelingTemplateModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
        public IEnumerable<JobTypeEnum> JobTypeList { get; set; } = Enumerable.Empty<JobTypeEnum>();
        public bool IsCoordinatorReason { get; set; }
    }
}
