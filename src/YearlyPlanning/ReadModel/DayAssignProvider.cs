using GroupsContract.Interfaces;
using Infrastructure.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;

namespace YearlyPlanning.ReadModel
{
    public class DayAssignProvider : IDayAssignProvider
    {
        private readonly IMongoCollection<DayAssign> collection;
        private readonly IGroupService groupService;

        public IQueryable<DayAssign> Query => collection.AsQueryable();

        static DayAssignProvider()
        {
            BsonClassMap.RegisterClassMap<DayAssign>(m =>
            {
                m.AutoMap();
                m.SetIgnoreExtraElements(true);
            });
        }

        public DayAssignProvider(
            IYearlyPlanningConfiguration configuration,
            IGroupService groupService)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<DayAssign>(nameof(DayAssign));

            this.groupService = groupService;
        }

        public DayAssign Get(Guid id)
        {
            var result = Query.FirstOrDefault(i => i.Id == id);
            return result;
        }

        public IEnumerable<IDayAssign> GetDayAssignsForMembersByFilter(MemberDayAssignFilterModel filter)
        {
            Expression<Func<DayAssign, bool>> query = GetQuery(filter);
            IQueryable<DayAssign> data = Query.Where(query);

            return data;
        }

        public IEnumerable<DayAssign> GetList(IEnumerable<Guid> ids)
        {
            return Query.Where(d => ids.Contains(d.Id)).ToList();
        }

        public List<IDayAssign> GetDayAssignsForMember(Guid memberId, bool isNotAllowedEmptyDepartment, IEnumerable<Guid> departments = null, int? daysAhead = null)
        {
            var filter = new TaskDataFilterModel
            {
                MemberIds = new List<Guid> { memberId },
                HousingDepartments = departments,
                EndDate = daysAhead.HasValue ? DateTime.Now.Date.AddDays(daysAhead.Value - 1).SetToLastTickOfDay() : default(DateTime?)
            };

            var query = GetQuery(filter, isNotAllowedEmptyDepartment);
            var data = Query.Where(query).ToList<IDayAssign>();
            return data;
        }

        public List<IDayAssign> GetDayAssignsForMemberByStatuses(Guid memberId, IEnumerable<JobStatus> statuses, int? daysAhead = null,
            IEnumerable<Guid> departments = null)
        {
            var filter = new TaskDataFilterModel
            {
                MemberIds = new List<Guid> { memberId },
                HousingDepartments = departments,
                EndDate = daysAhead.HasValue ? DateTime.Now.Date.AddDays(daysAhead.Value - 1).SetToLastTickOfDay() : default(DateTime?),
                JobStatuses = statuses
            };

            var query = GetQuery(filter, isNotAllowedEmptyDepartment: true);
            var data = Query.Where(query).ToList<IDayAssign>();
            return data;
        }

        public List<Guid> GetDayAssignIds(ITaskDataFilterModel filter)
        {
            var query = GetQuery(filter);
            var ids = Query.Where(query).Select(i => i.Id).ToList();
            return ids;
        }

        public List<IDayAssign> GetDayAssignsForGroups(IEnumerable<Guid> groupIds)
        {
            var groupIdsList = groupIds.ToList();
            return Query.Where(f => f.GroupId != null && groupIdsList.Contains(f.GroupId.Value)).ToList<IDayAssign>();
        }

        public List<IDayAssign> GetDayAssignsForGroupAndTeamLead(Guid groupId, Guid memberId)
        {
            return Query.Where(f => f.GroupId != null && f.GroupId.Value == groupId && f.TeamLeadId != null && f.TeamLeadId == memberId).ToList<IDayAssign>();
        }

        public List<DayAssign> GetDayAssigns(Guid jobAssignId, string jobId, Guid departmentId, int weekNumber)
        {
            using (var cursor = collection.FindSync(f => f.DepartmentId == departmentId && f.JobId == jobId && f.JobAssignId == jobAssignId && f.WeekNumber == weekNumber))
            {
                return cursor.ToList();
            }
        }

        public List<DayAssign> GetDayAssigns(List<Guid> jobAssignList, string jobId, Guid department, List<JobStatus> notAllowedStatusList)
        {
            Expression<Func<DayAssign, bool>> filter = f => f.JobId == jobId;
            filter = filter.And(f => f.DepartmentId == department);
            filter = filter.And(f => jobAssignList.Contains(f.JobAssignId));

            using (var cursor = collection.FindSync(filter))
            {
                return cursor.ToList();
            }
        }

        public List<DayAssign> GetForWeek(Guid departmentId, int year, int weekNumber)
        {
            var data = Query.Where(i => i.DepartmentId == departmentId && i.Year == year && i.WeekNumber == weekNumber).ToList();
            return data;
        }

