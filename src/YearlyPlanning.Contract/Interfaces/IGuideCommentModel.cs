using System;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IGuideCommentModel
    {
        Guid? Id { get; set; }

        string JobId { get; set; }

        Guid DayAssignId { get; set; }

        Guid MemberId { get; set; }

        DateTime Date { get; set; }

        string Comment { get; set; }

        string MemberAvatar { get; set; }

        string MemberName { get; set; }
    }
}
