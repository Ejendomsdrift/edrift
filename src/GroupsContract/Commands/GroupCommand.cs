using Infrastructure.Messaging;

namespace GroupsContract.Commands
{
    public abstract class GroupCommand : ICommand
    {
        public string Id { get; set; }

        protected GroupCommand(string id)
        {
            Id = id;
        }
    }
}