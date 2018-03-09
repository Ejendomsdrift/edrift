using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Core.ChartBuildConfigs
{
    class SpentTimeChartConfig : ISpentTimeChartConfig
    {
        public IEnumerable<JobTypeEnum> TaskTypesToInclude { get; set; }
    }
}