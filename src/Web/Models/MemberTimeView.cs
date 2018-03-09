using System.Collections.Generic;
using MemberCore.Contract.Interfaces;

namespace Web.Models
{
    public class MemberTimeView
    {
        public IMemberModel MemberModel { get; set; }
        public IEnumerable<TimeViewModel> WeekTimeView { get; set; }
        public TimeViewModel WeekTotal { get; set; }
    }
}