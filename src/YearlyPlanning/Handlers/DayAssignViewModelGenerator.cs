using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Contract.Events;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using MongoDB.Driver;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Events.DayAssignEvents;
using YearlyPlanning.ReadModel;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Events.OperationalTaskEvents;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Handlers
{
    public class DayAssignViewModelGenerator :
        IHandler<DayAssignCreated>,
        IHandler<DayAssignDateChanged>,
        IHandler<DayAssignEstimatedMinutesChanged>,
        IHandler<DayAssignMembersGroupAssigned>,
        IHandler<RemoveDayAssignMembersEvent>,
        IHandler<DayAssignChangeJobAssignId>,
        IHandler<DayAssignChangeStatus>,
        IHandler<TenantTaskChangeTypeEvent>,
        IHandler<TenantTaskChangeUrgencyEvent>,
        IHandler<OperationalTaskChangeTimeEvent>,
        IHandler<TenantTaskChangeResidentNameEvent>,
        IHandler<TenantTaskChangeResidentPhoneEvent>,
        IHandler<TenantTaskChangeCommentEvent>,
        IHandler<DayAssignUploadDataUploaded>,
        IHandler<DayAssignWeekNumberChangedEvent>
    {
        private readonly IMongoCollection<DayAssign> collection;
        private readonly IRepository<DayAssign> repository;

        public DayAssignViewModelGenerator(IYearlyPlanningConfiguration configuration, IRepository<DayAssign> repository)
        {
            this.repository = repository;

            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<DayAssign>(nameof(DayAssign));
        }


        public async Task Handle(DayAssignCreated message)
        {
            repository.Save(message.Map<DayAssign>());
        }

        public async Task Handle(DayAssignDateChanged message)
        {
            await Update(message.SourceId, 
                Builders<DayAssign>.Update.Set(f => f.Date, message.Date),
                Builders<DayAssign>.Update.Set(f => f.WeekDay, message.WeekDay),
                Builders<DayAssign>.Update.Set(f => f.Year, message.Year));
        }

        public async Task Handle(DayAssignEstimatedMinutesChanged message)
        {
            await Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.EstimatedMinutes, message.EstimatedMinutes));
        }

        public async Task Handle(DayAssignMembersGroupAssigned message)
        {
            await Update(message.SourceId,
                Builders<DayAssign>.Update.Set(f => f.GroupId, message.GroupId),
                Builders<DayAssign>.Update.Set(f => f.UserIdList, message.UserIdList),
                Builders<DayAssign>.Update.Set(f => f.TeamLeadId, message.TeamLeadId),
                Builders<DayAssign>.Update.Set(f => f.IsAssignedToAllUsers, message.IsAssignedToAllUsers)
                );
        }

        private async Task Update(string id, params UpdateDefinition<DayAssign>[] updates)
        {
            await collection.FindOneAndUpdateAsync(
                Builders<DayAssign>.Filter.Eq(f => f.Id, Guid.Parse(id)),
                Builders<DayAssign>.Update.Combine(updates),
                new FindOneAndUpdateOptions<DayAssign> { IsUpsert = true }
            );
        }

        public async Task Handle(RemoveDayAssignMembersEvent message)
        {           
            await Update(message.SourceId,
                Builders<DayAssign>.Update.PullAll(f => f.UserIdList, message.UserIdList));
        }

        public async Task Handle(DayAssignChangeJobAssignId message)
        {
            await Update(message.SourceId,
                Builders<DayAssign>.Update.Set(f => f.JobAssignId, message.JobAssignId));
        }

        public async Task Handle(DayAssignChangeStatus message)
        {
            await Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.StatusId, message.Status));
        }
       
        public Task Handle(TenantTaskChangeTypeEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.TenantType, message.Type));
        }

        public Task Handle(TenantTaskChangeUrgencyEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.IsUrgent, message.IsUrgent));
        }
           
        public Task Handle(OperationalTaskChangeTimeEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.Date, message.Time));
        }

        public Task Handle(TenantTaskChangeResidentNameEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.ResidentName, message.ResidentName));
        }

        public Task Handle(TenantTaskChangeResidentPhoneEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.ResidentPhone, message.ResidentPhone));
        }

        public Task Handle(TenantTaskChangeCommentEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.Comment, message.Comment));
        }

        public async Task Handle(DayAssignUploadDataUploaded message)
        {
            var model = new UploadFileModel
            {
                FileId = Guid.Parse(message.SourceId),
                FileName = message.Name,
                Path = message.Path,
                ContentType = message.ContentType,
                CreationDate = message.UploadedOn,
                UploaderId = message.UploaderId
            };

            DayAssign dayAssign = GetDayAssignById(message.DayAssignId);

            if (dayAssign.UploadList == null)
            {
                dayAssign.UploadList = new List<UploadFileModel> { model };
            }
            else
            {
                dayAssign.UploadList.Add(model);
            }

            await Update(message.DayAssignId.ToString(),
                Builders<DayAssign>.Update.Set(f => f.UploadList, dayAssign.UploadList));
        }

        public Task Handle(DayAssignWeekNumberChangedEvent message)
        {
            return Update(message.SourceId, Builders<DayAssign>.Update.Set(f => f.WeekNumber, message.WeekNumber));
        }

        private DayAssign GetDayAssignById(Guid dayAssignId)
        {
            using (var cursor = collection.FindSync(f => f.Id == dayAssignId, new FindOptions<DayAssign> { Limit = 1 }))
            {
                return cursor.FirstOrDefault();
            }
        }
    }
}