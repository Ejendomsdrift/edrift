using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Web.Models.Task
{
    public class JobLocationViewModel
    {
        public bool IsGroupedJob { get; set; }
        public Guid? GroupedJobHousingDepartmentId { get; set; }
        public IEnumerable<IdValueModel<string, string>> GroupedTasks { get; set; }
        public IEnumerable<IdValueModel<Guid, string>> Departments { get; set; }
        public Dictionary<string, Dictionary<Guid, string>> AddressList { get; set; } = new Dictionary<string, Dictionary<Guid, string>>();
    }
}