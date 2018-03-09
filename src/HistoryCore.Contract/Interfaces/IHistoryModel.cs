using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;

namespace HistoryCore.Contract.Interfaces
{
    public interface IHistoryModel
    {
        DateTime JobCreationDate { get; set; }

        string ResidentName { get; set; }

        DateTime ChangeStatusDate { get; set; }

        string Title { get; set; }

        string JobComment { get; set; }

        JobStatus JobStatus { get; set; }

        string ChangeStatusComment { get; set; }

        Guid JobHousingDepartmentId { get; set; }

        Guid DayAssignId { get; set; }

        string JobId { get; set; }

        string UserNameWhoChangedStatus { get; set; }

        double ReportedHours { get; set; }

        double ReportedMinutes { get; set; }

        string Address { get; set; }

        List<IFileModel> UploadedFiles { get; set; }

        string CancellationReason { get; set; }

    }
}
