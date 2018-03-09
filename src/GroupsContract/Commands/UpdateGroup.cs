namespace GroupsContract.Commands
{
    public class UpdateGroup : GroupCommand
    {
        public string Name { get; set; }

        public UpdateGroup(string id, string name) : base(id)
        {
            Name = name;
        }
    }
}