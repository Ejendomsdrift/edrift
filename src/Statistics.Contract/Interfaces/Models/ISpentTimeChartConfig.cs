using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Contract.Interfaces.Models
{
    public interface ISpentTimeChartConfig
    {
        IEnumerable<JobTypeEnum> TaskTypesToInclude { get; set; }
    }
}