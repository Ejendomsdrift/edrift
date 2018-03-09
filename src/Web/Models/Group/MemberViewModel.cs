using System;

namespace Web.Models.Group
{
    public class MemberViewModel
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int? SpentHours { get; set; }
        public int? SpentMinutes { get; set; }
        public bool HasSpentTime { get; set; }
    }
}