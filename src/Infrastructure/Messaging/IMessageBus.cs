using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface IMessageBus
    {
        Task Publish<T>(T message) where T : IMessage;

        Task Publish<T>(IEnumerable<T> messages) where T : IMessage;
    }
}