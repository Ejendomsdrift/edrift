using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using Infrastructure.Enums;
using SecurityCore.Contract.Enums;

namespace SecurityCore.Contract.Interfaces
{
    public interface ISecurityQuery
    {
        SecurityPages? Page { get; set; }
        IEnumerable<string> KeyList { get; set; }
        string GroupName { get; set; }
        IMemberModel Member { get; set; }
        RoleType? CreatorRole { get; set; }
        JobStatus? DayAssignStatus { get; set; }
        DateTime? DayAssignDate { get; set; }
        PlatformType? CurrentPlatformType { get; set; }
        bool IsGroupedTask { get; set; }
    }
}
