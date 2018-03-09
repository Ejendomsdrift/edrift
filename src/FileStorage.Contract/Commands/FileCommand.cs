using Infrastructure.Messaging;

namespace FileStorage.Contract.Commands
{
    public abstract class FileCommand : ICommand
    {
        public string Id { get; set; }

        protected FileCommand(string id)
        {
            Id = id;
        }
    }
}
