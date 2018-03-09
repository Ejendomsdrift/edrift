using FileStorage.Contract.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Web.Models.Task
{
    public class UploadFileViewModel
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public DateTime CreationDate { get; set; }

        public string Path { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UploadedContentEnum ContentType { get; set; }

        public string Description { get; set; }

        public Guid UploaderId { get; set; }

        public MemberViewModel Uploader { get; set; }
    }
}