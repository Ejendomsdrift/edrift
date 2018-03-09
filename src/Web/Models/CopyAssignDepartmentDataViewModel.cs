using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Web.Models
{
    public class CopyAssignDepartmentDataViewModel
    {
        public string Id { get; set; }

        public string GlobalDepartmentId { get; set; }

        public string LocalDepartmentId { get; set; }

        public IEnumerable<WeekModel> Weeks { get; set; }

        public bool IsLocked { get; set; }
    }
}