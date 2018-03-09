using System.Collections.Generic;
using ManagementDepartmentCore.Contract.Interfaces;

namespace MemberCore.Contract.Interfaces
{
    public interface ICurrentUserContextModel
    {
        IMemberModel MemberModel { get; set; }
        IEnumerable<IManagementDepartmentModel> ManagementDepartments { get; set; }
        IManagementDepartmentModel SelectedManagementDepartment { get; set; }
    }
}