using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Implementation
{
    public class SynchronousMessageBus : IMessageBus
    {
        private readonly IHandlersProvider handlersProvider;

        public SynchronousMessageBus(IHandlersProvider handlersProvider)
        {
            this.handlersProvider = handlersProvider;
        }

        public async Task Publish<T>(T message) where T : IMessage
        {
            var handlers = handlersProvider.GetFor(message.GetType());
            if (message is ICommand && handlers.Length > 1)
            {
                throw new Exception($"More than one command handler found, for command {nameof(T)}");
            }

            foreach (var handler in handlers)
            {
                await ((dynamic)handler).Handle((dynamic)message);
            }
        }

        public async Task Publish<T>(IEnumerable<T> messages) where T : IMessage
        {
            foreach (var message in messages)
            {
                await Publish(message);
            }
        }
    }
}