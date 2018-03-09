using Infrastructure.EventSourcing.Implementation;

namespace FileStorage.Contract.Events
{
    public class UploadDataDescriptionChanged : EventBase
    {
        public string Description { get; set; }
    }
}
