using Infrastructure.EventSourcing.Implementation;

namespace MongoEventStore.Tests.Stubs
{
    internal class Events
    {
        public class Created : EventBase
        {
            public string Id { get; set; }
        }

        internal class XSetted : EventBase
        {
            public int Value { get; set; }
        }

        internal class YSetted : EventBase
        {
            public int Value { get; set; }
        }

        internal class Summed : EventBase
        {
        }
    }
}