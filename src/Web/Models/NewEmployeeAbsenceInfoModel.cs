using System;

namespace Web.Models
{
    public class NewEmployeeAbsenceInfoModel
    {
        public Guid MemberId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Text { get; set; }

        public Guid? AbsenceTemplateId { get; set; }
    }
}