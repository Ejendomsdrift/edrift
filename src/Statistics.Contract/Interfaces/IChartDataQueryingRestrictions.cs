using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using Statistics.Contract.Enums;
using StatusCore.Contract.Enums;

namespace Statistics.Contract.Interfaces
{
    public interface IChartDataQueryingRestrictions
    {
        IDictionary<Guid, IEnumerable<Guid>> AccessibleManagementToHousingDepartmentsRelation { get; set; }
        RoleType CurrentMemberRole { get; set; }
        IEnumerable<JobStatus> AllowedStatuses { get; set; }
        QueryingAlgorithmType QueryingAlgorithm { get; set; }
        bool ShowLastCompletedOrCanceledStatus { get; set; }
}
}