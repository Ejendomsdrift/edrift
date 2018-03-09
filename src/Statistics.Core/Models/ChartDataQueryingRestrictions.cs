using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using Statistics.Contract.Enums;
using Statistics.Contract.Interfaces;
using StatusCore.Contract.Enums;

namespace Statistics.Core.Models
{
    class ChartDataQueryingRestrictions : IChartDataQueryingRestrictions
    {
        public RoleType CurrentMemberRole { get; set; }
        public IDictionary<Guid, IEnumerable<Guid>> AccessibleManagementToHousingDepartmentsRelation { get; set; }
        public IEnumerable<JobStatus> AllowedStatuses { get; set; }
        public QueryingAlgorithmType QueryingAlgorithm { get; set; }
        public bool ShowLastCompletedOrCanceledStatus { get; set; }
    }
}