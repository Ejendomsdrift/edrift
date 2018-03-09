using ManagementDepartmentCore.Contract.Interfaces;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IFormattedJobAssign
    {
        JobAssign GlobalAssign { get; set; }
        IEnumerable<JobAssign> Assigns { get; set; }
        IEnumerable<IHousingDepartmentModel> AssignedDepartments { get; set; }
    }
}
