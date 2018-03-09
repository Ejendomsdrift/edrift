using System;

namespace FileStorage.Contract.Commands
{
    public abstract class UploadCommand : FileCommand
    {
        public byte[] Buffer { get; set; }
        public string OriginalName { get; set; }
        public string AbsolutePathPart { get; set; }

        protected UploadCommand(Guid id, byte[] buffer, string originalName, string absolutePathPart) : base(id.ToString())
        {
            Buffer = buffer;
            OriginalName = originalName;
            AbsolutePathPart = absolutePathPart;
        }
    }
}