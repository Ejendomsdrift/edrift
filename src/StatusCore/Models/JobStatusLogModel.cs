using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Extensions;
using StatusCore.Contract.Interfaces;

namespace StatusCore.Models
{
    public class JobStatusLogModel : IJobStatusLogModel
    {
        public Guid Id { get; set; }

        public Guid DayAssignId { get; set; }

        public JobStatus StatusId { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public Guid MemberId { get; set; }

        public Guid PreviousStatusId { get; set; }

        public Guid? CancelingId { get; set; }

        public string CancelingReason { get; set; }

        public IEnumerable<ITimeLogModel> TimeLogList { get; set; }

        public Guid[] UploadedFileIds { get; set; }

        public bool IsCommentExistInAnyStatus { get; set; }

        public decimal TotalSpentTime => TimeLogList.Sum(t => t.SpentTime.TotalMinutes).MinutesToHours();
    }
}