        public List<DayAssign> GetForWeekForAllDepartments(int year, int weekNumber)
        {
            var data = Query.Where(i => i.Year == year && i.WeekNumber == weekNumber).ToList();
            return data;
        }

        public Task<DayAssign> GetByJobId(string jobId)
        {
            using (var cursor = collection.FindSync(f => f.JobId == jobId))
            {
                return cursor.FirstOrDefaultAsync();
            }
        }

        public List<DayAssign> GetByJobIds(IEnumerable<string> jobIds)
        {
            var result = Query.Where(d => jobIds.Contains(d.JobId)).ToList();
            return result;
        }

        public Task<DayAssign> GetByJobIdAndDepartmentId(string jobId, Guid departmentId)
        {
            using (var cursor = collection.FindSync(f => f.JobId == jobId && f.DepartmentId == departmentId))
            {
                return cursor.FirstOrDefaultAsync();
            }
        }

        public IEnumerable<IDayAssign> GetDayAssignsByJobId(string jobId)
        {
            return Query.Where(j => j.JobId == jobId);
        }

        public List<DayAssign> Find(Expression<Func<DayAssign, bool>> filter)
        {
            return collection.FindSync(filter).ToList();
        }

        public Expression<Func<DayAssign, bool>> GetQuery(ITaskDataFilterModel filter)
        {
            Expression<Func<DayAssign, bool>> query = GetQueryByWeek(filter);

            if (filter.MemberIds.HasValue())
            {
                query = query.And(GetQuery(filter.MemberIds));
            }

            if (filter.JobState.HasValue && filter.Year.HasValue && filter.Week.HasValue)
            {
                query = query.And(GetQuery(filter.JobState.Value));
            }

            if (filter.Year.HasValue)
            {
                query = query.And(f => f.Year == filter.Year);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.And(f => f.Date <= filter.EndDate);
            }

            if (filter.HousingDepartments.HasValue())
            {
                query = query.And(f => filter.HousingDepartments.Contains(f.DepartmentId));
            }

            if (filter.JobStatuses.HasValue())
            {
                query = query.And(f => filter.JobStatuses.Contains(f.StatusId));
            }

            return query;
        }

        public Expression<Func<DayAssign, bool>> GetQuery(ITaskDataFilterModel filter, bool isNotAllowedEmptyDepartment)
        {
            Expression<Func<DayAssign, bool>> query = GetQueryByWeek(filter);

            if (filter.MemberIds.HasValue())
            {
                query = query.And(GetQuery(filter.MemberIds));
            }

            if (filter.JobState.HasValue && filter.Year.HasValue && filter.Week.HasValue)
            {
                query = query.And(GetQuery(filter.JobState.Value));
            }

            if (filter.Year.HasValue)
            {
                query = query.And(f => f.Year == filter.Year);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.And(f => f.Date <= filter.EndDate);
            }

            if (filter.JobStatuses.HasValue())
            {
                query = query.And(f => filter.JobStatuses.Contains(f.StatusId));
            }

            if (isNotAllowedEmptyDepartment)
            {
                query = query.And(f => filter.HousingDepartments.Contains(f.DepartmentId));
            }
            else if (filter.HousingDepartments.HasValue())
            {
                query = query.And(f => filter.HousingDepartments.Contains(f.DepartmentId));
            }

            return query;
        }

        public void UpdateSingleProperty<TProp>(Guid id, Expression<Func<DayAssign, TProp>> property, TProp value)
        {
            var filter = Builders<DayAssign>.Filter.Eq(p => p.Id, id);
            var update = Builders<DayAssign>.Update.Set(property, value);
            collection.UpdateOne(filter, update);
        }

        public bool HasTasks(IWeekPlanFilterModel filter)
        {
            IEnumerable<IDayAssign> dayAssigns = Query.Where(GetQuery(filter));
            return dayAssigns.Any();
        }

        public void UpdateEstimate(List<Guid> idList, int estimateMinutes)
        {
            Expression<Func<DayAssign, bool>> filter = f => idList.Contains(f.Id);
            UpdateDefinition<DayAssign> update = Builders<DayAssign>.Update.Set(d => d.EstimatedMinutes, estimateMinutes);
            collection.UpdateMany(filter, update);
        }

