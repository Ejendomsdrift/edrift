using CategoryCore.Contract.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class YearlyPlanService : IYearlyPlanService
    {
        private readonly IJobProvider jobProvider;
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly ICategoryService categoryService;
        private readonly IMemberService memberService;

        private List<YearPlanWeekData> emptyWeeks;

        private List<YearPlanWeekData> EmptyWeeks
        {
            get
            {
                return emptyWeeks ?? (emptyWeeks = Enumerable.Range(0, Constants.DateTime.WeeksInYear).Select(x => new YearPlanWeekData
                {
                    WeekNumber = x + 1,
                    Status = YearTaskStatus.NotDefined
                }).ToList());
            }
        }

        public YearlyPlanService(
            IJobProvider jobProvider,
            ICategoryService categoryService,
            IDayAssignProvider dayAssignProvider,
            IMemberService memberService,
            IJobAssignProvider jobAssignProvider)
        {
            this.jobProvider = jobProvider;
            this.categoryService = categoryService;
            this.dayAssignProvider = dayAssignProvider;
            this.memberService = memberService;
            this.jobAssignProvider = jobAssignProvider;
        }

        public DepartmentYearPlanViewModel GetYearPlanCategories()
        {
            var result = new DepartmentYearPlanViewModel();

            IEnumerable<ICategoryModel> categories = categoryService.GetTree();
            List<Tuple<ICategoryModel, int>> categoryList = categories.FlattenWithLevel(c => c.Children).ToList();
            IEnumerable<Guid> categoryIds = categoryList.Select(c => c.Item1.Id);
            IMemberModel currentUser = memberService.GetCurrentUser();
            List<Job> jobList = GetJobs(categoryIds, currentUser);

            result.YearPlanItems = GetYearPlanItems(currentUser, categoryList, jobList);
            return result;
        }

        public IEnumerable<YearPlanItemViewModel> GetYearPlanDepartments(string taskId, int year)
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            IEnumerable<IManagementDepartmentModel> managementDepartments = memberService.GetUserManagementDepartments(currentUser.MemberId, onlyActiveManagementDepartment: true);
            List<IHousingDepartmentModel> housingDepartmentList = managementDepartments.SelectMany(x => x.HousingDepartmentList).ToList();

            if (!housingDepartmentList.Any())
            {
                return Enumerable.Empty<YearPlanItemViewModel>();
            }

            Job facilityTask = taskId.IsNotNullOrEmpty() ? jobProvider.GetForManagementDepartment(taskId, currentUser.ActiveManagementDepartmentId) : null;

            if (facilityTask != null)
            {
                IEnumerable<DayAssign> dayAssigns = dayAssignProvider.Query.Where(i => i.JobId == facilityTask.Id && i.Year == year);
                facilityTask.DayAssigns = dayAssigns.ToList<IDayAssign>();
            }

            IEnumerable<YearPlanItemViewModel> result = housingDepartmentList.Select(x => ToYearPlanDepartmentViewModel(facilityTask, x.Id, $"{x.SyncDepartmentId} {x.Name}", year));

            return result;
        }

        public IDictionary<string, List<YearPlanWeekData>> GetYearPlanWeekData(Guid departmentId, int year)
        {
            List<JobAssign> jobAssigns = jobAssignProvider.GetByHousingDepartmentForYear(departmentId, year);
            IEnumerable<string> jobIdList = jobAssigns.SelectMany(a => a.JobIdList).Distinct();

            List<Job> jobList = jobProvider.GetByIds(jobIdList).Where(job => job.JobTypeId == JobTypeEnum.Facility).ToList();

            IDictionary<string, List<YearPlanWeekData>> yearPlanWeekData = GetYearPlanWeekData(departmentId, year, jobList);

            return yearPlanWeekData;
        }

        public HousingDepartmentYearPlanModel GetAllData(Guid departmentId, int year)
        {
            IEnumerable<ICategoryModel> categories = categoryService.GetTree();
            List<Tuple<ICategoryModel, int>> categoryList = categories.FlattenWithLevel(c => c.Children).ToList();
            IEnumerable<Guid> categoryIds = categoryList.Select(c => c.Item1.Id);
            IMemberModel currentUser = memberService.GetCurrentUser();
            List<Job> jobList = currentUser.IsAdmin()
                ? jobProvider.Get(categoryIds, onlyFacilityTask: true)
                : jobProvider.GetByCategoryIdsForCoordinator(categoryIds, currentUser, true);

            IEnumerable<YearPlanItemViewModel> yearPlanItems = GetYearPlanItems(currentUser, categoryList, jobList);
            IDictionary<string, List<YearPlanWeekData>> yearPlanWeekData = GetYearPlanWeekData(departmentId, year, jobList);

            var result = new HousingDepartmentYearPlanModel
            {
                YearPlanItems = yearPlanItems,
                WeeksData = yearPlanWeekData
            };
            return result;
        }

        private YearPlanItemViewModel ToYearPlanDepartmentViewModel(Job facilityTask, Guid? departmentId, string departmentName, int year)
        {
            var result = new YearPlanItemViewModel
            {
                Id = departmentId?.ToString(),
                Name = departmentName,
                Weeks = EmptyWeeks
            };

            if (facilityTask == null)
            {
                return result;
            }

            result.IsDisabled = facilityTask.IsHidden;
            result.IsAssigned = facilityTask.Assigns.Count > 0;

            if (departmentId.HasValue)
            {
                JobAssign assignedDepartment = GetJobAssign(facilityTask, departmentId.Value);

                if (assignedDepartment != null)
                {
                    facilityTask.Assigns.Add(assignedDepartment);
                    result.Weeks = ToYearPlanWeekData(assignedDepartment.WeekList, facilityTask, year, departmentId.Value);
                }
            }

            return result;
        }

        private JobAssign GetJobAssign(Job job, Guid housingDepartmentId)
        {
            JobAssign localAssign = null;
            JobAssign globalAssign = null;

            if (job.ParentId != null)
            {
                RelationGroupModel model = job.RelationGroupList.First();
                localAssign = job.Assigns.FirstOrDefault(x => !x.IsGlobal && x.HousingDepartmentIdList.Contains(housingDepartmentId));
                globalAssign = model.HousingDepartmentId == housingDepartmentId
                        ? job.Assigns.FirstOrDefault(x => x.IsGlobal && x.HousingDepartmentIdList.Contains(housingDepartmentId))
                        : null;
            }
            else
            {
                localAssign = job.Assigns.FirstOrDefault(x => !x.IsGlobal && x.HousingDepartmentIdList.Contains(housingDepartmentId));
                globalAssign = job.Assigns.FirstOrDefault(x => x.IsGlobal && x.HousingDepartmentIdList.Contains(housingDepartmentId));
            }

            return localAssign ?? globalAssign;
        }

        private YearPlanItemViewModel ToYearPlanItemViewModel(Tuple<YearPlanItem, int> source, List<Job> childJobList)
        {
            bool isGroupedJob = source.Item1.RelationGroupList.Any();
            bool isParentGroupedTask = isGroupedJob && !source.Item1.IsChildTask;
            string address = isParentGroupedTask ? string.Empty : source.Item1.Address;

            IDictionary<Guid, string> addressListForParentTask = isParentGroupedTask && source.Item1.AddressList.HasValue()
                ? GetJobAddressListForParentTask(source.Item1.Id, source.Item1.AddressList, source.Item1.RelationGroupList.Select(x => x.HousingDepartmentId), childJobList)
                : new Dictionary<Guid, string>();

            IEnumerable<Guid> assignedHousingDepartmentIds = isGroupedJob && source.Item1.IsChildTask
                ? source.Item1.RelationGroupList.Select(x => x.HousingDepartmentId)
                : source.Item1.AssignedHousingDepartmentIdList;

            return new YearPlanItemViewModel
            {
                Id = source.Item1.Id,
                Name = source.Item1.Name,
                Color = source.Item1.Color,
                IsDisabled = source.Item1.IsDisabled,
                IsAssigned = source.Item1.IsAssigned,
                IsTask = source.Item1.IsTask,
                ParentCategoryId = source.Item1.ParentCategoryId,
                Weeks = source.Item1.Weeks,
                JobTypeId = source.Item1.JobTypeId,
                Level = source.Item1.Level + source.Item2,
                Address = address,
                IsGroupedJob = isGroupedJob,
                IsParentGroupedJob = isParentGroupedTask,
                AddressListForParentTask = addressListForParentTask,
                ByCoordinator = source.Item1.ByCoordinator,
                AssignedHousingDepartmentIdList = assignedHousingDepartmentIds
            };
        }

        private Dictionary<Guid, string> GetJobAddressListForParentTask(
            string jobId, IEnumerable<JobAddress> addressList, IEnumerable<Guid> groupedHousingDepartmentIds, List<Job> childJobList)
        {
            var result = new Dictionary<Guid, string>();

            foreach (var id in groupedHousingDepartmentIds)
            {
                JobAddress addressModel = addressList.FirstOrDefault(x => x.HousingDepartmentId == id);
                if (addressModel != null && IsAnyChildJobsVisible(jobId, id, childJobList))
                {
                    result.Add(id, addressModel.Address);
                }
            }

            return result;
        }

        private bool IsAnyChildJobsVisible(string jobId, Guid housingDepartmentId, List<Job> childJobList)
        {
            IEnumerable<Job> childJobs = childJobList.Where(x => x.ParentId == jobId && x.RelationGroupList.Any(y => y.HousingDepartmentId == housingDepartmentId));

            return childJobs.Any(x => !x.IsHidden);
        }

        private YearPlanItem ToYearPlanItem(Tuple<ICategoryModel, int> category, List<Job> tasks)
        {
            var model = new YearPlanItem
            {
                Id = category.Item1.Id.ToString(),
                Name = category.Item1.Name,
                Color = category.Item1.Color,
                Level = category.Item2,
                IsTask = false,
                ParentCategoryId = category.Item1.ParentId,
                IsDisabled = false,
                IsAssigned = false,
                ByCoordinator = false,
                Weeks = new List<YearPlanWeekData>(),
                Tasks = ConvertJobs(category, tasks)
            };

            return model;
        }

        private IEnumerable<YearPlanItem> ConvertJobs(Tuple<ICategoryModel, int> category, List<Job> jobs)
        {
            List<Job> foundedJobList = jobs.FindAll(t => t.CategoryId == category.Item1.Id);
            List<YearPlanItem> selectedJobList = foundedJobList.Select(tt => ToYearPlanTaskViewModel(tt, category)).Where(t => t != null).ToList();

            return selectedJobList.OrderBy(t => t.Name).ThenBy(i => i.IsChildTask).ThenBy(i => i.CreationDate);
        }

        private YearPlanItem ToYearPlanTaskViewModel(Job job, Tuple<ICategoryModel, int> category)
        {
            if (job == null)
            {
                return null;
            }

            var result = new YearPlanItem
            {
                Id = job.Id,
                IsTask = true,
                IsChildTask = job.ParentId != null,
                ParentCategoryId = category.Item1.Children.HasValue() ? category.Item1.ParentId : category.Item1.Id,
                IsDisabled = job.IsHidden,
                Color = category.Item1.Color,
                Level = category.Item2,
                JobTypeId = job.JobTypeId,
                Name = job.Title,
                IsAssigned = job.Assigns.Any(m => m.HousingDepartmentIdList.Any()),
                Address = GetAddress(job),
                AddressList = job.AddressList,
                CreationDate = job.CreationDate,
                RelationGroupList = job.RelationGroupList,
                Tasks = new List<YearPlanItem>(),
                Weeks = EmptyWeeks,
                ByCoordinator = job.CreatedByRole == RoleType.Coordinator,
                AssignedHousingDepartmentIdList = job.Assigns.SelectMany(x => x.HousingDepartmentIdList)
            };

            return result;
        }

        private KeyValuePair<string, List<YearPlanWeekData>> GetYearPlanWeekDataForDepartment(Job job, List<Job> jobs, Guid departmentId, int year)
        {
            JobAssign departmentJobAssign = GetJobAssignForDepartment(job, jobs, departmentId);
            List<YearPlanWeekData> weekData = EmptyWeeks;

            if (departmentJobAssign != null && (departmentJobAssign.TillYear == default(int) || departmentJobAssign.TillYear >= year))
            {
                weekData = ToYearPlanWeekData(departmentJobAssign.WeekList, job, year, departmentId);
            }

            return new KeyValuePair<string, List<YearPlanWeekData>>(job.Id, weekData);
        }

        private JobAssign GetJobAssignForDepartment(Job job, List<Job> jobs, Guid housingDepartmentId)
        { 
            Job parentJob = GetParentJob(job.ParentId, jobs);

            if (IsValidChildTask(job, housingDepartmentId) && parentJob != null)
            {
                JobAssign parentGlobalAssign = parentJob.Assigns.First(x => x.IsGlobal);
                return parentGlobalAssign;
            }

            return GetCorrectJobAssign(job, housingDepartmentId);
        }

        private bool IsValidChildTask(Job job, Guid housingDepartmentId)
        {
            bool result = job.ParentId != null &&
                          job.RelationGroupList.Any(x => x.HousingDepartmentId == housingDepartmentId) &&
                          job.Assigns.Count(x => x.IsGlobal) == 1;

            return result;
        }

        private Job GetParentJob(string parentJobId, List<Job> jobs)
        {
            if (parentJobId.IsNullOrEmpty())
            {
                return null;
            }

            Job parentJob = jobs.FirstOrDefault(x => x.Id == parentJobId);

            return parentJob;
        }

        private JobAssign GetCorrectJobAssign(Job job, Guid housingDepartmentId)
        {
            JobAssign localJobAssign = job.Assigns.FirstOrDefault(x => !x.IsGlobal && x.HousingDepartmentIdList.Contains(housingDepartmentId));
            JobAssign globalJobAssign = job.Assigns.FirstOrDefault(x => x.HousingDepartmentIdList.Contains(housingDepartmentId));

            return localJobAssign ?? globalJobAssign;
        }

        private List<YearPlanWeekData> ToYearPlanWeekData(IEnumerable<WeekModel> weeks, Job job, int year, Guid housingDepartmentId)
        {
            IDictionary<int, WeekModel> weekDictionary = weeks.ToDictionary(w => w.Number, w => w);
            List<YearPlanWeekData> result = Enumerable.Range(1, Constants.DateTime.WeeksInYear).Select(i =>
            {
                WeekModel week;
                if (weekDictionary.TryGetValue(i, out week))
                {
                    return new YearPlanWeekData
                    {
                        WeekNumber = week.Number,
                        ChangedBy = week.ChangedBy,
                        IsDisabled = week.IsDisabled,
                        Status = GetWeekStatus(week, job, year, housingDepartmentId)
                    };
                }
                else
                {
                    return new YearPlanWeekData
                    {
                        WeekNumber = i,
                        Status = YearTaskStatus.NotDefined
                    };
                }
            }).ToList();

            return result;
        }

        private YearTaskStatus GetWeekStatus(WeekModel week, Job job, int year, Guid housingDepartmentId)
        {
            int currentWeek = DateTime.UtcNow.GetWeekNumber();
            int currentYear = DateTime.UtcNow.Year;
            IEnumerable<JobAssign> jobAssignsForWeek = job.Assigns
                .Where(x => (x.TillYear == default(int) || x.TillYear >= currentYear) &&
                            x.WeekList.Any(y => y.Number == week.Number) &&
                            x.HousingDepartmentIdList.Contains(housingDepartmentId));

            List<IDayAssign> dayAssignsForWeek = job.DayAssigns
                .Where(x => x.Year == year && x.WeekNumber == week.Number)
                .ToList();

            bool isJobExisted = year > job.CreationDate.Year || (year == job.CreationDate.Year && week.Number >= job.CreationDate.GetWeekNumber()) &&
                                (year > currentYear || week.Number >= currentWeek || dayAssignsForWeek.HasValue());

            List<IDayAssign> filteredDayAssignForWeek = dayAssignsForWeek.Where(x => x.ExpiredWeekNumber == week.Number || x.ExpiredWeekNumber == null).ToList();
            if (filteredDayAssignForWeek.Any(x => x.StatusId == JobStatus.Expired))
            {
                CheckExpiredDayAssignStatus(job, housingDepartmentId, filteredDayAssignForWeek, currentWeek, currentYear);
            }

            // TODO: If you think that you can refactor this, notify Igor about you courage.
            bool isAllDayAssignCompleted = filteredDayAssignForWeek.Any() && filteredDayAssignForWeek.All(x => x.StatusId == JobStatus.Completed);
            bool isAllTicketHasDayAssign = IsTaskHasOnlyRealTicket(job.Assigns, filteredDayAssignForWeek);

            if (week.ChangedBy == (int)ChangedByRole.None || !isJobExisted || !jobAssignsForWeek.HasValue())
            {
                return YearTaskStatus.NotDefined;
            }
            else if (isAllDayAssignCompleted && isAllTicketHasDayAssign)
            {
                return YearTaskStatus.Finished;
            }
            else if (year > currentYear || (year == currentYear && week.Number >= currentWeek))
            {
                return YearTaskStatus.NotStarted;
            }
            else
            {
                return YearTaskStatus.Failed;
            }
        }

        private bool IsTaskHasOnlyRealTicket(List<JobAssign> jobAssigns, List<IDayAssign> dayAssignList)
        {
            JobAssign localJobAssign = jobAssigns.FirstOrDefault(x => !x.IsGlobal);
            JobAssign jobAssign = localJobAssign ?? jobAssigns.First();

            return jobAssign.RepeatsPerWeek <= dayAssignList.Count;
        }

        private List<Job> GetJobs(IEnumerable<Guid> categoryIds, IMemberModel user)
        {
            List<Job> jobs = user.IsAdmin()
                    ? jobProvider.Get(categoryIds)
                    : jobProvider.GetByCategoryIdsForCoordinator(categoryIds, user);

            return jobs;
        }

        private string GetAddress(Job job)
        {
            string result = job.RelationGroupList.Any() ? job.FirstAddress : null;

            return result;
        }

        private void FillJobAssigns(List<Job> jobs, List<JobAssign> jobAssigns)
        {
            Parallel.ForEach(jobs, i =>
            {
                i.Assigns = jobAssigns.Where(x => x.JobIdList.Contains(i.Id) && x.IsEnabled).ToList();
            });
        }

        private void FillDayAssigns(List<Job> jobs, int year, Guid departmentId)
        {
            IEnumerable<string> jobIds = jobs.Select(i => i.Id);
            List<DayAssign> dayAssigns = dayAssignProvider.Query.Where(i => i.Year == year && i.DepartmentId == departmentId && jobIds.Contains(i.JobId)).ToList();

            Parallel.ForEach(jobs, i =>
            {
                i.DayAssigns = dayAssigns.Where(x => x.JobId == i.Id).ToList<IDayAssign>();
            });
        }

        private void CheckExpiredDayAssignStatus(Job job, Guid housingDepartmentId, List<IDayAssign> dayAssignsForWeek, int currentWeek, int currentYear)
        {
            List<IDayAssign> relatedDayAssigns = GetRelatedDayAssigns(job, housingDepartmentId, currentWeek, currentYear);

            if (relatedDayAssigns.Any(x => x.StatusId == JobStatus.Completed))
            {
                foreach (var expiredDayAssign in dayAssignsForWeek.Where(x => x.StatusId == JobStatus.Expired))
                {
                    expiredDayAssign.StatusId = IsDayAssignWasCompleted(expiredDayAssign, relatedDayAssigns) ? JobStatus.Completed : expiredDayAssign.StatusId;
                }
            }
        }

        private List<IDayAssign> GetRelatedDayAssigns(Job job, Guid housingDepartmentId, int currentWeekNumber, int currentYear)
        {
            IEnumerable<IDayAssign> relatedDayAssigns = job.DayAssigns
                .Where(x => x.DepartmentId == housingDepartmentId &&
                       x.WeekNumber <= currentWeekNumber &&
                      (x.Year >= currentYear || x.Year == default(int))).ToList();

            return relatedDayAssigns.ToList();
        }

        private bool IsDayAssignWasCompleted(IDayAssign expiredDayAssign, List<IDayAssign> relatedDayAssigns)
        {
            var expiredChildDayAssign = relatedDayAssigns.FirstOrDefault(x => x.ExpiredDayAssignId == expiredDayAssign.Id);
            if (expiredChildDayAssign != null && expiredChildDayAssign.StatusId == JobStatus.Expired)
            {
                return IsDayAssignWasCompleted(expiredChildDayAssign, relatedDayAssigns);
            }

            return expiredChildDayAssign != null && expiredChildDayAssign.StatusId == JobStatus.Completed;
        }

        private IEnumerable<YearPlanItemViewModel> GetYearPlanItems(IMemberModel currentUser, List<Tuple<ICategoryModel, int>> categoryList, List<Job> jobList)
        {
            if (currentUser.CurrentRole == RoleType.Coordinator)
            {
                jobList = jobList.Where(x => !x.IsHidden || x.IsHidden && x.CreatedByRole == currentUser.CurrentRole).ToList();
            }

            List<YearPlanItem> catJobs = categoryList.Select(c => ToYearPlanItem(c, jobList)).ToList();

            IEnumerable<string> jobIds = catJobs.Where(x => x.IsTask).Select(x => x.Id).Distinct();
            List<Job> childJobList = jobProvider.Query.Where(x => jobIds.Contains(x.ParentId)).ToList();

            IEnumerable<YearPlanItemViewModel> yearPlanItems = catJobs.FlattenWithLevel(c => c.Tasks).Select(x => ToYearPlanItemViewModel(x, childJobList));

            return yearPlanItems;
        }

        private IDictionary<string, List<YearPlanWeekData>> GetYearPlanWeekData(Guid departmentId, int year, List<Job> jobList)
        {
            var weeksDataResult = new ConcurrentBag<KeyValuePair<string, List<YearPlanWeekData>>>();

            List<JobAssign> jobAssigns = jobAssignProvider.GetByHousingDepartmentForYear(departmentId, year);

            FillJobAssigns(jobList, jobAssigns);
            FillDayAssigns(jobList, year, departmentId);
            
            Parallel.ForEach(jobList, i =>
            {
                weeksDataResult.Add(GetYearPlanWeekDataForDepartment(i, jobList, departmentId, year));
            });
            
            IDictionary<string, List<YearPlanWeekData>> yearPlanWeekData = weeksDataResult.ToDictionary(k => k.Key, v => v.Value);

            return yearPlanWeekData;
        }
    }
}