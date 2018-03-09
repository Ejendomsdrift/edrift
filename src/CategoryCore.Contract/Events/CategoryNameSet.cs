using Infrastructure.EventSourcing.Implementation;

namespace CategoryCore.Contract.Events
{
    public class CategoryNameSet : EventBase
    {
        public string Name { get; set; }
    }
}