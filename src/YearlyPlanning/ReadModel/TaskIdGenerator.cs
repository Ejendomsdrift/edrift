using System.Threading.Tasks;
using Infrastructure.Constants;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.ReadModel
{
    public class TaskIdGenerator : ITaskIdGenerator
    {
        private readonly IMongoCollection<TaskId> counterCollection;

        public TaskIdGenerator(IYearlyPlanningConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            counterCollection = database.GetCollection<TaskId>(nameof(TaskId));
        }

        public Task<string> Facility()
        {
            return GetNextInSequense(Constants.TaskId.Facility);
        }

        public Task<string> AdHoc()
        {
            return GetNextInSequense(Constants.TaskId.AdHoc);
        }

        public Task<string> Tenant()
        {
            return GetNextInSequense(Constants.TaskId.Tenant);
        }

        public Task<string> Other()
        {
            return GetNextInSequense(Constants.TaskId.Other);
        }

        private async Task<string> GetNextInSequense(string name)
        {
            var counter = await counterCollection.FindOneAndUpdateAsync(
                Builders<TaskId>.Filter.Eq(m => m.Id, name),
                Builders<TaskId>.Update.Inc(f => f.Count, 1),
                new FindOneAndUpdateOptions<TaskId> { IsUpsert = true, ReturnDocument = ReturnDocument.After });
            return $"{name}-{counter.Count}";
        }

        [BsonIgnoreExtraElements]
        internal class TaskId
        {
            [BsonId]
            public string Id { get; set; }

            public long Count { get; set; }
        }
    }
}