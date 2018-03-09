using System;

namespace FileStorage.Contract.Commands
{
    public class UploadForTaskInDepartment : UploadCommand
    {
        public Guid JobAssignId { get; set; }

        public Guid UploaderId { get; set; }

        public UploadForTaskInDepartment(Guid id, byte[] buffer, string originalName, string absolutePathPart, Guid jobAssignId, Guid uploaderId)
            : base(id, buffer, originalName, absolutePathPart)
        {
            JobAssignId = jobAssignId;
            UploaderId = uploaderId;
        }
    }
}