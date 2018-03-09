using System;
using MongoRepository.Contract.Interfaces;

namespace YearlyPlanning.ReadModel
{
    public class GuideComment: IEntity
    {
        public Guid Id { get; set; }

        public string JobId { get; set; }

        public Guid DayAssignId { get; set; }

        public Guid MemberId { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

    }
}
