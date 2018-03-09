using System;
using System.Collections.Generic;
using Infrastructure.Enums;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using Web.Enums;

namespace Web.Models.Security
{
    public class SequrityQueryViewModel
    {
        public string JobId { get; set; }
        public Pages? Page { get; set; }
        public IEnumerable<string> KeyList { get; set; }
        public string GroupName { get; set; }
        public IMemberModel Member { get; set; }
        public RoleType? CreatorRole { get; set; }
        public Guid? DayAssignId { get; set; }
        public JobStatus? DayAssignStatus { get; set; }
        public DateTime? DayAssignDate { get; set; }
        public PlatformType? CurrentPlatformType { get; set; }
        public bool IsGroupedTask { get; set; }
    }
}