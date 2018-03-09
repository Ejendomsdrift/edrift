using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface IHandler<in T> where T : IMessage
    {
        Task Handle(T message);
    }
}