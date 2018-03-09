using System;
using System.Collections.Generic;
using StatusCore.Contract.Enums;

namespace Web.Models
{
    public class ChangeJobStatusModel
    {
        public Guid DayAssignId { get; set; }

        public string ChangeStatusComment { get; set; }

        public List<MemberSpentTimeModel> Members { get; set; }

        public JobStatus NewJobStatus { get; set; }

        public Guid? SelectedCancellingId { get; set; }

        public Guid[] UploadedFileIds { get; set; }
    }
}