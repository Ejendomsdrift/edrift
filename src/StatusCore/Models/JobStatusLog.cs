using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository.Contract.Interfaces;
using StatusCore.Contract.Enums;

namespace StatusCore.Models
{
    [BsonIgnoreExtraElements]
    public class JobStatusLog : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DayAssignId { get; set; }

        public JobStatus StatusId { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public Guid MemberId { get; set; }

        public Guid PreviousStatusId { get; set; }

        public Guid? CancellingId { get; set; }

        public IEnumerable<TimeLog> TimeLogList { get; set; }

        public Guid[] UploadedFileIds { get; set; }

    }
}
