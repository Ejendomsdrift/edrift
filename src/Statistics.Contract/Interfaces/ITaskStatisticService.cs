using Statistics.Contract.Interfaces.Models;
using StatusCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Contract.Interfaces
{
    public interface ITaskStatisticService
    {
        IStatisticFiltersModel GetStatisticFiltersModel();

        IChartData<IAddressStatisticInfo> GetTenantTasksVsVisitsAmountData(ITimePeriod period);
        ITextFileResultModel GetTenantTasksVsVisitsAmountDataCsv(IAddressStatisticsCsvRequest tasksInfoRequest);

        IChartData<ITaskChartModel> GetSpentTimeVsFacilityTasksData(ITimePeriod period);
        ITextFileResultModel GetSpentTimeVsFacilityTasksCsv(IGroupedTasksCsvRequest<string> csvChartRequest);

        IChartData<ITaskChartModel> GetSpentTimeVsTenantTasksData(ITimePeriod period);
        ITextFileResultModel GetSpentTimeVsTenantTasksCsv(IGroupedTasksCsvRequest<string> tasksInfoRequest);

        IRatioChartData<bool, ITaskChartModel> GetCompletedVsOverdueTasksData(ITimePeriod period);
        ITextFileResultModel GetCompletedVsOverdueTasksCsv(IGroupedTasksCsvRequest<string> csvChartRequest);

        IRatioChartData<JobTypeEnum, ITaskChartModel> GetFacilityTasksVsTenantTasksData(ITimePeriod period);
        ITextFileResultModel GetFacilityTasksVsTenantTasksCsv(IGroupedTasksCsvRequest<JobTypeEnum> csvChartRequest);

        IRatioChartData<JobStatus, ITaskChartModel> GetUnprocessedVsProcessedTasksData(ITimePeriod period);
        ITextFileResultModel GetUnprocessedVsProcessedDataCsv(IGroupedTasksCsvRequest<string> csvChartRequest);

        IChartData<IAbsenceDataModel> GetAbsencesData(ITimePeriod period);
        ITextFileResultModel GetAbsencesDataCsv(IAbsencesStatisticsCsvRequest absencesInfoRequest);

        IChartData<ICancelingReasonDataModel> GetRejectionReasonDataForTenant(ITimePeriod period);
        ITextFileResultModel GetRejectedReasonDataCsv(ICancelingReasonInfoRequest cancelingReasonInfoRequest);
    }
}