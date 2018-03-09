using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.Contract.Events;
using Infrastructure.Messaging;
using MongoDB.Driver;
using YearlyPlanning.Contract.Events.JobAssignEvents;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Handlers
{
    public class JobAssignViewModelGenerator :
        IHandler<JobAssignCreatedEvent>,
        IHandler<JobAssignCreatedFromGlobalEvent>,
        IHandler<JobAssignEvent>,
        IHandler<JobUnassignEvent>,
        IHandler<JobAssignChangeIsEnabledEvent>,
        IHandler<JobAssignDescriptionChangedEvent>,
        IHandler<JobAssignTillYearChangedEvent>,
        IHandler<JobAssignWeeksChangedEvent>,
        IHandler<JobAssignAllWeeksChangedEvent>,
        IHandler<JobAssignSaveDaysPerWeekEvent>,
        IHandler<JobAssignLockIntervalEvent>,
        IHandler<UploadDataUploaded>,
        IHandler<UploadDataDeleted>,
        IHandler<UploadDataDescriptionChanged>,
        IHandler<UploadDataDayAssignDeleted>,
        IHandler<AdHockJobAssignCreatedEvent>,
        IHandler<TenantJobAssignCreatedEvent>,
        IHandler<JobAssignSheduleChangedEvent>,
        IHandler<JobAssignJobIdListChangedEvent>,
        IHandler<CopyCommonJobAssignInfoEvent>
    {
        private readonly IMongoCollection<JobAssign> collection;
        private readonly IMongoCollection<DayAssign> dayAssignCollection;

        public JobAssignViewModelGenerator(IYearlyPlanningConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<JobAssign>(nameof(JobAssign));
            dayAssignCollection = database.GetCollection<DayAssign>(nameof(DayAssign));
        }

        public Task Handle(JobAssignCreatedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.IsEnabled, message.IsEnabled),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear),
                Builders<JobAssign>.Update.Set(f => f.IsLocked, message.IsLocked),
                Builders<JobAssign>.Update.Set(f => f.CreatedByRole, message.CreatedByRole),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.WeekList),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.JobIdList, message.JobIdList),
                Builders<JobAssign>.Update.Set(f => f.IsGlobal, message.IsGlobal),
                Builders<JobAssign>.Update.Set(f => f.HousingDepartmentIdList, message.HousingDepartmentIdList),
                Builders<JobAssign>.Update.Set(f => f.UploadList, message.UploadList),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }

        public Task Handle(JobAssignCreatedFromGlobalEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.HousingDepartmentIdList, message.HousingDepartmentIdList),
                Builders<JobAssign>.Update.Set(f => f.IsEnabled, message.IsEnabled),
                Builders<JobAssign>.Update.Set(f => f.Description, message.Description),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.IsLocked, message.IsLocked),
                Builders<JobAssign>.Update.Set(f => f.CreatedByRole, message.CreatedByRole),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.WeekList),
                Builders<JobAssign>.Update.Set(f => f.UploadList, message.UploadList),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.JobIdList, message.JobIdList),
                Builders<JobAssign>.Update.Set(f => f.IsGlobal, message.IsGlobal));
        }

        public Task Handle(AdHockJobAssignCreatedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Push(f => f.HousingDepartmentIdList, message.DepartmentId),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.Description, message.Description),
                Builders<JobAssign>.Update.Set(f => f.JobIdList, message.JobIdList),
                Builders<JobAssign>.Update.Set(f => f.CreatedByRole, message.CreatedByRole),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.WeekList),
                Builders<JobAssign>.Update.Set(f => f.IsEnabled, message.IsEnabled)
                );
        }

        public Task Handle(TenantJobAssignCreatedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Push(f => f.HousingDepartmentIdList, message.DepartmentId),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.Description, message.Description),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.JobIdList, message.JobIdList),
                Builders<JobAssign>.Update.Set(f => f.CreatedByRole, message.CreatedByRole),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.WeekList),
                Builders<JobAssign>.Update.Set(f => f.IsEnabled, message.IsEnabled),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear)
                );
        }

        public Task Handle(JobAssignEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId), Builders<JobAssign>.Update.Push(f => f.HousingDepartmentIdList, message.DepartmentId));
        }

        public Task Handle(JobUnassignEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId), Builders<JobAssign>.Update.Pull(f => f.HousingDepartmentIdList, message.DepartmentId));
        }

        public Task Handle(JobAssignChangeIsEnabledEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId), Builders<JobAssign>.Update.Set(f => f.IsEnabled, message.IsEnabled));
        }

        public Task Handle(JobAssignDescriptionChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                    Builders<JobAssign>.Update.Set(f => f.Description, message.Description));
        }

        public Task Handle(JobAssignJobIdListChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                    Builders<JobAssign>.Update.Set(f => f.JobIdList, message.JobIdList));
        }

        public Task Handle(JobAssignTillYearChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear),
                Builders<JobAssign>.Update.Set(f => f.IsLocalIntervalChanged, message.IsLocalIntervalChanged),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }

        public Task Handle(JobAssignWeeksChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.Weeks),
                Builders<JobAssign>.Update.Set(f => f.IsLocalIntervalChanged, message.IsLocalIntervalChanged),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }

        public Task Handle(JobAssignAllWeeksChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.Weeks),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }

        public Task Handle(JobAssignSaveDaysPerWeekEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }

        public Task Handle(JobAssignLockIntervalEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.IsLocked, message.IsLocked));
        }

        public Task Handle(JobAssignSheduleChangedEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.IsLocalIntervalChanged, message.IsLocalIntervalChanged),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedBy));
        }

        public Task Handle(UploadDataUploaded message)
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

            JobAssign jobAssign = GetJobAssignById(message.JobAssignId);

            if (jobAssign.UploadList == null)
            {
                jobAssign.UploadList = new List<UploadFileModel> { model };
            }
            else
            {
                jobAssign.UploadList.Add(model);
            }

            return UpdateJobAssign(message.JobAssignId,
                Builders<JobAssign>.Update.Set(f => f.UploadList, jobAssign.UploadList));
        }

        private JobAssign GetJobAssignByFileId(Guid fileId)
        {
            using (var cursor = collection.FindSync(f => f.UploadList.Any(u => u.FileId == fileId), new FindOptions<JobAssign> { Limit = 1 }))
            {
                return cursor.FirstOrDefault();
            }
        }

        private DayAssign GetDayAssignByFileId(Guid fileId)
        {
            using (var cursor = dayAssignCollection.FindSync(x => x.UploadList.Any(u => u.FileId == fileId), new FindOptions<DayAssign> { Limit = 1 }))
            {
                return cursor.FirstOrDefault();
            }
        }

        private JobAssign GetJobAssignById(Guid jobAssignId)
        {
            using (var cursor = collection.FindSync(f => f.Id == jobAssignId, new FindOptions<JobAssign> { Limit = 1 }))
            {
                return cursor.FirstOrDefault();
            }
        }

        public Task Handle(UploadDataDescriptionChanged message)
        {
            JobAssign jobAssign = GetJobAssignByFileId(Guid.Parse(message.SourceId));// source id here is a file id

            jobAssign.UploadList.FirstOrDefault(u => u.FileId == Guid.Parse(message.SourceId)).Description = message.Description;

            return UpdateJobAssign(jobAssign.Id,
                Builders<JobAssign>.Update.Set(f => f.UploadList, jobAssign.UploadList));
        }

        public Task Handle(UploadDataDeleted message)
        {
            JobAssign jobAssign = GetJobAssignByFileId(Guid.Parse(message.SourceId));// source id here is a file id

            return UpdateJobAssign(jobAssign.Id,
                Builders<JobAssign>.Update.Pull(f => f.UploadList, jobAssign.UploadList.FirstOrDefault(x => x.FileId == Guid.Parse(message.SourceId))));
        }

        public Task Handle(UploadDataDayAssignDeleted message)
        {
            DayAssign dayAssign = GetDayAssignByFileId(Guid.Parse(message.SourceId));
            return UpdateDayAssign(dayAssign.Id,
                Builders<DayAssign>.Update.Pull(f => f.UploadList, dayAssign.UploadList.FirstOrDefault(x => x.FileId == Guid.Parse(message.SourceId))));
        }

        private async Task UpdateJobAssign(Guid id, params UpdateDefinition<JobAssign>[] updates)
        {
            await collection.FindOneAndUpdateAsync(
                Builders<JobAssign>.Filter.Eq(f => f.Id, id),
                Builders<JobAssign>.Update.Combine(updates),
                new FindOneAndUpdateOptions<JobAssign> { IsUpsert = true }
            );
        }

        private async Task UpdateDayAssign(Guid id, params UpdateDefinition<DayAssign>[] updates)
        {
            await dayAssignCollection.FindOneAndUpdateAsync(
                Builders<DayAssign>.Filter.Eq(f => f.Id, id),
                Builders<DayAssign>.Update.Combine(updates),
                new FindOneAndUpdateOptions<DayAssign> { IsUpsert = true }
            );
        }

        public Task Handle(CopyCommonJobAssignInfoEvent message)
        {
            return UpdateJobAssign(Guid.Parse(message.SourceId),
                Builders<JobAssign>.Update.Set(f => f.TillYear, message.TillYear),
                Builders<JobAssign>.Update.Set(f => f.WeekList, message.WeekList),
                Builders<JobAssign>.Update.Set(f => f.DayPerWeekList, message.DayPerWeekList),
                Builders<JobAssign>.Update.Set(f => f.RepeatsPerWeek, message.RepeatsPerWeek),
                Builders<JobAssign>.Update.Set(f => f.ChangedByRole, message.ChangedByRole));
        }
    }
}
