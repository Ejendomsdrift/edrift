using System;
using FileStorage.Contract.Enums;
using FileStorage.Contract.Events;
using Infrastructure.EventSourcing.Implementation;

namespace FileStorage
{
    public class UploadData : AggregateBase
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string MimeType { get; set; }

        public string Description { get; set; }

        public UploadedContentEnum ContentType { get; set; }

        public DateTime UploadedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Guid JobAssignId { get; set; }

        public Guid DayAssignId { get; set; }

        public Guid UploaderId { get; set; }

        public UploadData()
        {
            RegisterTransition<UploadDataUploaded>(Apply);
            RegisterTransition<UploadDataDescriptionChanged>(Apply);
            RegisterTransition<UploadDataDeleted>(Apply);
            RegisterTransition<DayAssignUploadDataUploaded>(Apply);
            RegisterTransition<UploadDataDayAssignDeleted>(Apply);
        }

        public UploadData(Guid dayAssignId, Guid uploaderId, UploadedContentEnum contentType, string id, string originalName, string path, string mimeType) : this()
        {
            Id = id;

            RaiseEvent(new DayAssignUploadDataUploaded
            {
                Name = originalName,
                Path = path,
                MimeType = mimeType,
                ContentType = contentType,
                UploadedOn = DateTime.UtcNow,
                DayAssignId = dayAssignId,
                UploaderId = uploaderId,
            });
        }

        private void Apply(DayAssignUploadDataUploaded e)
        {
            Id = e.SourceId;
            Name = e.Name;
            Path = e.Path;
            MimeType = e.MimeType;
            ContentType = e.ContentType;
            UploadedOn = e.UploadedOn;
            DayAssignId = e.DayAssignId;
            UploaderId = e.UploaderId;
        }

        public UploadData(UploadedContentEnum contentType, string id, string originalName, string path, string mimeType, Guid jobAssignId, Guid uploaderId) : this()
        {
            Id = id;

            RaiseEvent(new UploadDataUploaded
            {
                Name = originalName,
                Path = path,
                MimeType = mimeType,
                ContentType = contentType,
                UploadedOn = DateTime.UtcNow,
                JobAssignId = jobAssignId,
                UploaderId = uploaderId,
            });
        }

        private void Apply(UploadDataUploaded e)
        {
            Id = e.SourceId;
            Name = e.Name;
            Path = e.Path;
            MimeType = e.MimeType;
            ContentType = e.ContentType;
            UploadedOn = e.UploadedOn;
            JobAssignId = e.JobAssignId;
            UploaderId = e.UploaderId;
        }

        public void ChangeDescription(string description)
        {
            RaiseEvent(new UploadDataDescriptionChanged { Description = description });
        }

        private void Apply(UploadDataDescriptionChanged e)
        {
            Description = e.Description;
        }

        public void Delete()
        {
            RaiseEvent(new UploadDataDeleted());
        }

        private void Apply(UploadDataDeleted e)
        {
            IsDeleted = true;
        }

        public void DeleteDataInDayAssign()
        {
            RaiseEvent(new UploadDataDayAssignDeleted());
        }

        private void Apply(UploadDataDayAssignDeleted e)
        {
            IsDeleted = true;
        }

        internal static UploadData Upload(UploadedContentEnum typeEnum, string id, string originalName, string path, string mimeType, Guid jobAssignId, Guid upoloaderId)
        {
            return new UploadData(typeEnum, id, originalName, path, mimeType, jobAssignId, upoloaderId);
        }

        internal static UploadData DayAssignUpload(UploadedContentEnum typeEnum, string id, string originalName, string path, string mimeType, Guid dayAssignId, Guid upoloaderId)
        {
            return new UploadData(dayAssignId, upoloaderId, typeEnum, id, originalName, path, mimeType);
        }
    }
}
