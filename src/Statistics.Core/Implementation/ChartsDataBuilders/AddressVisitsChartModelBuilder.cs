using ManagementDepartmentCore.Contract.Interfaces;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.ChartsDataBuilders;
using Statistics.Contract.Interfaces.Models;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Interfaces;
using Translations.Models;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Services;

namespace Statistics.Core.Implementation.ChartsDataBuilders
{
    public class AddressVisitsChartModelBuilder : IAddressVisitsChartModelBuilder
    {
        private const int MaxAmountOfVisits = 6;
        private const string MaxAmountOfVisitsLabel = "6+";
        private const string EmptyAddressGroupLabel = "emptyAddressGroupLabel";

        private readonly IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector;
        private readonly JobService jobService;
        private readonly ITranslationService translationService;
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IManagementDepartmentService managementDepartmentService;

        public AddressVisitsChartModelBuilder(
            IDayAssignsTimeSpanSelector dayAssignsTimeSpanSelector,
            JobService jobService,
            ITranslationService translationService,
            IJobStatusLogService jobStatusLogService,
            IManagementDepartmentService managementDepartmentService)
        {
            this.dayAssignsTimeSpanSelector = dayAssignsTimeSpanSelector;
            this.jobService = jobService;
            this.translationService = translationService;
            this.jobStatusLogService = jobStatusLogService;
            this.managementDepartmentService = managementDepartmentService;
        }

        public IChartData<IAddressStatisticInfo> Build(IChartDataQueryingRestrictions restrictions, IAddressVisitsChartConfig config, ITimePeriod period,
            bool showLastCompletedOrCanceledStatus)
        {
            List<IDayAssign> dayAssignList = dayAssignsTimeSpanSelector.Get(period, restrictions).ToList();
            IEnumerable<string> jobIds = dayAssignList.Select(d => d.JobId).Distinct();
            IEnumerable<Guid> dayAssignIds = dayAssignList.Select(x => x.Id);

            IEnumerable<IJob> jobs = jobService.GetByIds(jobIds);
            IEnumerable<IJob> filteredJobs = jobs.Where(job => config.TaskTypesToInclude.Contains(job.JobTypeId));

            Dictionary<Guid, IJobStatusLogModel> jobStatusLog = jobStatusLogService.GetStatusLogModelList(dayAssignIds, showLastCompletedOrCanceledStatus)
                    .ToDictionary(log => log.DayAssignId, log => log);

            List<Tuple<string, JobAddress>> addressToJobRelation = filteredJobs
                    .SelectMany(job => job.AddressList?.Select(a => new Tuple<string, JobAddress>(job.Id, a)) ?? Enumerable.Empty<Tuple<string, JobAddress>>())
                    .ToList();

            List<Tuple<Guid, JobAddress>> addressToDayAssignRelationList = GetAddressToDayAssignRelationList(dayAssignList, addressToJobRelation);
            List<AddressStatisticInfo> addressStatisticInfoList = GetAddressStatisticInfoList(addressToDayAssignRelationList, jobStatusLog);

            var result = (IChartData<IAddressStatisticInfo>)new ChartData<IAddressStatisticInfo>
            {
                Data = GetAddressStatistic(addressStatisticInfoList)
            };

            return result;
        }

        private List<Tuple<Guid, JobAddress>> GetAddressToDayAssignRelationList(List<IDayAssign> dayAssignList, List<Tuple<string, JobAddress>> addressToJobRelation)
        {
            var addressToDayAssignRelationList = new List<Tuple<Guid, JobAddress>>();

            foreach (var dayAssign in dayAssignList)
            {
                var jobAddress = addressToJobRelation.FirstOrDefault(t => t.Item1 == dayAssign.JobId && t.Item2.HousingDepartmentId == dayAssign.DepartmentId)?.Item2;

                addressToDayAssignRelationList.Add(new Tuple<Guid, JobAddress>(dayAssign.Id, jobAddress));
            }

            IEnumerable<Tuple<Guid, JobAddress>> result = addressToDayAssignRelationList.Where(x => x.Item2 != null);

            return result.ToList();
        }

        private List<AddressStatisticInfo> GetAddressStatisticInfoList(
            List<Tuple<Guid, JobAddress>> addressToDayAssignRelationList, Dictionary<Guid, IJobStatusLogModel> jobStatusLog)
        {
            var result = new List<AddressStatisticInfo>();

            IEnumerable<Guid> housingDepartmentIds = addressToDayAssignRelationList.Select(x => x.Item2.HousingDepartmentId);
            List<IManagementDepartmentModel> managementDepartments = managementDepartmentService.GetByHousingDepartmentIds(housingDepartmentIds);
            IDictionary<Guid, string> housingDepartmentsName = managementDepartments.SelectMany(m => m.HousingDepartmentList).Where(h => !h.IsDeleted).ToDictionary(h => h.Id, v => v.DisplayName);


            foreach (var address in addressToDayAssignRelationList)
            {
                IJobStatusLogModel log;
                IManagementDepartmentModel managementDepartment = managementDepartments.First(x => x.HousingDepartmentList.Any(y => y.Id == address.Item2.HousingDepartmentId));

                result.Add(new AddressStatisticInfo
                {
                    Address = address.Item2.Address,
                    HousingDepartmentId = address.Item2.HousingDepartmentId,
                    HousingDepartmentName = housingDepartmentsName[address.Item2.HousingDepartmentId],
                    SpentTime = jobStatusLog.TryGetValue(address.Item1, out log) ? log.TotalSpentTime : default(decimal),
                    ManagementDepartmentId = managementDepartment.Id
                });
            }

            return result;
        }

        private IEnumerable<AddressStatisticInfo> GetAddressStatistic(IEnumerable<AddressStatisticInfo> addresses)
        {
            var result = new List<AddressStatisticInfo>();

            string emptyAddressGroupLabel = translationService.Get(new[] { EmptyAddressGroupLabel }, LanguageKey.Default)[EmptyAddressGroupLabel];
            var groupedAddresses = addresses.GroupBy(address => address.Address);

            foreach (var address in groupedAddresses)
            {
                AddressStatisticInfo info = address.First();
                int visitsAmount = address.Count();

                result.Add(new AddressStatisticInfo
                {
                    ManagementDepartmentId = info.ManagementDepartmentId,
                    HousingDepartmentId = info.HousingDepartmentId,
                    HousingDepartmentName = info.HousingDepartmentName,
                    Address = string.IsNullOrEmpty(info.Address) ? emptyAddressGroupLabel : info.Address,
                    Amount = visitsAmount >= MaxAmountOfVisits ? MaxAmountOfVisitsLabel : visitsAmount.ToString(),
                    SpentTime = address.Sum(a => a.SpentTime)
                });
            }

            return result;
        }
    }
}