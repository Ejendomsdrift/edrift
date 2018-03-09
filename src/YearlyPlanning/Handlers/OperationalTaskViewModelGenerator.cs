using System.Threading.Tasks;
using Infrastructure.Messaging;
using MongoDB.Driver;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Events.OperationalTaskEvents;
using YearlyPlanning.Models;

namespace YearlyPlanning.Handlers
{
    public class OperationalTaskViewModelGenerator :
        IHandler<OperationalTaskCreatedEvent>,
        IHandler<AdHocTaskSaveDaysPerWeekEvent>,
        IHandler<OperationalTaskChangeEstimateEvent>,
        IHandler<OperationalTaskChangeDescriptionEvent>,
        IHandler<OperationalTaskChangeAddress>,
        IHandler<OperationalTaskSaveAssignEmployeesEvent>,
        IHandler<AdHocTaskChangeCategoryEvent>,
        IHandler<OperationalTaskChangeTitleEvent>

    {
        private readonly IMongoCollection<OperationalTaskModel> collection;

        public OperationalTaskViewModelGenerator(IYearlyPlanningConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<OperationalTaskModel>(nameof(OperationalTaskModel));
        }

        public Task Handle(OperationalTaskCreatedEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Year, message.Year),
                Builders<OperationalTaskModel>.Update.Set(f => f.Week, message.Week),
                Builders<OperationalTaskModel>.Update.Set(f => f.DepartmentId, message.DepartmentId),
                Builders<OperationalTaskModel>.Update.Set(f => f.CategoryId, message.CategoryId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Title, message.Title)
            );
        }

        public Task Handle(AdHocTaskSaveDaysPerWeekEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.DaysPerWeek, message.DaysPerWeek)
                );
        }

        public Task Handle(OperationalTaskChangeEstimateEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Estimate, message.Estimate)
                );
        }

        public Task Handle(OperationalTaskChangeDescriptionEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Description, message.Description)
                );
        }

        public Task Handle(OperationalTaskChangeAddress message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Address, message.Address)
                );
        }

        public Task Handle(OperationalTaskSaveAssignEmployeesEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.UserIdList, message.AssignedEmployees)
                                                         .Set(f => f.GroupId, message.GroupId)
                );
        }

        public Task Handle(AdHocTaskChangeCategoryEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.CategoryId, message.CategoryId)
                );
        }

        public Task Handle(OperationalTaskChangeTitleEvent message)
        {
            return Update(message.SourceId,
                Builders<OperationalTaskModel>.Update.Set(f => f.Id, message.SourceId),
                Builders<OperationalTaskModel>.Update.Set(f => f.Title, message.Title)
                );
        }

        private async Task Update(string id, params UpdateDefinition<OperationalTaskModel>[] updates)
        {
            await collection.FindOneAndUpdateAsync(
                Builders<OperationalTaskModel>.Filter.Eq(f => f.Id, id),
                Builders<OperationalTaskModel>.Update.Combine(updates),
                new FindOneAndUpdateOptions<OperationalTaskModel> { IsUpsert = true }
            );
        }
    }
}
