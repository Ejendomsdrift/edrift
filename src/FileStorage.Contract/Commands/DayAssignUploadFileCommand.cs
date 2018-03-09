using System;

namespace FileStorage.Contract.Commands
{
    public class DayAssignUploadFileCommand: UploadCommand
    {
        public Guid DayAssignId { get; set; }

        public Guid UploaderId { get; set; }

        public DayAssignUploadFileCommand(Guid id, byte[] buffer, string originalName, string absolutePathPart, Guid dayAssignId, Guid uploaderId) : base(id, buffer, originalName, absolutePathPart)
        {
            DayAssignId = dayAssignId;
            UploaderId = uploaderId;
        }
    }
}
