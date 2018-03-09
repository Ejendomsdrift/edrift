using System;

namespace Web.Models
{
    public class MemberModelWithTimeView
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public TimeViewModel TimeView { get; set; }
    }
}