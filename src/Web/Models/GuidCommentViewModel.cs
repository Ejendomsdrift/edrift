using System;

namespace Web.Models
{
    public class GuidCommentViewModel
    {
        public Guid? Id { get; set; }

        public string JobId { get; set; }

        public Guid DayAssignId { get; set; }

        public Guid MemberId { get; set; }

        public string Comment { get; set; }

        public string MemberAvatar { get; set; }

        public string MemberName { get; set; }

        public DateTime Date { get; set; }
    }
}