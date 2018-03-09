using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.EventSourcing;
using Infrastructure.Logging;
using Infrastructure.Logging.Implementation;
using Infrastructure.Messaging;
using MongoEventStore.Implementation;
using MongoEventStore.Tests.Stubs;
using Moq;
using NUnit.Framework;
using MemberCore.Contract.Interfaces;

namespace MongoEventStore.Tests
{
    [TestFixture]
    internal class RepositoryTests
    {
        private static AggregateRootRepository<T> BuildRepository<T>(IMessageBus messageBus = null, ILog logger = null)
            where T : IAggregateRoot, new()
        {
            return new AggregateRootRepository<T>(
                messageBus: messageBus ?? Mock.Of<IMessageBus>(),
                logger: logger ?? new NullLogger(),
                configuration: new LocalTestConfiguration(),
                memberService: Mock.Of<IMemberService>()
            );
        }

        [SetUp]
        public async Task Setup()
        {
            await BuildRepository<Calculator>().DeleteAll();
        }

        [Test]
        public async Task when_get_is_called_for_not_existing_id()
        {
            var repository = BuildRepository<Calculator>();

            var act = await repository.Get("not existing id");

            Assert.IsNull(act);
        }

        [Test]
        public async Task when_get_is_called_for_existing_id()
        {
            var messageBus = Mock.Of<IMessageBus>();
            var repository = BuildRepository<Calculator>(messageBus: messageBus);
            var result = 0;

            const string aggregateId = "test sum";
            var calculator = Calculator.Create(aggregateId);

            for (var i = 0; i < 100; i++)
            {
                calculator.SetX(calculator.Result);
                calculator.SetY(i);
                calculator.Sum();
                await repository.Save(calculator);
                result += i;
            }

            var resaved = await repository.Get(aggregateId);
            resaved.SetX(resaved.Result);
            resaved.SetY(-1000);
            resaved.Sum();
            await repository.Save(resaved);

            var actual = await repository.Get(aggregateId);

            actual.Result.Should().Be(result - 1000);
            Mock.Get(messageBus).Verify(b => b.Publish(It.IsAny<IEnumerable<IMessage>>()), Times.Exactly(101));
        }

        [Test]
        public async Task when_save_is_called()
        {
            var repository = BuildRepository<Calculator>();

            var calculator = Calculator.Create("test sum");
            calculator.SetX(1);
            calculator.SetY(2);
            calculator.Sum();

            await repository.Save(calculator);
        }

        [Test]
        public void when_save_is_called_twice_and_no_events()
        {
            var repository = BuildRepository<Calculator>();

            var calculator = new Calculator();

            Func<Task> act1 = async () => await repository.Save(calculator);
            Func<Task> act2 = async () => await repository.Save(calculator);

            act1.ShouldNotThrow();
            act2.ShouldNotThrow();
        }
    }
}