namespace Infrastructure.Messaging.Implementation
{
    public class StringMessageBus : IMessage
    {
        public string Message { get; set; }
    }
}
