using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Extensions;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public class JobAssignProvider : IJobAssignProvider
    {
        private readonly IMongoCollection<JobAssign> collection;

        public IQueryable<JobAssign> Query => collection.AsQueryable();

        public JobAssignProvider(IYearlyPlanningConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<JobAssign>(nameof(JobAssign));
        }

        public JobAssign GetById(Guid id)
        {
            using (var cursor = collection.FindSync(f => f.Id == id))
            {
                return cursor.FirstOrDefault();
            }
        }

        public JobAssign GetGlobalAssignByJobId(string jobId)
        {
            using (var cursor = collection.FindSync(f => f.JobIdList.Contains(jobId) && f.IsGlobal))
            {
                return cursor.FirstOrDefault();
            }
        }

        public JobAssign GetByJobId(string jobId)
        {
            using (var cursor = collection.FindSync(f => f.JobIdList.Contains(jobId) && f.IsEnabled))
            {
                return cursor.ToList().FirstOrDefault();
            }
        }

        public JobAssign GetAssignByJobIdAndDepartmentId(string jobId, Guid departmentId)
        {
            List<JobAssign> jobAssignList = GetByJobsDepartments(new List<string> { jobId }, new List<Guid> { departmentId });

            return jobAssignList.FirstOrDefault();
        }

        public JobAssign GetAssignByFilters(string jobId, Guid departmentId)
        {
            List<JobAssign> jobAssignList = GetAllJobAssignByFilters(new List<string> { jobId }, new List<Guid> { departmentId });

            return jobAssignList.FirstOrDefault();
        }

        public List<JobAssign> GetByHousingDepartmentForYear(Guid housingDepartmentId, int year)
        {
            Expression<Func<JobAssign, bool>> filter = f => (f.TillYear == default(int)
                                                          || f.TillYear == year)
                                                          && f.HousingDepartmentIdList.Contains(housingDepartmentId);

            List<JobAssign> jobAssignList = Query.Where(filter).ToList();

            return jobAssignList;
        }

        public List<JobAssign> GetAllByJobId(string jobId, bool showWithDisabledAssigns = false)
        {
            using (var cursor = collection.FindSync(f => f.JobIdList.Contains(jobId) && (showWithDisabledAssigns || f.IsEnabled)))
            {
                return cursor.ToList();
            }
        }

        public List<JobAssign> GetByJobsDepartments(IEnumerable<string> jobIds, List<Guid> departmentIdList, int? year = null)
        {
            int filteredYear = year ?? default(int);
            List<JobAssign> result = Query.Where(f => f.IsEnabled
                                     && (f.TillYear == default(int) || filteredYear <= f.TillYear)
                                     && jobIds.Any(i => f.JobIdList.Contains(i))
                                     && f.HousingDepartmentIdList.Any(d => departmentIdList.Contains(d)))
                                          .OrderBy(i => i.IsGlobal).ToList();

            return result;
        }

        public List<JobAssign> GetAllJobAssignByFilters(IEnumerable<string> jobIds, IEnumerable<Guid> departmentIds, int? year = null)
        {
            var filteredYear = year ?? default(int);
            Expression<Func<JobAssign, bool>> filter = f => (f.TillYear == default(int) || filteredYear <= f.TillYear)
                                                            && f.HousingDepartmentIdList.Any(d => departmentIds.Contains(d))
                                                            && jobIds.Any(i => f.JobIdList.Contains(i));

            var result = Query.Where(filter).OrderBy(i => i.IsGlobal).ToList();

            return result;
        }

        public List<JobAssign> GetByJobsDepartmentsWithGlobal(IEnumerable<string> jobIds, List<Guid> departmentIdList)
        {
            Expression<Func<JobAssign, bool>> filter = GetByJobsDepartmentsWithGlobalFilter(jobIds, departmentIdList);

            using (var cursor = collection.FindSync(filter))
            {
                return cursor.ToList();
            }
        }

        //get job assigns for filling of operational tasks
        public List<JobAssign> GetByDepartmentWeekAndYear(Guid departmentId, int year, int weekNumber) //get assign by departmentId, week and selected year
        {
            Expression<Func<JobAssign, bool>> filter = f => f.IsEnabled
                                                            && f.TillYear == year
                                                            && f.HousingDepartmentIdList.Contains(departmentId)
                                                            && f.WeekList.Any(w => w.Number == weekNumber);

            using (var cursor = collection.FindSync(filter))
            {
                return cursor.ToList();
            }
        }

        public List<JobAssign> GetForWeek(Guid departmentId, int year, int weekNumber, IEnumerable<Guid> jobAssignIds)
        {
            jobAssignIds = jobAssignIds ?? Enumerable.Empty<Guid>();
            Expression<Func<JobAssign, bool>> filter = f => jobAssignIds.Contains(f.Id) 
                                                         ||f.HousingDepartmentIdList.Contains(departmentId) 
                                                         && f.IsEnabled 
                                                         && (f.TillYear >= year || f.TillYear == default(int)) 
                                                         && f.WeekList.Any(x => x.Number == weekNumber && x.ChangedBy > default(int));

            List <JobAssign> jobAssignList = Query.Where(filter).ToList();

            return jobAssignList;
        }

        public List<JobAssign> GetForWeekForAllDepartments(int year, int weekNumber, IEnumerable<Guid> jobAssignIds)
        {
            jobAssignIds = jobAssignIds ?? Enumerable.Empty<Guid>();
            Expression<Func<JobAssign, bool>> filter = f => jobAssignIds.Contains(f.Id) 
                                                         || f.IsEnabled 
                                                         && (f.TillYear >= year || f.TillYear == default(int)) 
                                                         && f.WeekList.Any(x => x.Number == weekNumber && x.ChangedBy > default(int));

            List<JobAssign> jobAssignList = Query.Where(filter).ToList();

            return jobAssignList;
        }

        public List<JobAssign> GetAllByJobAssignIdList(IEnumerable<Guid> jobAssignIdList)
        {
            Expression<Func<JobAssign, bool>> filter = f => f.IsEnabled && jobAssignIdList.Contains(f.Id);

            using (var cursor = collection.FindSync(filter))
            {
                return cursor.ToList();
            }
        }

        public List<JobAssign> GetByJobIds(IEnumerable<string> jobIds)
        {
            Expression<Func<JobAssign, bool>> filter = f => f.IsEnabled && f.JobIdList.Any(jobIds.Contains);

            using (var cursor = collection.FindSync(filter))
            {
                return cursor.ToList();
            }
        }

        public int GetEstimate(string jobId, Guid housingDepartmentId)
        {
            JobAssign jobAssign = collection.Find(j => j.IsEnabled && j.JobIdList.Contains(jobId) && j.HousingDepartmentIdList.Contains(housingDepartmentId)).FirstOrDefault();

            if (jobAssign == null || !jobAssign.JobResponsibleList.Any(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId))
            {
                return default(int);
            }

            var estimate = jobAssign.JobResponsibleList.First(r =>r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId).EstimateInMinutes;

            return estimate;
        }

        public List<Guid> ChangeEstimate(string jobId, int estimateInMinutes, Guid housingDepartmentId)
        {
            List<JobAssign> jobAssigns = collection.Find(j => j.IsEnabled && j.JobIdList.Contains(jobId) && j.HousingDepartmentIdList.Contains(housingDepartmentId)).ToList();
            var resultIdList = new List<Guid>();
            foreach (var jobAssign in jobAssigns)
            {
                if (jobAssign.JobResponsibleList.Any(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId))
                {
                    jobAssign.JobResponsibleList.First(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId).EstimateInMinutes = estimateInMinutes;
                }
                else
                {
                    jobAssign.JobResponsibleList.Add(new Responsible
                    {
                        JobId = jobId,
                        EstimateInMinutes = estimateInMinutes,
                        HousingDepartmentId = housingDepartmentId
                    });
                }

                var update = Builders<JobAssign>.Update.Set(j => j.JobResponsibleList, jobAssign.JobResponsibleList);
                var filter = Builders<JobAssign>.Filter.Where(j => j.Id == jobAssign.Id);
                collection.UpdateOne(filter, update);
                resultIdList.Add(jobAssign.Id);
            }

            return resultIdList;
        }

        public List<Guid> ChangeAssignTeam(string jobId, Guid housingDepartmentId, List<Guid> userIdList, bool isAssignedToAllUsers, Guid? groupId, Guid teamLeadId)
        {
            var jobAssigns = collection.Find(j => j.IsEnabled && j.JobIdList.Contains(jobId) && j.HousingDepartmentIdList.Contains(housingDepartmentId)).ToList();
            var resultIdList = new List<Guid>();
            foreach (var jobAssign in jobAssigns)
            {
                if (jobAssign.JobResponsibleList.Any(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId))
                {
                    var index = jobAssign.JobResponsibleList.FindIndex(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId);
                    jobAssign.JobResponsibleList[index].UserIdList = userIdList;
                    jobAssign.JobResponsibleList[index].IsAssignedToAllUsers = isAssignedToAllUsers;
                    jobAssign.JobResponsibleList[index].GroupId = groupId;
                    jobAssign.JobResponsibleList[index].TeamLeadId = teamLeadId;
                }
                else
                {
                    jobAssign.JobResponsibleList.Add(new Responsible
                    {
                        JobId = jobId,
                        UserIdList = userIdList,
                        HousingDepartmentId = housingDepartmentId,
                        IsAssignedToAllUsers = isAssignedToAllUsers,
                        GroupId = groupId,
                        TeamLeadId = teamLeadId
                    });
                }

                var update = Builders<JobAssign>.Update.Set(j => j.JobResponsibleList, jobAssign.JobResponsibleList);
                var filter = Builders<JobAssign>.Filter.Where(j => j.Id == jobAssign.Id);
                collection.UpdateOne(filter, update);
                resultIdList.Add(jobAssign.Id);
            }

            return resultIdList;
        }

        public Responsible GetAssignTeam(string jobId, Guid housingDepartmentId)
        {
            JobAssign jobAssign = collection.Find(j => j.IsEnabled && j.JobIdList.Contains(jobId) && j.HousingDepartmentIdList.Contains(housingDepartmentId)).First();
            
            Responsible result = jobAssign?.JobResponsibleList?.FirstOrDefault(r => r.JobId == jobId && r.HousingDepartmentId == housingDepartmentId);

            return result;
        }

        private Expression<Func<JobAssign, bool>> GetByJobsDepartmentsWithGlobalFilter(IEnumerable<string> jobIds, List<Guid> departmentIdList)
        {
            Expression<Func<JobAssign, bool>> filter = f => f.IsEnabled && jobIds.Any(i => f.JobIdList.Contains(i));

            if (departmentIdList.HasValue())
            {
                filter = filter.And(f => departmentIdList.Any(i => f.HousingDepartmentIdList.Contains(i)));
            }

            return filter;
        }
    }
}