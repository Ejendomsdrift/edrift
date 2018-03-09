using Statistics.Contract.Interfaces.Models;

namespace Statistics.Contract.Interfaces.ChartsDataBuilders
{
    public interface IAddressVisitsChartModelBuilder
    {
        IChartData<IAddressStatisticInfo> Build(IChartDataQueryingRestrictions restrictions, IAddressVisitsChartConfig config, ITimePeriod period, bool showLastCompletedOrCanceledStatus);
    }
}