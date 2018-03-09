using System;

namespace FileStorage.Contract.Commands
{
    public class UploadAvatar : UploadCommand
    {
        public string MemberId { get; set; }

        public UploadAvatar(Guid id, byte[] buffer, string originalName, string memberId, string absolutePathPart)
            : base(id, buffer, originalName, absolutePathPart)
        {
            MemberId = memberId;
        }
    }
}