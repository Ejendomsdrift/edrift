using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace StatusCore.Contract.Interfaces
{
    public interface IJobStatusLogModel
    {
        Guid Id { get; set; }

        Guid DayAssignId { get; set; }

        JobStatus StatusId { get; set; }

        string Comment { get; set; }

        DateTime Date { get; set; }

        Guid MemberId { get; set; }

        Guid PreviousStatusId { get; set; }

        Guid? CancelingId { get; set; }

        string CancelingReason { get; set; }

        IEnumerable<ITimeLogModel> TimeLogList { get; set; }

        Guid[] UploadedFileIds { get; set; }

        bool IsCommentExistInAnyStatus { get; set; }

        decimal TotalSpentTime { get; }
    }
}
