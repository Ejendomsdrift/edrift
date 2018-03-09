using System;

namespace FileStorage.Contract.Commands
{
    public class ChangeDescription : FileCommand
    {
        public string Description { get; set; }

        public ChangeDescription(Guid id, string description) : base(id.ToString())
        {
            Description = description;
        }
    }
}