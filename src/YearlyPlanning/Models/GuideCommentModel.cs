using System;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class GuideCommentModel: IGuideCommentModel
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
