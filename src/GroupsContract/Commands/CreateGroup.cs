using System;

namespace GroupsContract.Commands
{
    public class CreateGroup : GroupCommand
    {
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }

        public CreateGroup(string id, string name, Guid managementId, bool deleted) : base(id)
        {
            Name = name;
            ManagementId = managementId;
            Deleted = deleted;
        }
    }
}