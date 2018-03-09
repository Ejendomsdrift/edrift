using System.Collections.Generic;
using MemberCore.Contract.Interfaces;

namespace Web.Models
{
    public class CurrentUserContextViewModel
    {
        public IMemberModel MemberModel { get; set; }
        public IEnumerable<ManagementViewModel> ManagementDepartments { get; set; }
        public ManagementViewModel SelectedManagementDepartment { get; set; }
    }
}