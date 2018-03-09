using System;

namespace Web.Models
{
    public class MemberSettingsUpdateModel
    {
        public Guid MemberId { get; set; }
        public int DaysAhead { get; set; }
        public Guid? Department { get; set; }
    }
}