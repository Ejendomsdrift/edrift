using System;
using MongoRepository.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class EmployeeAbsenceInfo : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsDeleted { get; set; }

        public Guid MemberId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Text { get; set; }

        public Guid? AbsenceTemplateId { get; set; }
    }
}