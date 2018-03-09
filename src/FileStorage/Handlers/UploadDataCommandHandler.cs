using System;
using System.IO;
using System.Threading.Tasks;
using FileStorage.Contract.Commands;
using FileStorage.Contract.Enums;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;
using MimeTypes;
using Infrastructure.Helpers.Implementation;

namespace FileStorage.Handlers
{
    public class UploadDataCommandHandler :
        IHandler<ChangeDescription>,
        IHandler<Delete>,
        IHandler<DeleteDataInDayAssign>,
        IHandler<UploadAvatar>,
        IHandler<UploadForTaskInDepartment>,
        IHandler<DayAssignUploadFileCommand>
    {
        private readonly IAggregateRootRepository<UploadData> repository;

        public UploadDataCommandHandler(IAggregateRootRepository<UploadData> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(ChangeDescription message)
        {
            var uploadData = await repository.Get(message.Id);
            uploadData.ChangeDescription(message.Description);
            await repository.Save(uploadData);
        }

        public async Task Handle(DeleteDataInDayAssign message)
        {
            var uploadData = await repository.Get(message.Id);
            uploadData.DeleteDataInDayAssign();
            await repository.Save(uploadData);
        }

        public async Task Handle(Delete message)
        {
            var uploadData = await repository.Get(message.Id);
            uploadData.Delete();
            await repository.Save(uploadData);
        }

        public Task Handle(UploadAvatar message)
        {
            throw new NotImplementedException();
        }

        public async Task Handle(UploadForTaskInDepartment message)
        {
            try
            {
                var item = await repository.Get(message.Id);

                if (item != null)
                {
                    throw new Exception($"File with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }
            var extension = Path.GetExtension(message.OriginalName);
            var mimeType = MimeTypeMap.GetMimeType(extension);
            var contentType = MapContentType(mimeType);

            var relativePath = $"/{message.Id}{extension}";
            var absolutePath = $"{message.AbsolutePathPart}/{message.JobAssignId}/{relativePath}";

            FileHelper.EnsureFolder(absolutePath);

            File.WriteAllBytes(absolutePath, message.Buffer);

            var uploadData = UploadData.Upload(contentType, message.Id, message.OriginalName, relativePath, mimeType, message.JobAssignId, message.UploaderId);
            await repository.Save(uploadData);
        }

        private static UploadedContentEnum MapContentType(string mimeType)
        {
            var name = mimeType.Split('/')[0].ToLowerInvariant();
            switch (name)
            {
                case "application": return UploadedContentEnum.Document;
                case "image": return UploadedContentEnum.Image;
                case "video": return UploadedContentEnum.Video;
                default:
                    throw new ArgumentOutOfRangeException(mimeType);
            }
        }

        public async Task Handle(DayAssignUploadFileCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id.ToString());
                if (item != null)
                {
                    throw new Exception($"File with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }
            var extension = Path.GetExtension(message.OriginalName);
            var mimeType = MimeTypeMap.GetMimeType(extension);
            var contentType = MapContentType(mimeType);

            var relativePath = $"/{message.Id}{extension}";
            var absolutePath = $"{message.AbsolutePathPart}/{message.DayAssignId}/{relativePath}";

            FileHelper.EnsureFolder(absolutePath);

            File.WriteAllBytes(absolutePath, message.Buffer);

            var uploadData = UploadData.DayAssignUpload(contentType, message.Id, message.OriginalName, relativePath, mimeType, message.DayAssignId, message.UploaderId);
            await repository.Save(uploadData);
        }
    }
}
