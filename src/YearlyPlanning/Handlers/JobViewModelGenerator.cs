using Infrastructure.Messaging;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Events.JobEvents;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Handlers
{
    public class JobViewModelGenerator :
        IHandler<JobCreated>,
        IHandler<JobCategoryChanged>,
        IHandler<JobAddressChanged>,
        IHandler<JobTitleChanged>,
        IHandler<JobVisibilityChanged>
    {
        private readonly IMongoCollection<Job> collection;

        public JobViewModelGenerator(IYearlyPlanningConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<Job>(nameof(Job));
        }

        public Task Handle(JobCreated message)
        {
            return UpdateFacilityTask(message.SourceId,
                Builders<Job>.Update.Set(f => f.Id, message.SourceId),
                Builders<Job>.Update.Set(f=>f.ParentId, message.ParentId),
                Builders<Job>.Update.Set(f => f.CategoryId, message.CategoryId),
                Builders<Job>.Update.Set(f => f.Title, message.Title),
                Builders<Job>.Update.Set(f => f.JobTypeId, message.JobTypeId),
                Builders<Job>.Update.Set(f => f.AddressList, message.AddressList ?? new List<JobAddress>()),
                Builders<Job>.Update.Set(f => f.RelationGroupList, message.RelationGroupList ?? new List<RelationGroupModel>()),
                Builders<Job>.Update.Set(f => f.IsHidden, false),
                Builders<Job>.Update.Set(f => f.CreationDate, message.CreationDate),
                Builders<Job>.Update.Set(f => f.CreatorId, message.CreatorId),
                Builders<Job>.Update.Set(f => f.CreatedByRole, message.CreatedByRole));
        }

        public Task Handle(JobCategoryChanged message)
        {
            return UpdateFacilityTask(message.SourceId,
                Builders<Job>.Update.Set(f => f.CategoryId, message.CategoryId)
            );
        }

        public Task Handle(JobAddressChanged message)
        {
            return UpdateFacilityTask(message.SourceId,
                Builders<Job>.Update.Set(f => f.AddressList, message.AddressList ?? new List<JobAddress>())
            );
        }

        public Task Handle(JobTitleChanged message)
        {
            return UpdateFacilityTask(message.SourceId, Builders<Job>.Update.Set(f => f.Title, message.Title));
        }

        public Task Handle(JobVisibilityChanged message)
        {
            return UpdateFacilityTask(message.SourceId, Builders<Job>.Update.Set(f => f.IsHidden, message.IsHidden));
        }

        private async Task UpdateFacilityTask(string id, params UpdateDefinition<Job>[] updates)
        {
            await collection.FindOneAndUpdateAsync(
                Builders<Job>.Filter.Eq(f => f.Id, id),
                Builders<Job>.Update.Combine(updates),
                new FindOneAndUpdateOptions<Job> { IsUpsert = true }
            );
        }
    }
}