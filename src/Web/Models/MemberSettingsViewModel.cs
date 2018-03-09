using System.Collections.Generic;
using MemberCore.Contract.Interfaces;

namespace Web.Models
{
    public class MemberSettingsViewModel
    {
        public IMemberModel MemberModel { get; set; }
        public IEnumerable<ManagementViewModel> Departments { get; set; }
    }
}