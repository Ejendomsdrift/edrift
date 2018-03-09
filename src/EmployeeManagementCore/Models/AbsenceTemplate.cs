using System;
using MongoRepository.Contract.Interfaces;

namespace AbsenceTemplatesCore.Models
{
    public class AbsenceTemplate : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsDeleted { get; set; }

        public string Text { get; set; }
    }
}