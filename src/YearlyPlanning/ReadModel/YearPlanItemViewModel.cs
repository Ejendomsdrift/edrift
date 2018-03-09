using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.ReadModel
{
    public class YearPlanItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Level { get; set; }
        public bool IsTask { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsAssigned { get; set; }
        public bool ByCoordinator { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public string Address { get; set; }
        public bool IsGroupedJob { get; set; }
        public bool IsParentGroupedJob { get; set; }
        public IDictionary<Guid, string> AddressListForParentTask { get; set; } = new Dictionary<Guid, string>();
        public List<YearPlanWeekData> Weeks { get; set; } = new List<YearPlanWeekData>();
        public IEnumerable<Guid> AssignedHousingDepartmentIdList { get; set; } = Enumerable.Empty<Guid>();
    }
}
