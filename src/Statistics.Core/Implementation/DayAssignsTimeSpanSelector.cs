using System;
using System.Collections.Generic;
using System.Linq;
using MemberCore.Contract.Enums;
using Statistics.Contract.Enums;
using Statistics.Contract.Interfaces;
using StatusCore.Contract.Interfaces;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation
{
    public class DayAssignsTimeSpanSelector : IDayAssignsTimeSpanSelector
    {
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IDayAssignService dayAssignService;

        public DayAssignsTimeSpanSelector(
            IJobStatusLogService jobStatusLogService,
            IDayAssignService dayAssignService)
        {
            this.jobStatusLogService = jobStatusLogService;
            this.dayAssignService = dayAssignService;
        }


        public IEnumerable<IDayAssign> Get(ITimePeriod period, IChartDataQueryingRestrictions restrictions)
        {
            switch (restrictions.QueryingAlgorithm)
            {
                case QueryingAlgorithmType.ByCompletitionDateCriteria:
                    return GetByCompletionDateCriteria(period, restrictions);
                case QueryingAlgorithmType.ByDateCriteria:
                    return GetByDateCriteria(period, restrictions);
                default:
                    throw new ArgumentOutOfRangeException($"No such querying algorithm {restrictions.QueryingAlgorithm}");
            }
        }

        private IEnumerable<IDayAssign> GetByCompletionDateCriteria(ITimePeriod period, IChartDataQueryingRestrictions restrictions)
        {
            var dayAssigns = Enumerable.Empty<IDayAssign>();

            List<Guid> dayAssignIdList = jobStatusLogService.GetDayAssignIds(period.StartDate, period.EndDate, restrictions.AllowedStatuses).ToList();

            if (restrictions.CurrentMemberRole == RoleType.Coordinator)
            {
                IEnumerable<Guid> allowedHousingDepartments = restrictions.AccessibleManagementToHousingDepartmentsRelation.SelectMany(pair => pair.Value);
                dayAssigns = dayAssignService.GetForStatisticByIdsWithRestrictions(dayAssignIdList, allowedHousingDepartments);
            }
            else
            {
                dayAssigns = dayAssignService.GetForStatisticByIdsWithRestrictions(dayAssignIdList);
            }

            return dayAssigns;
        }

        private IEnumerable<IDayAssign> GetByDateCriteria(ITimePeriod period, IChartDataQueryingRestrictions restrictions)
        {
            var result = Enumerable.Empty<IDayAssign>();

            if (restrictions.CurrentMemberRole == RoleType.Coordinator)
            {
                var allowedHousingDepartments = restrictions.AccessibleManagementToHousingDepartmentsRelation.SelectMany(pair => pair.Value);
                result = dayAssignService.GetForStatisticTimeSpan(period.StartDate, period.EndDate, restrictions.AllowedStatuses, allowedHousingDepartments);
            }
            else
            {
                result = dayAssignService.GetForStatisticTimeSpan(period.StartDate, period.EndDate, restrictions.AllowedStatuses);
            }

            return result;
        }
    }
}