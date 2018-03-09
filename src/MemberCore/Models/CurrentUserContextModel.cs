using System.Collections.Generic;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;

namespace MemberCore.Models
{
    public class CurrentUserContextModel : ICurrentUserContextModel
    {
        public IMemberModel MemberModel { get; set; }
        public IEnumerable<IManagementDepartmentModel> ManagementDepartments { get; set; }
        public IManagementDepartmentModel SelectedManagementDepartment { get; set; }
    }
}