        public void UpdateTeam(List<Guid> idList, bool isAssignedToAllUser, Guid? groupId, Guid teamLeadId, List<Guid> userIdList)
        {
            Expression<Func<DayAssign, bool>> filter = f => idList.Contains(f.Id);
            UpdateDefinition<DayAssign> update = Builders<DayAssign>.Update
                .Set(d => d.IsAssignedToAllUsers, isAssignedToAllUser)
                .Set(d => d.GroupId, groupId)
                .Set(d => d.TeamLeadId, teamLeadId)
                .Set(d => d.UserIdList, userIdList);
            collection.UpdateMany(filter, update);
        }
        private Expression<Func<DayAssign, bool>> GetQuery(IWeekPlanFilterModel filter)
        {
            Expression<Func<DayAssign, bool>> query = i => true;

            if (filter.HousingDepartmentId.HasValue)
            {
                query = query.And(f => f.DepartmentId == filter.HousingDepartmentId.Value);
            }

            query = query.And(f => f.WeekNumber == filter.Week);
            query = query.And(f => f.Year == filter.Year);

            switch (filter.JobState)
            {
                case JobStateType.NotCompleted:
                    query = query.And(f => f.StatusId == JobStatus.Canceled || f.StatusId == JobStatus.Expired);
                    break;
                case JobStateType.InProgress:
                    query = query.And(f => f.StatusId != JobStatus.Completed && f.StatusId != JobStatus.Canceled && f.StatusId != JobStatus.Expired);
                    break;
                case JobStateType.Completed:
                    query = query.And(f => f.StatusId == JobStatus.Completed);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"No such job state {filter.JobState}");
            }

            if (filter.MemberIds.Any())
            {
                query = query.And(f => f.IsAssignedToAllUsers || f.UserIdList.Any(u => filter.MemberIds.Contains(u)));
            }

            return query;
        }

        private Expression<Func<DayAssign, bool>> GetQueryByWeek(ITaskDataFilterModel filter)
        {
            Expression<Func<DayAssign, bool>> query = i => true;
            if (filter.StartWeek.HasValue)
            {
                query = query.And(f => f.WeekNumber >= filter.StartWeek);
            }
            else if (filter.BiggerThanWeek.HasValue)
            {
                query = query.And(f => f.WeekNumber > filter.BiggerThanWeek);
            }
            else if (filter.Week.HasValue)
            {
                query = query.And(f => f.WeekNumber == filter.Week);
            }

            return query;
        }

        private Expression<Func<DayAssign, bool>> GetQuery(IEnumerable<Guid> memberIds)
        {
            var groupIds = groupService.GetAllByUserIds(memberIds).Select(x => x.Id);
            Expression<Func<DayAssign, bool>> query = f =>
                          ((f.IsAssignedToAllUsers && f.UserIdList.Count == 0 && f.GroupId == null) ||
                          (f.GroupId != null && groupIds.Contains(f.GroupId.Value) && f.UserIdList.Count == 0) ||
                          (f.UserIdList != null && memberIds.Any(i => f.UserIdList.Contains(i))) ||
                          (f.TeamLeadId != null && memberIds.Contains(f.TeamLeadId.Value)));

            return query;
        }

        private Expression<Func<DayAssign, bool>> GetQuery(JobStateType jobStateType)
        {
            Expression<Func<DayAssign, bool>> filter = i => true;

            switch (jobStateType)
            {
                case JobStateType.Completed:
                    {
                        filter = filter.And(f => f.StatusId == JobStatus.Completed);
                        break;
                    }
                case JobStateType.NotCompleted:
                    {
                        int currentWeekNumber = DateTime.Now.GetWeekNumber();
                        int currentYear = DateTime.UtcNow.Year;
                        filter = filter.And(f => f.StatusId == JobStatus.Canceled ||
                                                (f.StatusId != JobStatus.Completed &&
                                                 (f.Year < currentYear || (f.Year == currentYear && f.WeekNumber < currentWeekNumber))));
                        break;
                    }
                case JobStateType.InProgress:
                    {
                        filter = filter.And(f => f.StatusId != JobStatus.Completed && f.StatusId != JobStatus.Canceled);
                        break;
                    }
                default:
                    throw new NotImplementedException("GetQuery doesn't implement JobStateType: " + jobStateType);
            }

            return filter;
        }

        private Expression<Func<DayAssign, bool>> GetQuery(MemberDayAssignFilterModel filterModel)
        {
            Expression<Func<DayAssign, bool>> filter = i => true;

            filter = filter.And(f => f.StatusId != JobStatus.Canceled);

            if (filterModel.Day.HasValue)
            {
                filter = filter.And(f => f.WeekDay == filterModel.Day.Value);
            }

            filter = filter.And(f => f.WeekNumber == filterModel.Week);
            filter = filter.And(f => f.Year == filterModel.Year);

            if (filterModel.WithEstimatedMinutes)
            {
                filter = filter.And(f => f.EstimatedMinutes.HasValue && f.EstimatedMinutes.Value != 0);
            }

            if (filterModel.WithDate)
            {
                filter = filter.And(f => f.Date != null);
            }

            if (filterModel.HousingDepartmentIds.HasValue())
            {
                filter = filter.And(f => filterModel.HousingDepartmentIds.Contains(f.DepartmentId));
            }

            return filter;
        }
    }
}