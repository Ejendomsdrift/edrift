using System;
using FileStorage.Contract.Enums;
using Infrastructure.EventSourcing.Implementation;

namespace FileStorage.Contract.Events
{
    public class UploadDataUploaded : EventBase
    {
        public Guid JobAssignId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string MimeType { get; set; }

        public UploadedContentEnum ContentType { get; set; }

        public DateTime UploadedOn { get; set; }

        public Guid UploaderId { get; set; }
    }
}