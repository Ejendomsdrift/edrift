using Infrastructure.EventSourcing.Implementation;

namespace GroupsContract.Events
{
    public class GroupNameSet : EventBase
    {
        public string Name { get; set; }
    }
}