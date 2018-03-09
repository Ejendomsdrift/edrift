using GroupsContract.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using NLog;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupsContract.Models;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class WeekPlanService : IWeekPlanService
    {
        private readonly IJobProvider jobProvider;
        private readonly IMemberService memberService;
        private readonly IGroupService groupService;
        private readonly IDayAssignService dayAssignService;
        private readonly IJobStatusLogService jobStatusLogService;
        private readonly IPathHelper pathHelper;
        private readonly IManagementDepartmentService managementDepartmentService;
        private readonly IJobService jobService;
        private readonly IAppSettingHelper appSettingHelper;
        private readonly IJobStatusService jobStatusService;

        private static Logger logger;

        private const int WorkingDayCount = 5;

        public WeekPlanService(
            IJobProvider jobProvider,
            IMemberService memberService,
            IGroupService groupService,
            IDayAssignService dayAssignService,
            IJobStatusLogService jobStatusLogService,
            IPathHelper pathHelper,
            IManagementDepartmentService managementDepartmentService,
            IJobService jobService,
            IAppSettingHelper appSettingHelper,
            IJobStatusService jobStatusService)
        {
            this.jobProvider = jobProvider;
            this.memberService = memberService;
            this.groupService = groupService;
            this.dayAssignService = dayAssignService;
            this.jobStatusLogService = jobStatusLogService;
            this.pathHelper = pathHelper;
            this.managementDepartmentService = managementDepartmentService;
            this.jobService = jobService;
            this.appSettingHelper = appSettingHelper;
            this.jobStatusService = jobStatusService;

            logger = LogManager.GetLogger("MoveExpiriedJobsLog");
        }

        public IWeekJobsResultModel GetJobsForWeek(IWeekPlanFilterModel filter)
        {
            IEnumerable<IWeekPlanListViewModel> jobs = Enumerable.Empty<IWeekPlanListViewModel>();

            IWeekPlanGridModel model = GetWeekPlanGridModel(filter);

            if (filter.MemberIds.HasValue())
            {
                var allowedDayAssignIds = dayAssignService.GetDayAssignIds(filter.Map<ITaskDataFilterModel>()).ToList();
                jobs = GetListViewModel(model, filter.JobState)
                    .Where(x => x.DayAssignId.HasValue && allowedDayAssignIds.Contains(x.DayAssignId.Value));
            }
            else
            {
                jobs = GetListViewModel(model, filter.JobState);
            }

            List<IWeekPlanListViewModel> orderedJobList = jobs.OrderByDescending(x => x.AssignDate).ToList();

            filter.Week = filter.Week > default(int) ? --filter.Week : default(int);

            int previousNotEmptyWeekNumber = jobService.GetPreviousNotEmptyWeekNumber(filter);

            return new WeekJobsResultModel
            {
                Jobs = orderedJobList,
                IsAllowedPreviousWeeks = orderedJobList.Any() && previousNotEmptyWeekNumber > default(int),
                PreviousNotEmptyWeekNumber = previousNotEmptyWeekNumber
            };
        }

        private List<Job> GetWeekTasks(IWeekPlanFilterModel filter)
        {
            var result = new List<Job>();

            if (filter.HousingDepartmentId.HasValue)
            {
                result = jobProvider.GetByDepartmentIdYearWeek(filter.HousingDepartmentId.Value, filter.Year, filter.Week);
            }
            else
            {
                result = jobProvider.GetByYearWeekForAllDepartments(filter.Year, filter.Week);
            }

            return result;
        }

        private IEnumerable<IWeekPlanListViewModel> GetListViewModel(IWeekPlanGridModel weekPlanModel, JobStateType jobState)
        {
            IEnumerable<IWeekPlanJobModel> weekPlanJobList = weekPlanModel.BackLogJobs.Concat(weekPlanModel.WeekJobs);
            foreach (var job in weekPlanJobList)
            {
                //TODO Replace IsAllowedJobForCurrentTab
                if (IsAllowedJobForCurrentTab(job, jobState))
                {
                    yield return new WeekPlanListViewModel
                    {
                        Id = job.Id,
                        Title = job.Title,
                        StatusId = job.StatusId,
                        AssignDate = GetJobAssignDate(job),
                        DayAssignId = job.DayAssignId,
                        GroupName = job.GroupName,
                        DepartmentName = job.DepartmentName,
                        Users = job.Users,
                        JobType = job.JobType,
                        Address = job.Address,
                        TimeDate = job.DayAssignDate,
                        IsAssignedToAllUsers = job.IsAssignedToAllUsers,
                        SpentTime = job.SpentTime,
                        EstimatedTime = job.Estimate,
                        WeekDay = GetWeekDay(job),
                        WeekNumber = job.WeekNumber,
                        HasChangeStatusComment = job.IsCommentExistOnAnyStatus,
                        IsUrgent = job.IsUrgent
                    };
                }
            }
        }

        private bool IsAllowedJobForCurrentTab(IWeekPlanJobModel weekPlanJob, JobStateType jobState)
        {
            switch (jobState)
            {
                case JobStateType.Completed:
                    {
                        return weekPlanJob.StatusId == JobStatus.Completed;
                    }
                case JobStateType.NotCompleted:
                    {
                        int currentWeekNumber = DateTime.Now.GetWeekNumber();
                        int currentYear = DateTime.UtcNow.Year;

                        return weekPlanJob.StatusId.In(JobStatus.Canceled, JobStatus.Expired) ||
                               (weekPlanJob.StatusId != JobStatus.Completed &&
                                (weekPlanJob.Year < currentYear || (weekPlanJob.Year == currentYear && weekPlanJob.WeekNumber < currentWeekNumber)));
                    }
                default:
                    {
                        return weekPlanJob.StatusId != JobStatus.Completed &&
                               weekPlanJob.StatusId != JobStatus.Canceled &&
                               weekPlanJob.StatusId != JobStatus.Expired;
                    }
            }
        }

        private DateTime? GetJobAssignDate(IWeekPlanJobModel job)
        {
            if (job.IsVirtualTicket && !job.IsBackLogJob)
            {
                return CalendarHelper.GetDateByWeekAndDayNumber(job.Year, job.WeekNumber, job.WeekDay.Value);
            }
            if (!job.IsVirtualTicket && !job.IsBackLogJob && job.DayAssignDate.HasValue)
            {
                return job.DayAssignDate.Value;
            }

            return null;
        }

        public IWeekPlanGridModel GetWeekPlanGridModel(IWeekPlanFilterModel filter)
        {
            IWeekPlanGridModel result = new WeekPlanGridModel();
            List<IWeekPlanJobModel> allJobs = GetWeekPlanJobs(filter);
            result.BackLogJobs = allJobs.Where(j => j.IsBackLogJob);
            result.WeekJobs = allJobs.Where(j => !j.IsBackLogJob).OrderByDescending(j => j.StatusId.GetSortIndex());
            result.WeekendJobCount = allJobs.Count(j => j.IsWeekEndJob);
            result.IsShowWeekend = result.WeekendJobCount > 0;

            return result;
        }

        private List<IWeekPlanJobModel> GetWeekPlanJobs(IWeekPlanFilterModel filter)
        {
            IEnumerable<Job> jobs = GetWeekTasks(filter);
            return GetTicketsForJobList(jobs, filter.HousingDepartmentId, filter.Week, filter.Year, false);
        }

        private List<IWeekPlanJobModel> GetTicketsForJobList(IEnumerable<Job> jobs, Guid? departmentId, int weekNumber, int year, bool isAllowGetVirtualTickets)
        {
            List<IWeekPlanJobModel> result = new List<IWeekPlanJobModel>();

            var currentMember = memberService.GetCurrentUser();

            if (!currentMember.ActiveManagementDepartmentId.HasValue)
            {
                return result;
            }

            var departmentIds = new List<Guid>();
            var departmentList = new List<IHousingDepartmentModel>();

            var jobList = jobs.AsList();
            Guid deptId = departmentId ?? Guid.Empty;
            WeekPlanParamsModel query = GetWeekPlanParamsModel(jobList, deptId, weekNumber, year, isAllowGetVirtualTickets);

            if (departmentId.HasValue)
            {
                departmentIds.Add(departmentId.Value);
                departmentList = managementDepartmentService.GetHousingDepartments(currentMember.ActiveManagementDepartmentId.Value, departmentIds).ToList();
            }

            foreach (var job in jobList)
            {
                var assignsForDepartments = new Dictionary<Guid, JobAssign>();

                if (departmentId.HasValue)
                {
                    var assign = GetJobAssing(job, departmentId.Value);
                    assignsForDepartments.Add(departmentId.Value, assign);
                }
                else
                {
                    departmentIds = job.Assigns.SelectMany(x => x.HousingDepartmentIdList).Distinct().ToList();
                    departmentList = managementDepartmentService.GetHousingDepartments(currentMember.ActiveManagementDepartmentId.Value, departmentIds).ToList();

                    foreach (var department in departmentIds)
                    {
                        var jobAssign = GetJobAssing(job, department);
                        assignsForDepartments.Add(department, jobAssign);
                    }
                }

                foreach (var assignForDepartment in assignsForDepartments)
                {
                    if (assignForDepartment.Value == null)
                    {
                        continue;
                    }

                    var department = departmentList.FirstOrDefault(x => x.Id == assignForDepartment.Key);

                    if (department == null)
                    {
                        continue;
                    }

                    query.Job = job;
                    query.JobAssign = assignForDepartment.Value;
                    query.DepartmentId = department.Id;

                    List<IWeekPlanJobModel> backLogJobs = GetBackLogJobs(query, department.DisplayName);
                    result.AddRange(backLogJobs);

                    List<IWeekPlanJobModel> assignedToWeekDaysJobs = GetAssignedToWeekDaysJobs(query, department.DisplayName);
                    result.AddRange(assignedToWeekDaysJobs);
                }
            }

            return result;
        }

        private List<IWeekPlanJobModel> GetTicketsForExpiredJobs(IEnumerable<Job> jobs, Guid departmentId, int weekNumber, int year, bool isAllowGetVirtualTickets)
        {
            List<IWeekPlanJobModel> result = new List<IWeekPlanJobModel>();

            List<Job> jobList = jobs.AsList();
            WeekPlanParamsModel query = GetWeekPlanParamsModel(jobList, departmentId, weekNumber, year, isAllowGetVirtualTickets);

            foreach (var job in jobList)
            {
                JobAssign jobAssign = GetJobAssing(job, departmentId);

                if (jobAssign == null)
                {
                    continue;
                }

                query.Job = job;
                query.JobAssign = jobAssign;

                List<IWeekPlanJobModel> backLogJobs = GetBackLogJobs(query, string.Empty);
                result.AddRange(backLogJobs);

                List<IWeekPlanJobModel> assignedToWeekDaysJobs = GetAssignedToWeekDaysJobs(query, string.Empty);
                result.AddRange(assignedToWeekDaysJobs);
            }

            return result;
        }

        private WeekPlanParamsModel GetWeekPlanParamsModel(IEnumerable<Job> jobs, Guid departmentId, int weekNumber, int year, bool isAllowGetVirtualTickets)
        {
            var dayAssignIds = jobs.SelectMany(i => i.DayAssigns.Select(d => d.Id));
            var jobLogResultModel = jobStatusLogService.GetStatusLogModelList(dayAssignIds, false);

            return new WeekPlanParamsModel
            {
                DepartmentId = departmentId,
                WeekNumber = weekNumber,
                Year = year,
                IsAllowGetVirtualTickets = isAllowGetVirtualTickets,
                JobStatusLogs = jobLogResultModel
            };
        }

        private List<IWeekPlanJobModel> GetBackLogJobs(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> backLogJobsResult = new List<IWeekPlanJobModel>();

            var isAllowedCreationVirtualTasks = IsAllowedCreationVirtualTasks(query);
            if (isAllowedCreationVirtualTasks || query.IsAllowGetVirtualTickets)
            {
                List<IWeekPlanJobModel> virtualBackLogTickets = GetVirtualBackLogTickets(query, departmentName);
                backLogJobsResult.AddRange(virtualBackLogTickets);
            }

            List<IWeekPlanJobModel> realBackLogTickets = GetRealBackLogTickets(query, departmentName);
            backLogJobsResult.AddRange(realBackLogTickets);

            return backLogJobsResult;
        }

        private List<IWeekPlanJobModel> GetRealBackLogTickets(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> realBackLogTickets = new List<IWeekPlanJobModel>();

            List<IDayAssign> realBackLogAssignList = query.Job.DayAssigns.Where(da => da.WeekNumber == query.WeekNumber && !da.WeekDay.HasValue && da.Year == query.Year).ToList();
            IEnumerable<Guid> groupIds = realBackLogAssignList.Where(i => i.GroupId.HasValue).Select(i => i.GroupId.Value);
            List<IGroupModel> groupList = groupService.GetByIds(groupIds).ToList();

            foreach (var dayAssign in realBackLogAssignList)
            {
                IWeekPlanJobModel weekDayJob = MapJobsToWeekModel(query, true, departmentName, dayAssign);

                if (dayAssign.GroupId.HasValue)
                {
                    weekDayJob.GroupName = groupList.First(i => i.Id == dayAssign.GroupId).Name;
                }

                realBackLogTickets.Add(weekDayJob);
            }

            return realBackLogTickets;
        }

        private List<IWeekPlanJobModel> GetVirtualBackLogTickets(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> virtualBackLogTickets = new List<IWeekPlanJobModel>();

            if (!CanCreateVirtualTickets(query))
            {
                return virtualBackLogTickets;
            }

            // using this count we cut virtual tickets after we changed estimate or user on backlog ticket without set week day
            int backLogJobsCount = GetBacklogJobsCount(query);

            virtualBackLogTickets
                .AddRange(Enumerable.Range(1, backLogJobsCount)
                .Select(i => MapJobsToWeekModel(query, true, departmentName)));

            return virtualBackLogTickets;
        }

        private int GetBacklogJobsCount(WeekPlanParamsModel query)
        {
            int assignedToWeekDaysJobCount = query.JobAssign.DayPerWeekList.Count();
            int realBackLogAssignCount = query.Job.DayAssigns.Count(a => query.JobAssign.DayPerWeekList.All(i => i.Id != a.DayPerWeekId) &&
                                                                         a.WeekNumber == query.WeekNumber &&
                                                                         a.ExpiredDayAssignId == null);

            var backlogJobsCount = query.JobAssign.RepeatsPerWeek - assignedToWeekDaysJobCount - realBackLogAssignCount;
            return backlogJobsCount >= 0 ? backlogJobsCount : default(int);
        }

        private List<IWeekPlanJobModel> GetAssignedToWeekDaysJobs(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> assignedToWeekDaysJobsResult = new List<IWeekPlanJobModel>();

            var isAllowedCreationVirtualTasks = IsAllowedCreationVirtualTasks(query);
            if (isAllowedCreationVirtualTasks || query.IsAllowGetVirtualTickets)
            {
                List<IWeekPlanJobModel> virtualWeekAssignedTickets = GetVirtualWeekDayTickets(query, departmentName);
                assignedToWeekDaysJobsResult.AddRange(virtualWeekAssignedTickets);
            }

            List<IWeekPlanJobModel> realWeekAssignedTickets = GetRealWeekDayTickets(query, departmentName);
            assignedToWeekDaysJobsResult.AddRange(realWeekAssignedTickets);

            return assignedToWeekDaysJobsResult;
        }

        private List<IWeekPlanJobModel> GetVirtualWeekDayTickets(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> virtualTickets = new List<IWeekPlanJobModel>();

            if (!CanCreateVirtualTickets(query))
            {
                return virtualTickets;
            }

            IEnumerable<DayPerWeekModel> dayPerWeeksWithoutAlreadyAssigned =
                query.JobAssign.DayPerWeekList.Where(
                    d => !query.Job.DayAssigns.Any(da => da.DayPerWeekId == d.Id && da.WeekNumber == query.WeekNumber) && query.Job.JobTypeId == JobTypeEnum.Facility);

            foreach (var dayPerWeek in dayPerWeeksWithoutAlreadyAssigned)
            {
                IWeekPlanJobModel weekDayJob = MapJobsToWeekModel(query, false, departmentName);

                weekDayJob.IsWeekEndJob = dayPerWeek.WeekDay > WorkingDayCount;
                weekDayJob.WeekDay = dayPerWeek.WeekDay;
                weekDayJob.DayPerWeekId = dayPerWeek.Id;

                virtualTickets.Add(weekDayJob);
            }
            return virtualTickets;
        }

        private List<IWeekPlanJobModel> GetRealWeekDayTickets(WeekPlanParamsModel query, string departmentName)
        {
            List<IWeekPlanJobModel> realTickets = new List<IWeekPlanJobModel>();
            IEnumerable<Guid> groupIds = query.Job.DayAssigns.Where(i => i.GroupId.HasValue).Select(i => i.GroupId.Value);
            List<IGroupModel> groups = groupService.GetByIds(groupIds).ToList();

            IEnumerable<IDayAssign> dayAssignsWithoutVirtual = query.Job.DayAssigns?.Where(d => IsValidDayAssign(d, query.WeekNumber, query.Year));

            foreach (var dayAssign in dayAssignsWithoutVirtual)
            {
                IWeekPlanJobModel weekDayJob = MapJobsToWeekModel(query, false, departmentName, dayAssign);

                if (dayAssign.GroupId.HasValue)
                {
                    weekDayJob.GroupName = groups.First(i => i.Id == dayAssign.GroupId).Name;
                }

                realTickets.Add(weekDayJob);
            }

            return realTickets;
        }

        private WeekPlanJobModel MapJobsToWeekModel(WeekPlanParamsModel query, bool isBackLog, string departmentName, IDayAssign dayAssign = null)
        { 
           var taskDisplayColorDictionary = appSettingHelper.GetFromJson<IDictionary<JobTypeEnum, string>>(Constants.AppSetting.TaskTypesDisplayColors);
            WeekPlanJobModel resultModel = new WeekPlanJobModel
            {
                Id = query.Job.Id,
                JobAssignId = query.JobAssign.Id,
                IsBackLogJob = isBackLog,
                StatusId = dayAssign?.StatusId ?? JobStatus.Pending,
                Title = query.Job.Title,
                WeekNumber = query.WeekNumber,
                Category = query.Job.Category,
                TaskDisplayColor = query.Job.Category != null ? query.Job.Category.Color : taskDisplayColorDictionary[query.Job.JobTypeId],
                DepartmentName = departmentName,
                AllowedDays = Enumerable.Range(1, 7),
                JobType = query.Job.JobTypeId,
                JobTypeName = query.Job.JobTypeId.ToString(),
                TenantTypeName = dayAssign != null ? dayAssign.TenantType.ToString() : string.Empty,
                Address = query.DepartmentId.HasValue ? query.Job.GetAddress(query.DepartmentId.Value) : string.Empty,
                Year = query.Year,
                CreatorId = query.Job.CreatorId
            };

            if (dayAssign != null)
            {
                var jobStatusLog = query.JobStatusLogs.FirstOrDefault(i => i.DayAssignId == dayAssign.Id);
                resultModel.DayAssignId = dayAssign.Id;
                resultModel.Users = memberService.GetByIds(dayAssign.UserIdList);
                resultModel.GroupId = dayAssign.GroupId;
                resultModel.IsWeekEndJob = dayAssign.WeekDay.HasValue && dayAssign.WeekDay > WorkingDayCount;
                resultModel.DayAssignDate = dayAssign.Date;
                resultModel.WeekDay = dayAssign.WeekDay;
                resultModel.ExpiredDayAssignId = dayAssign.ExpiredDayAssignId;
                resultModel.ExpiredWeekNumber = dayAssign.ExpiredWeekNumber;
                resultModel.IsAssignedToAllUsers = dayAssign.IsAssignedToAllUsers;
                resultModel.TeamLeadId = dayAssign.TeamLeadId;
                resultModel.Estimate = dayAssign.EstimatedMinutes.MinutesToHours();
                resultModel.SpentTime = jobStatusLog?.TotalSpentTime;
                resultModel.ChangeStatusInfo = GetJobStatusInfo(jobStatusLog, dayAssign);
                resultModel.IsUrgent = dayAssign.IsUrgent;
                resultModel.IsCommentExistOnAnyStatus = query.JobStatusLogs.Where(x => x.DayAssignId == dayAssign.Id).Any(x => x.IsCommentExistInAnyStatus);
            }

            return resultModel;
        }

        public IChangeStatusInfo GetJobStatusInfo(IJobStatusLogModel jobStatusLog, IDayAssign dayAssign)
        {
            ChangeStatusInfo resultModel = null;

            if (IsValidLog(jobStatusLog, dayAssign))
            {
                resultModel = new ChangeStatusInfo
                {
                    ChangeStatusComment = jobStatusLog.Comment,
                    CancellationReason = jobStatusLog.CancelingReason,
                    ChangeStatusDate = jobStatusLog.Date,
                    UploadedFileList = GetUploadedFileList(dayAssign)
                };

                IMemberModel member = memberService.GetById(jobStatusLog.MemberId);

                if (member != null)
                {
                    resultModel.Avatar = member.Avatar;
                    resultModel.Name = member.Name;
                }
            }

            return resultModel;
        }

        private IEnumerable<IChangeStatusUploadedFile> GetUploadedFileList(IDayAssign dayAssign)
        {
            var result = new List<ChangeStatusUploadedFile>();
            foreach (var file in dayAssign.UploadList)
            {
                result.Add(new ChangeStatusUploadedFile
                {
                    Link = GetUploadedFileLink(file, dayAssign.Id),
                    FileName = file.FileName
                });
            }

            return result;
        }

        public string GetUploadedFileLink(UploadFileModel model, Guid dayAssignId)
        {
            if (model != null)
            {
                var extension = Path.GetExtension(model.Path);
                return pathHelper.GetDayAssignUploadsPath(dayAssignId, model.FileId, extension);
            }

            return string.Empty;
        }

        private bool IsAllowedCreationVirtualTasks(WeekPlanParamsModel query)
        {
            var isValidNextYear = query.Year > DateTime.Now.Year && (query.JobAssign.TillYear == 0 || query.Year <= query.JobAssign.TillYear);
            var isValidWeek = query.Year == DateTime.Now.Year && query.WeekNumber >= DateTime.Now.GetWeekNumber();

            return query.Job.JobTypeId == JobTypeEnum.Facility &&
                   (!query.DepartmentId.HasValue || query.JobAssign.HousingDepartmentIdList.Contains(query.DepartmentId.Value)) &&
                   query.JobAssign.WeekList.Any(i => !i.IsDisabled && i.Number == query.WeekNumber) &&
                   (isValidNextYear || isValidWeek);
        }

        private bool IsValidDayAssign(IDayAssign dayAssign, int weekNumber, int year)
        {
            var isValidYear = !dayAssign.Date.HasValue || dayAssign.Date.Value.Year == year;
            return dayAssign.WeekNumber == weekNumber && dayAssign.WeekDay != null && isValidYear;
        }

        //--------------------------------------------------------------------------------------------------------------------

        public async Task MoveExpiredJobs(bool checkWeek)
        {
            int weekNumber = CalendarHelper.GetWeekNumber(DateTime.UtcNow);

            logger.Info($"Enter MoveExpiredJobs in {DateTime.UtcNow} with parametrs:\n\t weekNumber {weekNumber}; \n\t checkWeek {checkWeek} \n");

            int year = DateTime.UtcNow.Year;
            IEnumerable<IHousingDepartmentModel> housingDepartments = managementDepartmentService.GetAllHousingDepartments();

            foreach (var department in housingDepartments)
            {
                await MoveExpiredJobsForDepartment(department.Id, weekNumber, year, checkWeek);
            }

            logger.Info($"Leave MoveExpiriedJobs in {DateTime.UtcNow} \n\n");
        }

        private async Task MoveExpiredJobsForDepartment(Guid departmentId, int weekNumber, int year, bool checkWeek)
        {
            if (!checkWeek || IsDateCurrentOrPrevious(year, weekNumber))
            {
                var filter = new WeekPlanFilterModel
                {
                    HousingDepartmentId = departmentId,
                    Week = weekNumber,
                    Year = year
                };

                IEnumerable<IWeekPlanJobModel> prevWeekExpiredJobs = GetExpiredJobsForPreviousWeek(filter);
                IEnumerable<IWeekPlanJobModel> alreadyMovedExpiredJobs = GetAlreadyMovedExpiredJobsForSelectedWeek(filter);

                var notMovedTickets = prevWeekExpiredJobs.Where(pj => alreadyMovedExpiredJobs.All(mj => mj.ExpiredDayAssignId != pj.DayAssignId));

                await MoveTicketsToBacklogAndCreateRealForPreviousWeek(notMovedTickets, departmentId, weekNumber);
            }
        }

        private async Task MoveTicketsToBacklogAndCreateRealForPreviousWeek(IEnumerable<IWeekPlanJobModel> notMovedTickets, Guid departmentId, int weekNumber)
        {
            foreach (IWeekPlanJobModel ticket in notMovedTickets)
            {
                if (ticket.IsVirtualTicket)
                {
                    Guid dayAssignId = await CreateDayAssignForPreviousWeek(ticket, departmentId, weekNumber, ticket.CreatorId);
                    await jobStatusService.Expired(dayAssignId, JobStatus.Pending, ticket.CreatorId);
                    await CreateExpiredMovedDayAssignForBackLog(dayAssignId, ticket, departmentId, weekNumber, ticket.CreatorId);
                }
                else
                {
                    IDayAssign dayAssign = dayAssignService.GetDayAssignById(ticket.DayAssignId.Value);
                    await jobStatusService.Expired(ticket.DayAssignId.Value, dayAssign.StatusId, ticket.CreatorId);
                    await CreateExpiredMovedDayAssignForBackLog(ticket.DayAssignId.Value, ticket, departmentId, weekNumber, ticket.CreatorId);
                }
            }
        }

        private async Task<Guid> CreateDayAssignForPreviousWeek(IWeekPlanJobModel ticket, Guid departmentId, int weekNumber, Guid? creatorId)
        {
            int previousWeek = GetPreviousWeekNumber(weekNumber);
            INewDayAssignModel model = GetNewDayAssignForExpiredTasks(ticket, departmentId, previousWeek, creatorId);
            Guid dayAssignId = await dayAssignService.CreateDayAssign(model);
            return dayAssignId;
        }

        private async Task CreateExpiredMovedDayAssignForBackLog(Guid dayAssignId, IWeekPlanJobModel ticket, Guid departmentId, int weekNumber, Guid? creatorId)
        {
            INewDayAssignModel model = GetNewDayAssignForExpiredTasks(ticket, departmentId, weekNumber, creatorId);
            model.ExpiredDayAssignId = dayAssignId;
            await dayAssignService.CreateDayAssign(model);
        }

        private NewDayAssignModel GetNewDayAssignForExpiredTasks(IWeekPlanJobModel ticket, Guid departmentId, int currentWeekNumber, Guid? creatorId)
        {
            var result = new NewDayAssignModel
            {
                JobId = ticket.Id,
                DepartmentId = departmentId,
                GroupId = null,
                GroupName = string.Empty,
                UserIdList = new List<Guid>(),
                WeekNumber = currentWeekNumber,
                CurrentWeekDay = ticket.WeekDay,
                IsAssignedToAllUsers = false,
                TeamLeadId = null,
                ExpiredDayAssignId = ticket.DayAssignId,
                ExpiredWeekNumber = ticket.ExpiredDayAssignId != null ? ticket.ExpiredWeekNumber : ticket.WeekNumber,
                JobAssignId = ticket.JobAssignId,
                DayPerWeekId = ticket.DayPerWeekId,
                EstimatedMinutes = ticket.Estimate?.HoursToMinutes() ?? 0,
                CreatorId = creatorId
            };

            if (ticket.DayAssignId.HasValue)
            {
                var dayAssign = dayAssignService.GetDayAssignById(ticket.DayAssignId.Value);
                result.Comment = dayAssign.Comment;
                result.ResidentName = dayAssign.ResidentName;
                result.ResidentPhone = dayAssign.ResidentPhone;
                result.TenantType = dayAssign.TenantType;
                result.UploadList = dayAssign.UploadList;
                result.EstimatedMinutes = dayAssign.EstimatedMinutes ?? 0;
            }

            return result;
        }

        private IEnumerable<IWeekPlanJobModel> GetAlreadyMovedExpiredJobsForSelectedWeek(IWeekPlanFilterModel filter)
        {
            IEnumerable<Job> jobs = GetWeekTasks(filter);
            List<IWeekPlanJobModel> ticketsExceptVirtual = GetTicketsForExpiredJobs(jobs, filter.HousingDepartmentId.Value, filter.Week, filter.Year, false);
            return ticketsExceptVirtual.Where(t => t.ExpiredDayAssignId.HasValue);
        }

        private IEnumerable<IWeekPlanJobModel> GetExpiredJobsForPreviousWeek(IWeekPlanFilterModel filter)
        {
            IEnumerable<JobStatus> allowedStatusesForMovingInExpiredStatus = appSettingHelper.GetFromJson<IEnumerable<JobStatus>>(Constants.AppSetting.AllowedStatusesForMovingInExpiredStatus);
            int previousWeekNumber = GetPreviousWeekNumber(filter.Week);
            int previousWeekYear = GetPreviousWeekYear(filter.Week, filter.Year);

            var previousWeekFilter = filter.Clone();
            previousWeekFilter.Week = previousWeekNumber;
            previousWeekFilter.Year = previousWeekYear;

            IEnumerable<Job> jobs = GetWeekTasks(previousWeekFilter);
            IEnumerable<IWeekPlanJobModel> allTicketsFromPreviousWeek = GetTicketsForExpiredJobs(jobs, filter.HousingDepartmentId.Value, previousWeekNumber, previousWeekYear, true);

            return allTicketsFromPreviousWeek.Where(j => allowedStatusesForMovingInExpiredStatus.Contains(j.StatusId));
        }

        public bool IsDateCurrentOrPrevious(int year, int weekNumber)
        {
            int currentWeekNumber = DateTime.Now.GetWeekNumber();
            int currentYear = DateTime.UtcNow.Year;

            return year < currentYear || (year == currentYear && weekNumber <= currentWeekNumber);
        }

        private int GetPreviousWeekYear(int currentWeekNumber, int currentYear)
        {
            return currentWeekNumber == 1 ? currentYear - 1 : currentYear;
        }

        private int GetPreviousWeekNumber(int currentWeekNumber)
        {
            int weeksInYear = Constants.DateTime.WeeksInYear;
            return currentWeekNumber == 1 ? weeksInYear : currentWeekNumber - 1;
        }

        private bool CanCreateVirtualTickets(WeekPlanParamsModel query)
        {
            var createdJobWeek = query.Job.CreationDate.GetWeekNumber();
            var isJobExisted = query.Year > query.Job.CreationDate.Year || (query.Year == query.Job.CreationDate.Year && query.WeekNumber >= createdJobWeek);

            return (!query.DepartmentId.HasValue || query.JobAssign.HousingDepartmentIdList.Contains(query.DepartmentId.Value)) && query.JobAssign.IsEnabled &&
                   query.JobAssign.WeekList.Any(i => i.Number == query.WeekNumber) &&
                   isJobExisted;
        }

        private int? GetWeekDay(IWeekPlanJobModel job)
        {
            if (job.IsVirtualTicket && !job.IsBackLogJob && job.WeekDay.HasValue)
            {
                return job.WeekDay.Value;
            }
            if (!job.IsVirtualTicket && !job.IsBackLogJob && job.DayAssignDate.HasValue)
            {
                return job.DayAssignDate.Value.GetWeekDayNumber();
            }

            return null;
        }

        private JobAssign GetJobAssing(Job job, Guid departmentId)
        {
            JobAssign jobAssign = job.Assigns.FirstOrDefault(a => a.HousingDepartmentIdList.Contains(departmentId));

            if (jobAssign == null && job.DayAssigns.HasValue())
            {
                jobAssign = job.Assigns.OrderBy(i => i.IsGlobal).FirstOrDefault(a => a.JobIdList.Contains(job.Id));
            }

            return jobAssign;
        }

        private bool IsValidLog(IJobStatusLogModel jobStatusLog, IDayAssign dayAssign)
        {
            bool result = jobStatusLog != null
                          && (!string.IsNullOrEmpty(jobStatusLog.Comment) || !string.IsNullOrEmpty(jobStatusLog.CancelingReason) || dayAssign.UploadList != null && dayAssign.UploadList.Count > 0);

            return result;
        }
    }
}