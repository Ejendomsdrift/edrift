using Infrastructure.Extensions;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.Models;
using StatusCore.Contract.Enums;
using System.Web.Http;
using Web.Core.Attributes;
using Web.Models;
using YearlyPlanning.Contract.Enums;

namespace Web.Controllers
{
    [CompressFilter]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        private readonly ITaskStatisticService taskStatisticService;

        public StatisticsController(ITaskStatisticService taskStatisticService)
        {
            this.taskStatisticService = taskStatisticService;
        }

        [HttpGet, Route("getStatisticFiltersModel")]
        public IStatisticFiltersModel GetStatisticFiltersModel()
        {
            var result = taskStatisticService.GetStatisticFiltersModel();
            return result;
        }

        [HttpPost, Route("getTenantTasksVsVisitsAmountData")]
        public IChartData<IAddressStatisticInfo> GetTenantTasksVsVisitsAmountData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetTenantTasksVsVisitsAmountData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getTenantTasksVsVisitsAmountCsv")]
        public ITextFileResultModel GetTenantTasksVsVisitsAmountDataCsv(AddressInfoRequest tasksInfoRequest)
        {
            var result = taskStatisticService.GetTenantTasksVsVisitsAmountDataCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getSpentTimeVsTenantTasksData")]
        public IChartData<ITaskChartModel> GetSpentTimeVsTenantTasksData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetSpentTimeVsTenantTasksData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getSpentTimeVsTenantTasksCsv")]
        public ITextFileResultModel GetSpentTimeVsTenantTasksCsv(GroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = taskStatisticService.GetSpentTimeVsTenantTasksCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getCompletedVsOverdueTasksData")]
        public IRatioChartData<bool, ITaskChartModel> GetCompletedVsOverdueTasksData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetCompletedVsOverdueTasksData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getCompletedVsOverdueTasksCsv")]
        public ITextFileResultModel GetCompletedVsOverdueTasksCsv(GroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = taskStatisticService.GetCompletedVsOverdueTasksCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getFacilityTasksVsTenantTasksData")]
        public IRatioChartData<JobTypeEnum, ITaskChartModel> GetFacilityTasksVsTenantTasksData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetFacilityTasksVsTenantTasksData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getFacilityTasksVsTenantTasksCsv")]
        public ITextFileResultModel GetFacilityTasksVsTenantTasksCsv(GroupedTasksCsvRequest<JobTypeEnum> tasksInfoRequest)
        {
            var result = taskStatisticService.GetFacilityTasksVsTenantTasksCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getUnprocessedVsProcessedTasksData")]
        public IRatioChartData<JobStatus, ITaskChartModel> GetUnprocessedVsProcessedTasksData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetUnprocessedVsProcessedTasksData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("GetUnprocessedVsProcessedTasksCsv")]
        public ITextFileResultModel GetUnprocessedVsProcessedTasksCsv(GroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = taskStatisticService.GetUnprocessedVsProcessedDataCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getAbsencesReasonData")]
        public IChartData<IAbsenceDataModel> GetAbsencesReasonData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetAbsencesData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getAbsencesDataCsv")]
        public ITextFileResultModel GetAbsencesDataCsv(IAbsencesStatisticsCsvRequest absencesInfoRequest)
        {
            ITextFileResultModel absencesCsvData = taskStatisticService.GetAbsencesDataCsv(absencesInfoRequest);
            return absencesCsvData;
        }

        [HttpPost, Route("getSpentTimeVsFacilityTasksData")]
        public IChartData<ITaskChartModel> GetSpentTimeVsFacilityTasksData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetSpentTimeVsFacilityTasksData(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getSpentTimeVsFacilityTasksCsv")]
        public ITextFileResultModel GetSpentTimeVsFacilityTasksCsv(GroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = taskStatisticService.GetSpentTimeVsFacilityTasksCsv(tasksInfoRequest);
            return result;
        }

        [HttpPost, Route("getTenantTaskSeparatedByCanceledReasonData")]
        public IChartData<ICancelingReasonDataModel> GetTenantTaskSeparatedByCanceledReasonData(TimePeriodViewModel period)
        {
            var mappedPeriod = period.Map<ITimePeriod>();
            var result = taskStatisticService.GetRejectionReasonDataForTenant(mappedPeriod);
            return result;
        }

        [HttpPost, Route("getTenantTaskSeparatedByCanceledReasonDataCsv")]
        public ITextFileResultModel GetTenantTaskSeparatedByCanceledReasonDataCsv(CancelingReasonInfoRequest cancelingReasonInfoRequest)
        {
            ITextFileResultModel rejectedReasonCsvData = taskStatisticService.GetRejectedReasonDataCsv(cancelingReasonInfoRequest);
            return rejectedReasonCsvData;
        }
    }
}