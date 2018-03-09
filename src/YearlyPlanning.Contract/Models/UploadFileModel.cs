using FileStorage.Contract.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YearlyPlanning.Contract.Models
{
    public class UploadFileModel
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Path { get; set; }
        [BsonRepresentation(BsonType.String)]
        public UploadedContentEnum ContentType { get; set; }
        public string Description { get; set; }
        public Guid UploaderId { get; set; }
    }
}