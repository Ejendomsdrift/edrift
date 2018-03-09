using System;
using System.Linq;
using Infrastructure.Messaging;
using Ninject;

namespace Web.Core
{
    public class HandlersProvider : IHandlersProvider
    {
        private static readonly Type HandlerType = typeof(IHandler<>);
        private readonly IKernel kernel;

        public HandlersProvider(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public object[] GetFor(Type messageType)
        {
            var handlerType = HandlerType.MakeGenericType(messageType);
            return kernel.GetAll(handlerType).ToArray();
        }
    }
}