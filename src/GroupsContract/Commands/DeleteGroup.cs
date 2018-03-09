namespace GroupsContract.Commands
{
    public class DeleteGroup : GroupCommand
    {
        public bool Deleted { get; set; }

        public DeleteGroup(string id, bool deleted) : base(id)
        {
            Deleted = deleted;
        }
    }
}