using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using ManagementDepartmentCore.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class FormattedJobAssign: IFormattedJobAssign
    {
        public FormattedJobAssign()
        {
            Assigns = Enumerable.Empty<JobAssign>();
            AssignedDepartments = Enumerable.Empty<IHousingDepartmentModel>();
        }

        public JobAssign GlobalAssign { get; set; }
        public IEnumerable<JobAssign> Assigns { get; set; }
        public IEnumerable<IHousingDepartmentModel> AssignedDepartments { get; set; }
        public bool IsUserTaskOwner { get; set; }
        public bool IsCurrentUserAdmin { get; set; }
    }
}
