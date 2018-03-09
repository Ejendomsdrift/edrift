using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Core.ChartBuildConfigs
{
    public class AddressVisitsChartConfig : IAddressVisitsChartConfig
    {
        public IEnumerable<JobTypeEnum> TaskTypesToInclude { get; set; }
    }
}
