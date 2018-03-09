using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace Web.Models
{
    public class CancellingTemplateViewModel
    {
        public string Text { get; set; }
        public IEnumerable<JobTypeEnum> JobTypeList { get; set; }
        public bool IsCoordinatorReason { get; set; }
    }
}