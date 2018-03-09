using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public class YearPlanItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Level { get; set; }
        public bool IsTask { get; set; }
        public bool IsChildTask { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsAssigned { get; set; }
        public bool ByCoordinator { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public string Address { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<JobAddress> AddressList { get; set; } = Enumerable.Empty<JobAddress>();
        public List<RelationGroupModel> RelationGroupList { get; set; } = new List<RelationGroupModel>();
        public List<YearPlanWeekData> Weeks { get; set; } = new List<YearPlanWeekData>();
        public IEnumerable<YearPlanItem> Tasks { get; set; } = Enumerable.Empty<YearPlanItem>();
        public IEnumerable<Guid> AssignedHousingDepartmentIdList { get; set; } = Enumerable.Empty<Guid>();
    }
}
