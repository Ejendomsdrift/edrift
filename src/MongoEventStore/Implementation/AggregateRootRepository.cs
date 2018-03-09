using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.EventSourcing.Implementation;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using MongoDB.Driver;
using MongoEventStore.Configurations;
using MongoEventStore.Models;
using MemberCore.Contract.Interfaces;

namespace MongoEventStore.Implementation
{
    public class AggregateRootRepository<T> : AggregateRootRepositoryBase<T>, IDisposable where T : IAggregateRoot, new()
    {
        private readonly ILog logger;
        private readonly IMessageBus messageBus;
        private readonly IMongoCollection<Counter> counterCollection;
        private readonly IMongoCollection<Commit> commitsCollection;
        private readonly IMemberService memberService;
        private bool disposed;

        public AggregateRootRepository(ILog logger,
            IMessageBus messageBus,
            IMongoEventStoreConfiguration configuration,
            IMemberService memberService)
        {
            this.logger = logger;
            this.messageBus = messageBus;
            this.memberService = memberService;
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);

            counterCollection = database.GetCollection<Counter>("Counters");

            commitsCollection = database.GetCollection<Commit>("Commits", new MongoCollectionSettings
            {
                AssignIdOnInsert = false,
                WriteConcern = WriteConcern.Acknowledged
            });

            CreateIndexes();
        }

        public override async Task<T> Get(string aggregateId)
        {
            var result = new T(); // should be done as first thing!!!
            var storedEvents = new SortedList<int, object>();

            using (var cursor = await commitsCollection.FindAsync(m => m.AggregateId == aggregateId))
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (var commit in cursor.Current)
                    {
                        foreach (var eventWrap in commit.Events)
                        {
                            storedEvents.Add(eventWrap.StreamRevision, eventWrap.Payload);
                        }
                    }
                }
            }

            if (storedEvents.Count == 0)
            {
                return default(T);//throw new AggregateNotFoundException();
            }

            foreach (var storedEvent in storedEvents)
            {
                result.ApplyEvent(storedEvent.Value as IEvent);
            }
            return result;
        }

        public override async Task Save(T aggregate, Guid? userId = null)
        {
            if (!aggregate.UncommitedEvents.Any()) return;

            var checkpoint = await GetNextFromCounter(nameof(Commit.CheckpointNumber));
            var sequence = await GetNextFromCounter(aggregate.Id);

            var commit = Commit.FromAggregate(aggregate, checkpoint, sequence);
            var currentUser = userId == null ? memberService.GetCurrentUser() : memberService.GetById(userId.Value);
            commit.MemberId = currentUser?.MemberId;

            await TryMongo(async () =>
            {
                await commitsCollection.InsertOneAsync(commit);

                await messageBus.Publish(aggregate.UncommitedEvents);

                aggregate.MarkEventsAsCommitted();

                logger.Debug(Messages.CommitPersisted, aggregate.Id);
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AggregateRootRepository()
        {
            Dispose(false);
        }

        internal async Task DeleteAll()
        {
            await counterCollection.DeleteManyAsync(f => true);
            await commitsCollection.DeleteManyAsync(f => true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || disposed)
            {
                return;
            }

            logger.Debug(Messages.ShuttingDownPersistence);
            disposed = true;
        }

        private async Task TryMongo(Func<Task> callback)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(Messages.StorageDisposed);
            }
            try
            {
                await callback();
            }
            catch (MongoConnectionException e)
            {
                logger.Warn(Messages.StorageUnavailable);
                throw new StorageUnavailableException(e.Message, e);
            }
            catch (MongoException e)
            {
                logger.Error(Messages.StorageThrewException, e.GetType());
                throw new StorageException(e.Message, e);
            }
        }

        private async Task<long> GetNextFromCounter(string name)
        {
            var counter = await counterCollection.FindOneAndUpdateAsync(
                Builders<Counter>.Filter.Eq(m => m.Id, name),
                Builders<Counter>.Update.Inc(f => f.Count, 1),
                new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After });
            return counter.Count;
        }

        private void CreateIndexes()
        {
            commitsCollection.Indexes.CreateOne(Builders<Commit>.IndexKeys
                    .Ascending(f => f.AggregateId)
                    .Ascending(f => f.StreamRevisionFrom),
                 new CreateIndexOptions { Name = "Concurrency_Constraint_Index", Unique = true });

            commitsCollection.Indexes.CreateOne(Builders<Commit>.IndexKeys
                    .Ascending(f => f.AggregateId)
                    .Ascending(f => f.CommitSequence),
                   new CreateIndexOptions { Name = "LogicalKey_Index", Unique = true });
        }
    }
}