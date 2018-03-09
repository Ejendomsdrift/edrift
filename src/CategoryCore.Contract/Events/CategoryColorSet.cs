using Infrastructure.EventSourcing.Implementation;

namespace CategoryCore.Contract.Events
{
    public class CategoryColorSet : EventBase
    {
        public string Color { get; set; }
    }
}