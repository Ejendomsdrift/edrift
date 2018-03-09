using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace StatusCore.Contract.Interfaces
{
    public interface IJobStatusLog
    {
        Guid Id { get; set; }

        Guid DayAssignId { get; set; }

        JobStatus StatusId { get; set; }

        string Comment { get; set; }

        DateTime Date { get; set; }

        Guid MemberId { get; set; }

        Guid PreviousStatusId { get; set; }

        IEnumerable<ITimeLog> TimeLogList { get; set; }
    }
}
