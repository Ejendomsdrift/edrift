namespace Infrastructure.Messaging
{
    public interface IEvent : IMessage
    {
        string SourceId { get; set; }
    }
}