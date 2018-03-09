using System;
using System.Collections.Generic;
using System.Linq;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Enums;

namespace CancellingTemplatesCore.Models
{
    public class CancellingTemplate : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsDeleted { get; set; }

        public bool IsCoordinatorReason { get; set; }

        public string Text { get; set; }

        public IEnumerable<JobTypeEnum> JobTypeList { get; set; } = Enumerable.Empty<JobTypeEnum>();
    }
}