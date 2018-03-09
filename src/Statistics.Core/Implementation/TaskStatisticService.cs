using System;
using System.Collections.Generic;
using System.Linq;
using AbsenceTemplatesCore.Contract.Interfaces;
using AutoMapper;
using CategoryCore.Contract.Interfaces;
using CsvHelper.Configuration;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using Statistics.Contract.Enums;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.ChartsDataBuilders;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.ChartBuildConfigs;
using Statistics.Core.Models;
using Statistics.Core.Profiles;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using Translations;
using Translations.Interfaces;
using Translations.Models;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace Statistics.Core.Implementation
{
    public class TaskStatisticService : ITaskStatisticService
    {
        private const string OtherReasonKey = "statistics-page__absence-otherReasons";
        private const string FileExstension = ".csv";
        private const string TranslationKeySuffix = "CsvFileName_";

        private readonly ICsvHelper csvHelper;
        private readonly ITasksInfoBuilder tasksInfoBuilder;
        private readonly IMemberService memberService;
        private readonly IManagementDepartmentService managementDepartmentService;
        private readonly IAppSettingHelper appSettingHelper;
        private readonly ITaskRatioChartModelBuilder ratioChartDataBuilder;
        private readonly ISpentTimeChartDataBuilder spentTimeChartDataBuilder;
        private readonly IAddressVisitsChartModelBuilder addressVisitsChartModelBuilder;
        private readonly IStatisticFiltersModelBuilder statisticFiltersModelBuilder;
        private readonly ITranslationService translationService;
        private readonly IEmployeeAbsenceInfoService absenceInfoService;
        private readonly IDayAssignService dayAssignService;
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly ICategoryService categoryService;
        private readonly IJobService jobService;

        public TaskStatisticService(
            IMemberService memberService,
            IManagementDepartmentService managementDepartmentService,
            ITaskRatioChartModelBuilder ratioChartDataBuilder,
            IAppSettingHelper appSettingHelper,
            ISpentTimeChartDataBuilder spentTimeChartDataBuilder,
            IStatisticFiltersModelBuilder statisticFiltersModelBuilder,
            ITasksInfoBuilder tasksInfoBuilder,
            ICsvHelper csvHelper,
            ITranslationService translationService,
            IEmployeeAbsenceInfoService absenceInfoService,
            IDayAssignService dayAssignService,
            IAddressVisitsChartModelBuilder addressVisitsChartModelBuilder,
            IJobStatusLogService jobStatusLogService,
            ICategoryService categoryService,
            IJobService jobService)
        {
            this.memberService = memberService;
            this.managementDepartmentService = managementDepartmentService;
            this.ratioChartDataBuilder = ratioChartDataBuilder;
            this.appSettingHelper = appSettingHelper;
            this.spentTimeChartDataBuilder = spentTimeChartDataBuilder;
            this.statisticFiltersModelBuilder = statisticFiltersModelBuilder;
            this.tasksInfoBuilder = tasksInfoBuilder;
            this.csvHelper = csvHelper;
            this.translationService = translationService;
            this.absenceInfoService = absenceInfoService;
            this.dayAssignService = dayAssignService;
            this.addressVisitsChartModelBuilder = addressVisitsChartModelBuilder;
            this.jobStatusLogService = jobStatusLogService;
            this.categoryService = categoryService;
            this.jobService = jobService;
        }

        public IStatisticFiltersModel GetStatisticFiltersModel()
        {
            return statisticFiltersModelBuilder.Build();
        }

        public IChartData<IAddressStatisticInfo> GetTenantTasksVsVisitsAmountData(ITimePeriod period)
        {
            var queryingRestrictions = GetUserFilteringCriterias();
            queryingRestrictions.AllowedStatuses = GetAllowedStatuses(Constants.ChartConfigurationKeys.AllowedStatuses_TenantTasksVsVisitsAmount);
            queryingRestrictions.QueryingAlgorithm = QueryingAlgorithmType.ByCompletitionDateCriteria;
            var chartConfig = new AddressVisitsChartConfig
            {
                TaskTypesToInclude = GetTaskTypes(Constants.ChartConfigurationKeys.TenantTasksVsVisitsAmountChartTypes)
            };
            var chartModel = addressVisitsChartModelBuilder.Build(queryingRestrictions, chartConfig, period, false);
            return chartModel;
        }

        public ITextFileResultModel GetTenantTasksVsVisitsAmountDataCsv(IAddressStatisticsCsvRequest tasksInfoRequest)
        {
            var infos = tasksInfoRequest.AddressStatisticInfos.Map<IEnumerable<AddressStatisticInfo>>();
            var fileName = GetChartCsvFileName(Constants.ChartName.TenantTasksVsVisitsAmount, tasksInfoRequest.RangeDateString);
            var result = GetCsvTextFileResult(infos, new AddressStatisticInfoCsvProfile(translationService), fileName);
            return result;
        }

        public IChartData<ITaskChartModel> GetSpentTimeVsFacilityTasksData(ITimePeriod period)
        {
            var priority = 0;
            var categorySortPriority = GetCategorySortPriority(ref priority);

            var chartModel = GetSpentTimeChartData(period,
                Constants.ChartConfigurationKeys.AllowedStatuses_SpentTimeVsFacilityTasks, Constants.ChartConfigurationKeys.FacilitySpentTimeChartTypes, true,
                QueryingAlgorithmType.ByCompletitionDateCriteria, categorySortPriority);
            return chartModel;
        }

        public ITextFileResultModel GetSpentTimeVsFacilityTasksCsv(IGroupedTasksCsvRequest<string> csvChartRequest)
        {
            var result = GetGroupedTasksCsv(csvChartRequest,
                Constants.ChartName.SpentTimeVsFacilityTasks, new SeparationBetweenSpentTimeVsFacilityTasksChartCsvProfile(translationService), true, true);
            return result;
        }

        public IRatioChartData<bool, ITaskChartModel> GetCompletedVsOverdueTasksData(ITimePeriod period)
        {
            var chartModel = GetTaskRatioChartModel(period,
                Constants.ChartConfigurationKeys.AllowedStatuses_CompletedVsOverdueTasks, Constants.ChartConfigurationKeys.Groupings_CompletedVsOverdueTasks,
                QueryingAlgorithmType.ByDateCriteria, t => t.IsOverdue, false);
            return chartModel;
        }

        public ITextFileResultModel GetCompletedVsOverdueTasksCsv(IGroupedTasksCsvRequest<string> csvChartRequest)
        {
            var result = GetGroupedTasksCsv(csvChartRequest,
                Constants.ChartName.CompletedVsOverdueTasks, new CompletedToOverdueTaskChartCsvProfile(translationService), false);
            return result;
        }

        public IRatioChartData<JobTypeEnum, ITaskChartModel> GetFacilityTasksVsTenantTasksData(ITimePeriod period)
        {
            var chartModel = GetTaskRatioChartModel(period,
                Constants.ChartConfigurationKeys.AllowedStatuses_FacilityTasksVsTenantTasks, Constants.ChartConfigurationKeys.Groupings_FacilityTasksVsTenantTasks,
                QueryingAlgorithmType.ByCompletitionDateCriteria, t => t.JobType, true);
            return chartModel;
        }

        public ITextFileResultModel GetFacilityTasksVsTenantTasksCsv(IGroupedTasksCsvRequest<JobTypeEnum> csvChartRequest)
        {
            var result = GetGroupedTasksCsv(csvChartRequest,
                Constants.ChartName.FacilityTasksVsTenantTasks, new SeparationBetweenFacilityTasksVsTenantTasksChartCsvProfile(translationService), true, true);
            return result;
        }

        public IRatioChartData<JobStatus, ITaskChartModel> GetUnprocessedVsProcessedTasksData(ITimePeriod period)
        {
            var chartModel = GetTaskRatioChartModel(period,
                Constants.ChartConfigurationKeys.AllowedStatuses_UnprocessedVsProcessedTasks, Constants.ChartConfigurationKeys.Groupings_UnprocessedVsProcessedTasks,
                QueryingAlgorithmType.ByCompletitionDateCriteria, t => t.JobStatus, true);
            return chartModel;
        }

        public ITextFileResultModel GetUnprocessedVsProcessedDataCsv(IGroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = GetGroupedTasksCsv(tasksInfoRequest,
                Constants.ChartName.UnprocessedVsProcessed, new UnprocessedVsProcessedChartCsvProfile(translationService), true);
            return result;
        }

        public IChartData<ITaskChartModel> GetSpentTimeVsTenantTasksData(ITimePeriod period)
        {
            var chartModel = GetSpentTimeChartData(period,
                Constants.ChartConfigurationKeys.AllowedStatuses_SpentTimeVsTenantTasks, Constants.ChartConfigurationKeys.TenantSpentTimeChartTypes, true,
                QueryingAlgorithmType.ByCompletitionDateCriteria, new Dictionary<Guid, int>());
            return chartModel;
        }

        public ITextFileResultModel GetSpentTimeVsTenantTasksCsv(IGroupedTasksCsvRequest<string> tasksInfoRequest)
        {
            var result = GetGroupedTasksCsv(tasksInfoRequest,
                Constants.ChartName.SpentTimeVsTenantTasks, new TenantTaskSpentTimeChartCsvProfile(translationService), false, true);
            return result;
        }

        public IChartData<IAbsenceDataModel> GetAbsencesData(ITimePeriod period)
        {
            IEnumerable<Guid> employeeIdList = GetEmployeeIdListForCurrentUser();
            IEnumerable<IEmployeeAbsenceInfoModel> employeesAbsencesList = absenceInfoService.GetByMemberIdsForPeriod(employeeIdList, period.StartDate, period.EndDate);
            IEnumerable<IAbsenceDataModel> absencesListWithTime = GetAbsencesListWithTime(employeesAbsencesList, period.StartDate, period.EndDate);
            var result = new ChartData<IAbsenceDataModel>
            {
                Data = absencesListWithTime
            };
            return result;
        }

        public ITextFileResultModel GetAbsencesDataCsv(IAbsencesStatisticsCsvRequest absencesInfoRequest)
        {
            IEnumerable<AbsenceInfo> absenceInfo = GetAbsencesInfoData(absencesInfoRequest);
            string fileName = GetChartCsvFileName(Constants.ChartName.AbsencesData, absencesInfoRequest.RangeDateString);
            ITextFileResultModel result = GetCsvTextFileResult(absenceInfo, new AbsencesDataChartCsvProfile(translationService), fileName);
            return result;
        }

        public IChartData<ICancelingReasonDataModel> GetRejectionReasonDataForTenant(ITimePeriod period)
        {
            var housingDepartments = Enumerable.Empty<Guid>();
            IMemberModel currentMember = memberService.GetCurrentUser();
            IEnumerable<JobStatus> allowedJobStatuses = GetAllowedStatuses(Constants.ChartConfigurationKeys.AllowedStatuses_TenantTaskVsRejectedReason);

            if (currentMember.CurrentRole == RoleType.Coordinator)
            {
                Guid userActiveManagementDepartmentId = currentMember.ActiveManagementDepartmentId ?? Guid.Empty;
                housingDepartments = managementDepartmentService.GetHousingDepartmentIds(userActiveManagementDepartmentId);
            }

            List<IDayAssign> dayAssign = dayAssignService.GetForStatisticTenantTask(period.StartDate, period.EndDate, allowedJobStatuses, housingDepartments);

            IEnumerable<ICancelingReasonDataModel> cancelingReasonModels = GetRejectionReasonDataModels(dayAssign);
            var result = new ChartData<ICancelingReasonDataModel>
            {
                Data = cancelingReasonModels
            };
            return result;  
        }

        public ITextFileResultModel GetRejectedReasonDataCsv(ICancelingReasonInfoRequest rejectedReasonInfoRequest)
        {
            IEnumerable<RejectionReasonInfo> cancelingReasonInfo = GetCancelingReasonInfo(rejectedReasonInfoRequest.DayAssignIdList);
            string fileName = GetChartCsvFileName(Constants.ChartName.CancelingReason, rejectedReasonInfoRequest.RangeDateString);
            ITextFileResultModel result = GetCsvTextFileResult(cancelingReasonInfo, new RejectionReasonChartCsvProfile(translationService), fileName);
            return result;
        }

        private IRatioChartData<TGroup, ITaskChartModel> GetTaskRatioChartModel<TGroup>(ITimePeriod period, string allowedStatusesConfigKey, string chartGroupingsConfigKey,
            QueryingAlgorithmType queryingAlgorithmType, Func<ITaskChartModel, TGroup> taskGroupingSelector, bool showLastCompletedOrCanceledStatus)
        {
            var queryingRestrictions = GetUserFilteringCriterias();
            queryingRestrictions.QueryingAlgorithm = queryingAlgorithmType;
            queryingRestrictions.AllowedStatuses = GetAllowedStatuses(allowedStatusesConfigKey);
            var chartConfig = new TaskRatioChartConfig<TGroup>
            {
                TypesGroupings = GetTaskGroupings<TGroup>(chartGroupingsConfigKey),
                TaskGroupingSelector = taskGroupingSelector
            };
            var chartModel = ratioChartDataBuilder.Build(queryingRestrictions, chartConfig, period, showLastCompletedOrCanceledStatus, new Dictionary<Guid, int>());
            return chartModel;
        }

        public ITextFileResultModel GetGroupedTasksCsv<T>(
            IGroupedTasksCsvRequest<T> csvChartRequest, string chartName, CsvClassMap<TaskInfo> csvProfile, bool showLastCompletedOrCanceledStatus, bool onlyWithTime = false)
        {
            IEnumerable<IDayAssign> dayAssigns = dayAssignService.GetByIds(csvChartRequest.GroupedTasksIds.SelectMany(g => g.Value));
            IEnumerable<TaskInfo> taskInfoList = tasksInfoBuilder.GetTaskInfoList(dayAssigns, showLastCompletedOrCanceledStatus, onlyWithTime);
            string fileName = GetChartCsvFileName(chartName, csvChartRequest.RangeDateString);
            var result = GetCsvTextFileResult(taskInfoList, csvProfile, fileName);
            return result;
        }

        private IChartData<ITaskChartModel> GetSpentTimeChartData(ITimePeriod period, string allowedStatusesConfigKey, string spentTimeChartTypesConfigKey,
            bool showLastCompletedOrCanceledStatus, QueryingAlgorithmType queryingAlgorithm, IDictionary<Guid, int> categorySortPriority)
        {
            var queryingRestrictions = GetUserFilteringCriterias();
            queryingRestrictions.AllowedStatuses = GetAllowedStatuses(allowedStatusesConfigKey);
            queryingRestrictions.ShowLastCompletedOrCanceledStatus = showLastCompletedOrCanceledStatus;
            queryingRestrictions.QueryingAlgorithm = queryingAlgorithm;
            var chartConfig = new SpentTimeChartConfig
            {
                TaskTypesToInclude = GetTaskTypes(spentTimeChartTypesConfigKey)
            };
            var chartModel = spentTimeChartDataBuilder.Build(queryingRestrictions, chartConfig, period, categorySortPriority);
            return chartModel;
        }

        private ITextFileResultModel GetCsvTextFileResult<T>(IEnumerable<T> data, CsvClassMap<T> csvProfile, string fileName)
        {
            return new TextFileResultModel
            {
                FileName = fileName,
                Content = csvHelper.ToCsv(data, csvProfile)
            };
        }

        private IEnumerable<AbsenceInfo> GetAbsencesInfoData(IAbsencesStatisticsCsvRequest absencesInfoRequest)
        {
            IDictionary<string, string> translationList = translationService.Get(new List<string> { OtherReasonKey }, LanguageKey.Default);
            IEnumerable<IEmployeeAbsenceInfoModel> absencesList = absenceInfoService.GetlByIds(absencesInfoRequest.AbsencesIdList);
            IEnumerable<IAbsenceDataModel> absencesListWithTime = GetAbsencesListWithTime(absencesList, absencesInfoRequest.StartDate, absencesInfoRequest.EndDate);
            IEnumerable<AbsenceInfo> absenceInfoList = absencesListWithTime
                .GroupBy(x => x.ReasonId)
                .Select(y => new AbsenceInfo
                {
                    AbsenceReason = y.Key == null ? translationList[OtherReasonKey] : y.First().Reason,
                    Hours = y.Sum(x => x.SpentTime)
                })
                .ToList();

            return absenceInfoList.OrderBy(x => x.AbsenceReason == translationList[OtherReasonKey]);
        }

        private IEnumerable<RejectionReasonInfo> GetCancelingReasonInfo(IEnumerable<Guid> dayAssignIds)
        {
            List<IDayAssign> dayAssign = dayAssignService.GetByIds(dayAssignIds).ToList();
            IEnumerable<ICancelingReasonDataModel> cancelingReasonModelList = GetRejectionReasonDataModels(dayAssign);
            return Mapper.Map<IEnumerable<RejectionReasonInfo>>(cancelingReasonModelList);
        }

        private IEnumerable<ICancelingReasonDataModel> GetRejectionReasonDataModels(List<IDayAssign> dayAssignList)
        {
            IEnumerable<Guid> dayAssignIds = dayAssignList.Select(x => x.Id);
            IEnumerable<string> jobIds = dayAssignList.Select(x => x.JobId);
            IEnumerable<Guid> housingDepartmentIds = dayAssignList.Select(x => x.DepartmentId).Distinct();

            IEnumerable<IJobStatusLogModel> jobStatusLogs = jobStatusLogService.GetLogsByDayAssignIds(dayAssignIds);
            IEnumerable<IJob> jobs = jobService.GetByIds(jobIds).ToList();
            IEnumerable<IHousingDepartmentModel> housingDepartments = managementDepartmentService.GetHousingDepartments(housingDepartmentIds);

            IEnumerable<Guid> creatorIds = jobs.Select(x => x.CreatorId).Distinct();
            Dictionary<Guid, string> creatorNames = memberService.GetMemberNames(creatorIds);

            IEnumerable<ICancelingReasonDataModel> result = dayAssignList
                .Select(x => GetCancelingReasonDataModel(x, jobStatusLogs, jobs, creatorNames, housingDepartments))
                .Where(cancelingReasonDataModel => cancelingReasonDataModel != null);

            return result;
        }

        private IEnumerable<IAbsenceDataModel> GetAbsencesListWithTime(IEnumerable<IEmployeeAbsenceInfoModel> absenceList, DateTime filterStartDate,
            DateTime filterEndDate)
        {
            List<IAbsenceDataModel> absencesListWithTime = new List<IAbsenceDataModel>();

            foreach (var absence in absenceList)
            {
                DateTime startDate = absence.StartDate > filterStartDate ? absence.StartDate : filterStartDate;
                DateTime endDate = absence.EndDate < filterEndDate ? absence.EndDate : filterEndDate;
                absencesListWithTime.Add(new AbsenceDataModel
                {
                    AbsenceId = absence.Id,
                    ReasonId = absence.AbsenceTemplateId,
                    Reason = absence.Text,
                    SpentTime = GetAbsenceTimeInHours(startDate, endDate)
                });
            }

            return absencesListWithTime;
        }

        private decimal GetAbsenceTimeInHours(DateTime startDate, DateTime endDate)
        {
            IDictionary<int, int> weekDaysWorkingMinutes = appSettingHelper.GetDictionaryAppSetting<int, int>(Constants.AppSetting.DaysWorkingMinutes);
            decimal totalMinutes = CalendarHelper.GetDatesRange(startDate, endDate).Sum(day => weekDaysWorkingMinutes[day.GetWeekDayNumber()]);
            return totalMinutes / Constants.Common.MinutesInOneHour;
        }

        private string GetChartCsvFileName(string chartName, string rangeDateString)
        {
            string chartNameTranslationKey = TranslationKeySuffix + chartName;
            string fileNameSuffix = translationService.Get(new[] { chartNameTranslationKey }, LanguageKey.Default)[chartNameTranslationKey];
            return $"{fileNameSuffix}_{rangeDateString}{FileExstension}";
        }

        private IDictionary<string, IEnumerable<T>> GetTaskGroupings<T>(string configurationKey)
        {
            var result = appSettingHelper.GetFromJson<IDictionary<string, IEnumerable<T>>>(configurationKey);
            return result;
        }

        private IEnumerable<JobTypeEnum> GetTaskTypes(string configurationKey)
        {
            var result = appSettingHelper.GetFromJson<IEnumerable<JobTypeEnum>>(configurationKey);
            return result;
        }

        private IChartDataQueryingRestrictions GetUserFilteringCriterias()
        {
            var currentUser = memberService.GetCurrentUser();
            IDictionary<Guid, IEnumerable<Guid>> managementToHousingDepartmentsRelation;
            if (currentUser.IsAdmin())
            {
                var managementsIds = managementDepartmentService.GetAllManagements().Select(m => m.Id);
                managementToHousingDepartmentsRelation = managementDepartmentService.GetManagementsToHousingDepartmentsRelation(managementsIds);
            }
            else
            {
                var userManagementDepartmentIds = currentUser.ManagementsToActiveRolesRelation[currentUser.CurrentRole];
                managementToHousingDepartmentsRelation = managementDepartmentService.GetManagementsToHousingDepartmentsRelation(userManagementDepartmentIds);
            }

            return new ChartDataQueryingRestrictions
            {
                AccessibleManagementToHousingDepartmentsRelation = managementToHousingDepartmentsRelation,
                CurrentMemberRole = currentUser.CurrentRole
            };
        }

        private IEnumerable<JobStatus> GetAllowedStatuses(string configurationKey)
        {
            var result = appSettingHelper.GetFromJson<IEnumerable<JobStatus>>(configurationKey);
            return result;
        }

        private IEnumerable<Guid> GetEmployeeIdListForCurrentUser()
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            return currentUser.IsAdmin()
                ? memberService.GetAll().Select(x => x.MemberId)
                : memberService.GetEmployeesByManagementDepartment(currentUser.ActiveManagementDepartmentId.Value).Select(x => x.MemberId);
        }

        private ICancelingReasonDataModel GetCancelingReasonDataModel(IDayAssign dayAssign, IEnumerable<IJobStatusLogModel> jobStatusLogs, IEnumerable<IJob> jobs,
            Dictionary<Guid, string> creatorNames, IEnumerable<IHousingDepartmentModel> housingDepartments)
        {
            List<IJobStatusLogModel> jobStatusLogList = jobStatusLogs.Where(x => x.DayAssignId == dayAssign.Id).ToList();

            IJobStatusLogModel unassignedLog = jobStatusLogList
                .OrderByDescending(log => log.Date)
                .FirstOrDefault(x => x.StatusId == JobStatus.Rejected);

            if (unassignedLog == null)
            {
                return null;
            }

            IJob job = jobs.First(x => x.Id == dayAssign.JobId);
            IHousingDepartmentModel housingDepartment = housingDepartments.First(x => x.Id == dayAssign.DepartmentId);

            decimal totalTimeSpent = (decimal) jobStatusLogList.SelectMany(x => x.TimeLogList).Sum(y => y.SpentTime.TotalHours);
            Guid managementDepartmentId = managementDepartmentService.GetParentManagementId(dayAssign.DepartmentId);
            JobAddress addressModel = job.AddressList.FirstOrDefault(x => x.HousingDepartmentId == dayAssign.DepartmentId);

            return new CancelingReasonDataModel
            {
                HousingDepartmentId = dayAssign.DepartmentId,
                DayAssignId = dayAssign.Id,
                ReasonId = unassignedLog.CancelingId ?? Guid.Empty,
                ManagementDepartmentId = managementDepartmentId,
                RejectionDate = unassignedLog.Date.ToString(Constants.DateTime.CsvDateFormat),
                Reason = unassignedLog.CancelingReason,
                SpentTime = totalTimeSpent,
                JobId = job.Id,
                Title = job.Title,
                HousingDepartmentName = housingDepartment.DisplayName,
                Address = addressModel?.Address,
                CreatorName = creatorNames[job.CreatorId],
                TenantType = dayAssign.TenantType.HasValue ? dayAssign.TenantType.Value.GetTenantTypeTaskLabel(translationService) : string.Empty,
                StatusName = dayAssign.StatusId.GetStatusLabel(translationService)
            };
        }

        private IDictionary<Guid, int> GetCategorySortPriority(ref int priority, IDictionary<Guid, int> categorySortPriority = null, IEnumerable<ICategoryModel> tree = null)
        {
            categorySortPriority = categorySortPriority ?? new Dictionary<Guid, int>();
            tree = tree ?? categoryService.GetTree();

            foreach (var leaf in tree)
            {
                if (leaf.Children.HasValue())
                {
                    GetCategorySortPriority(ref priority, categorySortPriority, leaf.Children);
                }
                else
                {
                    categorySortPriority.Add(leaf.Id, ++priority);
                }
            }

            return categorySortPriority;
        }
    }
}