using Infrastructure.EventSourcing.Implementation;

namespace MongoEventStore.Tests.Stubs
{
    internal class Calculator : AggregateBase
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Result { get; private set; }

        public Calculator()
        {
            RegisterTransition<Events.Created>(Apply);
            RegisterTransition<Events.XSetted>(Apply);
            RegisterTransition<Events.YSetted>(Apply);
            RegisterTransition<Events.Summed>(Apply);
        }

        private Calculator(string id) : this()
        {
            RaiseEvent(new Events.Created { Id = id });
        }

        private void Apply(Events.Created e)
        {
            Id = e.Id;
        }

        public void SetX(int value)
        {
            RaiseEvent(new Events.XSetted { Value = value });
        }

        private void Apply(Events.XSetted e)
        {
            X = e.Value;
        }

        public void SetY(int value)
        {
            RaiseEvent(new Events.YSetted { Value = value });
        }

        private void Apply(Events.YSetted e)
        {
            Y = e.Value;
        }

        public void Sum()
        {
            RaiseEvent(new Events.Summed());
        }

        private void Apply(Events.Summed e)
        {
            Result = X + Y;
        }

        public static Calculator Create(string id)
        {
            return new Calculator(id);
        }
    }
}