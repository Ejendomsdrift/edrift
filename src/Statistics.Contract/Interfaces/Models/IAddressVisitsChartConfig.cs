using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Contract.Interfaces.Models
{
    public interface IAddressVisitsChartConfig
    {
        IEnumerable<JobTypeEnum> TaskTypesToInclude { get; set; }
    }
}