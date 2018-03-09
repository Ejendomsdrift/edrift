using System;

namespace Infrastructure.Messaging
{
    public interface IHandlersProvider
    {
        object[] GetFor(Type messageType);
    }
}