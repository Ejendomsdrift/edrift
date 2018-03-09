using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Web.Models.Task
{
    public class FormattedJobAssignViewModel
    {
        public JobAssignViewModel GlobalAssign { get; set; }
        public IEnumerable<JobAssignViewModel> Assigns { get; set; }
        public IEnumerable<IHousingDepartmentModel> AssignedDepartments { get; set; }
        public IEnumerable<IdValueModel<string, string>> AddressList { get; set; }
        public IMemberModel CurrentUser { get; set; }
        public bool IsGroupedJob { get; set; }
        public bool IsChildJob { get; set; }
    }
}