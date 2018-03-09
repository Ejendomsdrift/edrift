using CategoryCore.Contract.Interfaces;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public class JobProvider : IJobProvider
    {
        private readonly IMongoCollection<Job> collection;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly ICategoryService categoryService;
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IManagementDepartmentService managementService;

        public IQueryable<Job> Query => collection.AsQueryable();

        static JobProvider()
        {
            BsonClassMap.RegisterClassMap<Job>(m =>
            {
                m.AutoMap();
                m.SetIgnoreExtraElements(true);
                m.UnmapField(i => i.FirstAddress);
            });
        }

        public JobProvider(
            IYearlyPlanningConfiguration configuration,
            IDayAssignProvider dayAssignProvider,
            ICategoryService categoryService,
            IJobAssignProvider jobAssignProvider,
            IManagementDepartmentService managementService)
        {
            this.dayAssignProvider = dayAssignProvider;
            this.categoryService = categoryService;
            this.jobAssignProvider = jobAssignProvider;
            this.managementService = managementService;
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<Job>(nameof(Job));
        }

        public void UpdateSingleProperty<TProp>(string id, Expression<Func<Job, TProp>> property, TProp value)
        {
            var filter = Builders<Job>.Filter.Eq(p => p.Id, id);
            var update = Builders<Job>.Update.Set(property, value);
            collection.UpdateOne(filter, update);
        }

        public Task<Job> Get(string id)
        {
            using (var cursor = collection.FindSync(f => f.Id == id, new FindOptions<Job> { Limit = 1 }))
            {
                return cursor.FirstOrDefaultAsync();
            }

        }

        public Job GetForManagementDepartment(string id, Guid? managementDepartmentId)
        {
            using (var cursor = collection.FindSync(f => f.Id == id, new FindOptions<Job> { Limit = 1 }))
            {
                Job job = cursor.FirstOrDefault();
                IEnumerable<IHousingDepartmentModel> departments = managementDepartmentId.HasValue ?
                    managementService.GetHousingDepartments(managementDepartmentId.Value) :
                    managementService.GetAllHousingDepartments();

                return FillAssigns(new List<Job> { job }, departments.Select(x => x.Id), true).FirstOrDefault();
            }
        }

        public Job GetForHousingDepartment(string id, Guid departmentId)
        {
            using (var cursor = collection.FindSync(f => f.Id == id, new FindOptions<Job> { Limit = 1 }))
            {
                Job job = cursor.FirstOrDefault();

                return FillAssigns(new List<Job> { job }, new List<Guid> { departmentId }).FirstOrDefault();
            }
        }

        public List<Job> GetByIds(IEnumerable<string> ids, bool fillCategories = false)
        {
            using (var cursor = collection.FindSync(f => ids.Contains(f.Id)))
            {
                List<Job> jobList = cursor.ToList();

                if (fillCategories)
                {
                    FillCategories(jobList.Where(j => j.JobTypeId == JobTypeEnum.Facility || j.JobTypeId == JobTypeEnum.AdHock));
                }

                return jobList;
            }
        }

        public List<Job> GetHiddenByIds(IEnumerable<string> ids)
        {
            using (var cursor = collection.FindSync(f => f.IsHidden && ids.Contains(f.Id)))
            {
                List<Job> jobList = cursor.ToList();

                return jobList;
            }
        }

        public List<Job> GetByIdsWithAssigns(IEnumerable<string> ids, Guid departmentId, int year)
        {
            using (var cursor = collection.FindSync(f => ids.Contains(f.Id)))
            {
                List<Job> jobList = cursor.ToList();

                return FillAssigns(jobList, new List<Guid> { departmentId }, false, year);
            }
        }

        public List<Job> GetByCategoryId(Guid categoryId)
        {
            using (var cursor = collection.FindSync(f => f.CategoryId == categoryId))
            {
                List<Job> jobList = cursor.ToList();

                return jobList;
            }
        }

        public List<Job> Get(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true, bool onlyFacilityTask = false)
        {
            Expression<Func<Job, bool>> filter = GetFilter(categoryIds, includeGroupedTasks, includeHiddenTasks, onlyFacilityTask);
            using (var cursor = collection.FindSync(filter))
            {
                List<Job> jobList = cursor.ToList();

                return FillAssigns(jobList, getWithGlobal: true);
            }
        }

        public List<Job> Get(IEnumerable<Guid> categoryIds, IEnumerable<Guid> housingDepartmentIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true)
        {
            Expression<Func<Job, bool>> filter = GetFilter(categoryIds, includeGroupedTasks, includeHiddenTasks);
            using (var cursor = collection.FindSync(filter))
            {
                List<Job> jobList = cursor.ToList();

                var jobWithAssign = FillAssigns(jobList, housingDepartmentIds, getWithGlobal: true);

                var result = jobWithAssign.Where(j => j.Assigns.Any()).ToList();

                return result;
            }
        }

        public List<Job> GetByCategoryIdsForCoordinator(IEnumerable<Guid> categoryIds, IMemberModel currentUser, bool onlyFacilityTask = false)
        {
            Expression<Func<Job, bool>> filter = GetByCategoryIdsForCoordinatorFilter(categoryIds, onlyFacilityTask);
            IEnumerable<Guid> managenentDepartmentIds = currentUser.ManagementsToActiveRolesRelation[currentUser.CurrentRole];
            List<Guid> departmentIdList = managementService.GetHousingDepartmentsByManagementIds(managenentDepartmentIds).Select(d => d.Id).ToList();

            using (var cursor = collection.FindSync(filter))
            {
                List<Job> jobList = cursor.ToList();
                FillAssigns(jobList, departmentIdList, true);
                List<Job> result = jobList.FindAll(t => t.ParentId == null || t.RelationGroupList.Any(x => departmentIdList.Contains(x.HousingDepartmentId)));

                return result;
            }
        }

        public List<Job> GetByCategoryIdsWithAssigns(IEnumerable<Guid> categoryIds, Guid departmentId)
        {
            using (var cursor = collection.FindSync(f => categoryIds.Contains(f.CategoryId)))
            {
                List<Job> jobList = cursor.ToList();

                return FillAssigns(jobList, new List<Guid> { departmentId }, true);
            }
        }

        public List<Job> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber, bool fillDayAssigns = true, bool fillCategories = true)
        {
            List<JobAssign> assignList = GetJobAssigns(departmentId, year, weekNumber);
            assignList.ForEach(i => i.WeekList = i.WeekList.Where(w => !w.IsDisabled && w.Number == weekNumber).ToList());

            List<Job> result = GetJobsWithFilledDayAssignsAndCategories(assignList, fillDayAssigns, fillCategories, year, weekNumber, departmentId);

            return result;
        }

        public List<Job> GetByYearWeekForAllDepartments(int year, int weekNumber, bool fillDayAssigns = true, bool fillCategories = true)
        {
            List<JobAssign> assignList = GetJobAssignsForAllDepartments(year, weekNumber);
            assignList.ForEach(i => i.WeekList = i.WeekList.Where(w => !w.IsDisabled && w.Number == weekNumber).ToList());

            List<Job> result = GetJobsWithFilledDayAssignsAndCategories(assignList, fillDayAssigns, fillCategories, year, weekNumber, departmentId: null);

            return result;
        }

        public List<Job> GetJobsByJobType(JobTypeEnum jobTypeId)
        {
            using (var cursor = collection.FindSync(f => f.JobTypeId == jobTypeId))
            {
                List<Job> taskList = cursor.ToList();

                return FillDayAssigns(taskList);
            }
        }

        public void SaveRelationGroupId(string id, RelationGroupModel model)
        {
            var filter = Builders<Job>.Filter.Eq(p => p.Id, id);
            var update = Builders<Job>.Update.Push(i => i.RelationGroupList, model);
            collection.UpdateOne(filter, update);
        }

        public long ClearData()
        {
            return collection.DeleteMany(FilterDefinition<Job>.Empty).DeletedCount;
        }

        public List<Job> GetByParentId(string parentId)
        {
            using (var cursor = collection.FindSync(f => f.ParentId == parentId))
            {
                List<Job> jobList = cursor.ToList();

                return jobList;
            }
        }

        #region utils

        private List<Job> FillDayAssigns(List<Job> jobList)
        {
            foreach (var job in jobList)
            {
                job.DayAssigns = dayAssignProvider.Query.Where(da => da.JobId == job.Id).ToList<IDayAssign>();
            }

            return jobList;
        }

        private List<JobAssign> GetJobAssigns(Guid departmentId, int year, int weekNumber)
        {
            Expression<Func<DayAssign, bool>> dayAssignFilter = i => i.DepartmentId == departmentId && i.Year == year && i.WeekNumber == weekNumber;

            List<Guid> jobAssignIdList = dayAssignProvider.Query.Where(dayAssignFilter).Select(i => i.JobAssignId).ToList();
            List<JobAssign> assignList = jobAssignProvider.GetForWeek(departmentId, year, weekNumber, jobAssignIdList);

            return assignList;
        }

        private List<JobAssign> GetJobAssignsForAllDepartments(int year, int weekNumber)
        {
            List<Guid> jobAssignIdList = dayAssignProvider.Query.Where(i => i.Year == year && i.WeekNumber == weekNumber).Select(i => i.JobAssignId).ToList();
            List<JobAssign> assignList = jobAssignProvider.GetForWeekForAllDepartments(year, weekNumber, jobAssignIdList);

            return assignList;
        }

        private List<Job> GetJobsWithFilledDayAssignsAndCategories(List<JobAssign> assigns, bool fillDayAssigns, bool fillCategories, int year, int weekNumber, Guid? departmentId)
        {
            List<string> idList = assigns.SelectMany(x => x.JobIdList).ToList();
            List<Job> jobList = Query.Where(i => !i.IsHidden && idList.Contains(i.Id)).ToList();

            FillAssigns(jobList, assigns);

            if (fillCategories)
            {
                FillCategories(jobList);
            }

            if (fillDayAssigns)
            {
                FillDayAssigns(jobList, departmentId, year, weekNumber);
            }

            var result = jobList.Where(x => IsJobAssignedToHousingDepartment(x, departmentId)).ToList();

            return result;
        }

        private bool IsJobAssignedToHousingDepartment(IJob job, Guid? housingDepartmentId)
        {
            if (job.RelationGroupList.HasValue() && housingDepartmentId.HasValue)
            {
                if (job.RelationGroupList.Any(x => x.HousingDepartmentId == housingDepartmentId))
                {
                    return true;
                }
                else
                {
                    var result = job.ParentId == null && (job.DayAssigns.Any(x => x.DepartmentId == housingDepartmentId) || job.Assigns.Any(x => x.HousingDepartmentIdList.Contains(housingDepartmentId.Value)));

                    return result;
                }
            }

            return true;
        }

        private void FillDayAssigns(IEnumerable<Job> jobs, Guid? departmentId, int year, int weekNumber)
        {
            var dayAssignList = new List<DayAssign>();

            if (departmentId.HasValue)
            {
                dayAssignList = dayAssignProvider.GetForWeek(departmentId.Value, year, weekNumber);
            }
            else
            {
                dayAssignList = dayAssignProvider.GetForWeekForAllDepartments(year, weekNumber);
            }

            foreach (var job in jobs)
            {
                IEnumerable<DayAssign> jobDayAssigns = dayAssignList.Where(i => i.JobId == job.Id);
                job.DayAssigns.AddRange(jobDayAssigns);
            }
        }

        private List<Job> FillAssigns(List<Job> jobList, IEnumerable<Guid> departmentIds = null, bool getWithGlobal = false, int? year = null)
        {
            IEnumerable<string> jobIds = jobList.Select(x => x.Id);
            List<Guid> departmentIdList = departmentIds.HasValue() ? departmentIds.ToList() : new List<Guid>();


            List<JobAssign> assignList = getWithGlobal
                ? jobAssignProvider.GetByJobsDepartmentsWithGlobal(jobIds, departmentIdList)
                : jobAssignProvider.GetByJobsDepartments(jobIds, departmentIdList, year);

            if (assignList.Any())
            {
                Parallel.ForEach(jobList, i =>
                {
                    i.Assigns = assignList.Where(x => x.JobIdList.Contains(i.Id)).ToList();
                });
            }

            return jobList;
        }

        private void FillAssigns(List<Job> jobs, IEnumerable<JobAssign> assigns)
        {
            List<JobAssign> assignList = assigns.AsList();

            foreach (var job in jobs)
            {
                job.Assigns = GetJobAssigns(job, assignList).ToList();
            }
        }

        private void FillCategories(IEnumerable<Job> jobs)
        {
            List<Job> jobList = jobs.AsList();
            List<ICategoryModel> categoryList = categoryService.GetByIds(jobList.Select(f => f.CategoryId)).ToList();

            foreach (var job in jobList)
            {
                job.Category = categoryList.FirstOrDefault(c => c.Id == job.CategoryId);
            }
        }

        private Expression<Func<Job, bool>> GetFilter(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true, bool onlyFacilityTask = false)
        {
            Expression<Func<Job, bool>> filter = f => categoryIds.Contains(f.CategoryId) && f.JobTypeId == JobTypeEnum.Facility;

            if (!includeGroupedTasks)
            {
                filter = filter.And(f => !f.RelationGroupList.Any());
            }

            if (onlyFacilityTask)
            {
                filter = filter.And(f => f.JobTypeId == JobTypeEnum.Facility);
            }

            if (!includeHiddenTasks)
            {
                filter = filter.And(f => !f.IsHidden);
            }

            return filter;
        }

        private Expression<Func<Job, bool>> GetFilter(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true)
        {
            Expression<Func<Job, bool>> filter = f => categoryIds.Contains(f.CategoryId) && f.JobTypeId == JobTypeEnum.Facility;

            if (!includeGroupedTasks)
            {
                filter = filter.And(f => !f.RelationGroupList.Any());
            }

            if (!includeHiddenTasks)
            {
                filter = filter.And(f => !f.IsHidden);
            }

            return filter;
        }

        private Expression<Func<Job, bool>> GetByCategoryIdsForCoordinatorFilter(IEnumerable<Guid> categoryIds, bool onlyFacilityTask = false)
        {
            Expression<Func<Job, bool>> filter = f => categoryIds.Contains(f.CategoryId);

            if (onlyFacilityTask)
            {
                filter = filter.And(f => f.JobTypeId == JobTypeEnum.Facility);
            }

            return filter;
        }

        private IEnumerable<JobAssign> GetJobAssigns(Job job, IEnumerable<JobAssign> assigns)
        {
            List<JobAssign> jobAssignList = assigns.Where(x => x.JobIdList.Contains(job.Id)).ToList();

            IEnumerable<JobAssign> filteredAssigns = jobAssignList.Where(i => !i.IsGlobal || !jobAssignList.Any(x => x.HousingDepartmentIdList.Any(d => i.HousingDepartmentIdList.Contains(d)) && !x.IsGlobal));

            return filteredAssigns;
        }
        #endregion
    }
}