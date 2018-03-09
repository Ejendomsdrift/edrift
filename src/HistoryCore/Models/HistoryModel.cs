using System;
using System.Collections.Generic;
using HistoryCore.Contract.Interfaces;
using StatusCore.Contract.Enums;

namespace HistoryCore.Models
{
    public class HistoryModel: IHistoryModel
    {
        public DateTime JobCreationDate { get; set; }

        public string ResidentName { get; set; }

        public DateTime ChangeStatusDate { get; set; }

        public string Title { get; set; }

        public string JobComment { get; set; }

        public JobStatus JobStatus { get; set; }

        public string ChangeStatusComment { get; set; }

        public Guid JobHousingDepartmentId { get; set; }

        public Guid DayAssignId { get; set; }

        public string JobId { get; set; }

        public string UserNameWhoChangedStatus { get; set; }

        public double ReportedHours { get; set; }

        public double ReportedMinutes { get; set; }

        public string Address { get; set; }

        public List<IFileModel> UploadedFiles { get; set; }
        public string CancellationReason { get; set; }
    }
}
