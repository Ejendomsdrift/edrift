using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Contract.Interfaces
{
    public interface IDayAssignsTimeSpanSelector
    {
        IEnumerable<IDayAssign> Get(ITimePeriod period, IChartDataQueryingRestrictions restrictions);
    }
}
