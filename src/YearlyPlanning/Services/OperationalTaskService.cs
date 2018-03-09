using CategoryCore.Contract.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using Infrastructure.Messaging;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Commands.JobCommands;
using YearlyPlanning.Contract.Commands.OperationalTaskCommands;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class OperationalTaskService : IOperationalTaskService
    {
        private readonly IMessageBus messageBus;
        private readonly ITaskIdGenerator taskIdGenerator;
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IDayAssignService dayAssignService;
        private readonly IJobProvider jobProvider;
        private readonly ICategoryService categoryService;
        private readonly IMemberService memberService;
        private readonly IPathHelper pathHelper;
        private readonly IJobStatusService jobStatusService;
        private readonly IJobService jobService;

        private const int DefaultRepeatsPerWeekConst = 1;

        public OperationalTaskService(
            IMessageBus messageBus,
            ITaskIdGenerator taskIdGenerator,
            IJobAssignProvider jobAssignProvider,
            IDayAssignService dayAssignService,
            IJobProvider jobProvider,
            ICategoryService categoryService,
            IPathHelper pathHelper,
            IMemberService memberService,
            IJobStatusService jobStatusService,
            IJobService jobService)
        {
            this.messageBus = messageBus;
            this.taskIdGenerator = taskIdGenerator;
            this.jobAssignProvider = jobAssignProvider;
            this.dayAssignService = dayAssignService;
            this.jobProvider = jobProvider;
            this.categoryService = categoryService;
            this.pathHelper = pathHelper;
            this.memberService = memberService;
            this.jobStatusService = jobStatusService;
            this.jobService = jobService;
        }

        public Task<IOperationalTaskModel> GetTask(Guid dayAssignId, JobTypeEnum type)
        {
            switch (type)
            {
                case JobTypeEnum.AdHock:
                    return GetAdHocTask(dayAssignId);
                case JobTypeEnum.Tenant:
                    return GetTenantTask(dayAssignId);
                case JobTypeEnum.Other:
                    return GetOtherTask(dayAssignId);
                default:
                    throw new Exception("GetTask method doesn't have implementation for type: " + type);
            }
        }

        public async Task<CreateOperationalTaskViewModel> CreateAdHocTask(IOperationalTaskModel model)
        {
            model.JobId = await taskIdGenerator.AdHoc();
            var addressList = new List<JobAddress>();
            addressList.Add(new JobAddress { HousingDepartmentId = model.DepartmentId, Address = model.Address });
            await messageBus.Publish(new CreateJobCommand(model.JobId, model.CategoryId, model.Title, JobTypeEnum.AdHock, model.CreatorId, RoleType.Coordinator, addressList, null, null));

            var jobAssignId = Guid.NewGuid();
            CreateOperationalTaskAssignCommand adHockAssignCommand = GetCreateOperationalTaskAssignCommand(model, model.JobId, jobAssignId);
            await messageBus.Publish(adHockAssignCommand);

            var dayAssignId = Guid.NewGuid();
            CreateDayAssignCommand dayAssign = GetCreateDayAssignCommand(model, dayAssignId, jobAssignId);
            await messageBus.Publish(dayAssign);

            await SetStatusAfterCreation(dayAssignId, model.TeamLeadId);

            CreateOperationalTaskViewModel result = new CreateOperationalTaskViewModel
            {
                Id = model.JobId,
                DayAssignId = dayAssignId,
                DepartmentId = model.DepartmentId
            };

            return result;
        }

        public async Task<CreateOperationalTaskViewModel> CreateTenantTask(IOperationalTaskModel model)
        {
            int weekNumber = CalendarHelper.GetWeekNumber(model.Date.Value);

            model.JobId = await taskIdGenerator.Tenant();
            var addressList = new List<JobAddress>();
            addressList.Add(new JobAddress { HousingDepartmentId = model.DepartmentId, Address = model.Address });
            await messageBus.Publish(new CreateJobCommand(model.JobId, Guid.Empty, model.Title, JobTypeEnum.Tenant, model.CreatorId, RoleType.Coordinator, addressList, null, null));

            var jobAssignId = Guid.NewGuid();
            CreateOperationalTaskAssignCommand tenantkAssignCommand = GetCreateOperationalTaskAssignCommand(model, model.JobId, jobAssignId, weekNumber);
            await messageBus.Publish(tenantkAssignCommand);

            var dayAssignId = Guid.NewGuid();
            CreateDayAssignCommand dayAssign = GetCreateDayAssignCommand(model, dayAssignId, jobAssignId, weekNumber);
            await messageBus.Publish(dayAssign);

            await SetStatusAfterCreation(dayAssignId, model.TeamLeadId);

            CreateOperationalTaskViewModel result = new CreateOperationalTaskViewModel
            {
                Id = model.JobId,
                DepartmentId = model.DepartmentId,
                DayAssignId = dayAssignId
            };

            return result;
        }

        public async Task<CreateOperationalTaskViewModel> CreateOtherTask(IOperationalTaskModel model)
        {
            int weekNumber = CalendarHelper.GetWeekNumber(model.Date.Value);

            model.JobId = await taskIdGenerator.Other();
            var addressList = new List<JobAddress>();
            addressList.Add(new JobAddress { HousingDepartmentId = model.DepartmentId, Address = model.Address });
            await messageBus.Publish(new CreateJobCommand(model.JobId, Guid.Empty, model.Title, JobTypeEnum.Other, model.CreatorId, RoleType.Coordinator, addressList, null, null));

            var jobAssignId = Guid.NewGuid();
            CreateOperationalTaskAssignCommand tenantkAssignCommand = GetCreateOperationalTaskAssignCommand(model, model.JobId, jobAssignId, weekNumber);
            await messageBus.Publish(tenantkAssignCommand);

            var dayAssignId = Guid.NewGuid();
            CreateDayAssignCommand dayAssign = GetCreateDayAssignCommand(model, dayAssignId, jobAssignId, weekNumber);
            await messageBus.Publish(dayAssign);

            await SetStatusAfterCreation(dayAssignId, model.TeamLeadId);

            CreateOperationalTaskViewModel result = new CreateOperationalTaskViewModel
            {
                Id = model.JobId,
                DepartmentId = model.DepartmentId,
                DayAssignId = dayAssignId
            };

            return result;
        }

        public async Task ChangeTenantTaskType(Guid dayAssignId, TenantTaskTypeEnum type)
        {
            await messageBus.Publish(new ChangeTenantTaskTypeCommand(dayAssignId, type));
        }

        public async Task ChangeTenantTaskUrgency(Guid dayAssignId, bool isUrgent)
        {
            await messageBus.Publish(new ChangeTenantTaskUrgencyCommand(dayAssignId, isUrgent));
        }

        public async Task ChangeTaskDate(string jobId, Guid dayAssignId, DateTime date)
        {
            JobAssign jobAssign = jobAssignProvider.GetByJobId(jobId);
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);
            WeekModel weekModel = jobAssign.WeekList.First();
            int weekDay = date.GetWeekDayNumber();
            int weekNumber = date.GetWeekNumber();
            var dayPerWeekList = new List<DayPerWeekModel> { new DayPerWeekModel { Id = Guid.NewGuid(), WeekDay = weekDay } };
            var weekList = new List<WeekModel> { new WeekModel { Number = weekNumber, IsDisabled = false, ChangedBy = weekModel.ChangedBy } };

            await messageBus.Publish(new ChangeJobAssignWeeksCommand(jobAssign.Id, weekList, (ChangedByRole)weekModel.ChangedBy, false));
            await messageBus.Publish(new SaveDaysPerWeekCommand(jobAssign.Id, dayPerWeekList));
            await messageBus.Publish(new ChangeOperationalTaskDateCommand(dayAssignId, date, weekDay));

            if (dayAssign.StatusId == JobStatus.Expired)
            {
                await jobStatusService.Pending(dayAssign.Id, dayAssign.StatusId);
            }
        }

        public async Task ChangeTaskTime(Guid dayAssignId, DateTime time)
        {
            await messageBus.Publish(new ChangeOperationalTaskTimeCommand(dayAssignId, time));
        }

        public async Task ChangeCategory(IdValueModel<string, Guid> model)
        {
            await messageBus.Publish(new ChangeJobCategoryCommand(model.Id, model.Value));
        }

        public async Task ChangeEstimate(IdValueModel<Guid, int> model)
        {
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(model.Id);

            var estimated = new ChangeDayAssignEstimatedMinutesCommand(dayAssign.Id.ToString());
            estimated.EstimatedMinutes = model.Value;
            await messageBus.Publish(estimated);
        }

        public async Task ChangeTitle(IdValueModel<string, string> model)
        {
            await messageBus.Publish(new ChangeJobTitleCommand(model.Id, model.Value));
        }

        public async Task ChangeResidentName(IdValueModel<Guid, string> model)
        {
            await messageBus.Publish(new ChangeTenantTaskResidentNameCommand(model.Id, model.Value));
        }

        public async Task ChangeResidentPhone(IdValueModel<Guid, string> model)
        {
            await messageBus.Publish(new ChangeTenantTaskResidentPhoneCommand(model.Id, model.Value));
        }

        public async Task ChangeTenantTaskComment(IdValueModel<Guid, string> model)
        {
            await messageBus.Publish(new ChangeTenantTaskCommentCommand(model.Id, model.Value));
        }

        public async Task ChangeDescription(IdValueModel<string, string> model)
        {
            JobAssign jobAssign = GetJobAssignByJobId(model.Id);
            await messageBus.Publish(new ChangeJobAssignDescriptionCommand(jobAssign.Id, model.Value));
        }

        public async Task ChangeTenantTaskDescription(Guid jobAssignId, string description)
        {
            await messageBus.Publish(new ChangeJobAssignDescriptionCommand(jobAssignId, description));
        }

        public async Task AssignEmployees(MemberAssignModel model)
        {
            IDayAssign dayAssign = GetDayAssign(model.DayAssignId);
            var currentUser = memberService.GetCurrentUser();

            //Janitor can create task only for himself
            if (currentUser.CurrentRole == RoleType.Janitor)
            {
                model.UserIdList = new List<Guid> { currentUser.MemberId };
                model.TeamLeadId = currentUser.MemberId;
                model.GroupId = null;
                model.IsAssignedToAllUsers = false;
            }

            var memberModel = new ChangeDayAssignMembersComand(dayAssign.Id.ToString())
            {
                UserIdList = model.UserIdList,
                GroupId = model.GroupId,
                TeamLeadId = model.TeamLeadId,
                IsAssignedToAllUsers = model.IsAssignedToAllUsers
            };

            await messageBus.Publish(memberModel);

            if (model.IsUnassignAll)
            {
                await jobStatusService.Pending(dayAssign.Id, dayAssign.StatusId);
            }
            else if (dayAssign.StatusId != JobStatus.Opened && !model.TeamLeadId.HasValue)
            {
                await jobStatusService.Opened(dayAssign.Id, dayAssign.StatusId);
            }
            else if (dayAssign.StatusId != JobStatus.Assigned && model.TeamLeadId.HasValue)
            {
                await jobStatusService.Assigned(dayAssign.Id, dayAssign.StatusId);
            }
        }

        public async Task UnassignEmployees(MemberAssignModel model)
        {
            IDayAssign dayAssign = GetDayAssign(model.DayAssignId);

            List<Guid> userIdList = model.UserIdList;
            await messageBus.Publish(new RemoveDayAssignMembersCommand(dayAssign.Id.ToString(), userIdList));
        }

        public async Task<List<IOperationalTaskModel>> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber)
        {
            List<IOperationalTaskModel> viewModelList = new List<IOperationalTaskModel>();

            List<JobAssign> jobAssigns = jobAssignProvider.GetByDepartmentWeekAndYear(departmentId, year, weekNumber);

            foreach (var jobAssign in jobAssigns)
            {
                foreach (var jobId in jobAssign.JobIdList)
                {
                    IOperationalTaskModel viewModel = new OperationalTaskModel();

                    FillJobAssignData(viewModel, jobAssign, departmentId, JobTypeEnum.AdHock);

                    List<IDayAssign> dayAssignList = dayAssignService.GetDayAssigns(jobAssign.Id, jobId, departmentId, weekNumber);
                    IDayAssign dayAssign = dayAssignList.FirstOrDefault(); // adhock tasks have only one day assign
                    FillDayAssignData(viewModel, dayAssign, JobTypeEnum.AdHock);

                    Job job = await jobProvider.Get(jobId);
                    FillJobData(viewModel, job, departmentId, JobTypeEnum.AdHock);

                    FillCategory(viewModel);

                    viewModelList.Add(viewModel);
                }
            }

            return viewModelList;
        }

        public async Task<ITaskCreationInfo> GetTaskCreationInfo(string jobId)
        {
            var job = await jobProvider.Get(jobId);
            var dayAssign = await dayAssignService.GetByJobId(jobId);
            var creator = memberService.GetById(job.CreatorId);
            var result = new TaskCreationInfo
            {
                CreatorAvatar = creator.Avatar,
                CreatorName = creator.Name,
                CreationDate = job.CreationDate,
                JobTypeId = job.JobTypeId,
                IsTaskCanceled = dayAssign != null && dayAssign.StatusId == JobStatus.Canceled,
                IsUrgent = dayAssign?.IsUrgent,
                Title = job.Title
            };

            return result;
        }

        public IEnumerable<UploadFileModel> GetUploads(Guid jobAssignId)
        {
            JobAssign jobAssign = jobAssignProvider.GetById(jobAssignId);
            jobService.FillCorrectPathForJobAssignsUploads(new List<JobAssign> { jobAssign });

            return jobAssign.UploadList;
        }

        private CreateOperationalTaskAssignCommand GetCreateOperationalTaskAssignCommand(IOperationalTaskModel model, string jobId, Guid assignId)
        {
            var weekNumber = CalendarHelper.GetWeekNumber(model.Date.Value);
            var result = new CreateOperationalTaskAssignCommand(assignId, new List<string> { jobId }, RoleType.Coordinator, weekNumber)
            {
                DepartmentId = model.DepartmentId,
                RepeatsPerWeek = DefaultRepeatsPerWeekConst,
                Description = model.Description,
                IsEnabled = true
            };

            return result;
        }

        private CreateOperationalTaskAssignCommand GetCreateOperationalTaskAssignCommand(IOperationalTaskModel model, string jobId, Guid assignId, int weekNumber)
        {
            var result = new CreateOperationalTaskAssignCommand(assignId, new List<string> { jobId }, RoleType.Coordinator, weekNumber)
            {
                DepartmentId = model.DepartmentId,
                RepeatsPerWeek = DefaultRepeatsPerWeekConst,
                Description = model.Description,
                IsEnabled = true
            };

            return result;
        }

        private IDayAssign GetDayAssign(Guid dayAssignId)
        {
            return dayAssignService.GetDayAssignById(dayAssignId);
        }

        private JobAssign GetJobAssignByJobId(string jobId)
        {
            List<JobAssign> jobAssignList = jobAssignProvider.GetAllByJobId(jobId);
            JobAssign jobAssign = jobAssignList.FirstOrDefault();

            return jobAssign;
        }

        private void FillCategory(IOperationalTaskModel adHocTask)
        {
            var category = categoryService.Get(adHocTask.CategoryId);
            adHocTask.Category = category;
        }

        private CreateDayAssignCommand GetCreateDayAssignCommand(IOperationalTaskModel model, Guid assignId, Guid jobAssignId)
        {
            var dayOfWeek = model.Date.Value.GetWeekDayNumber();
            var result = new CreateDayAssignCommand(assignId, model.JobId, jobAssignId, model.Date)
            {
                EstimatedMinutes = (int?)model.Estimate,
                DepartmentId = model.DepartmentId,
                GroupId = model.GroupId,
                UserIdList = model.UserIdList.ToList(),
                Address = model.Address,
                StatusId = JobStatus.Pending,
                WeekNumber = CalendarHelper.GetWeekNumber(model.Date.Value),
                WeekDay = dayOfWeek,
                TeamLeadId = model.TeamLeadId,
                IsAssignedToAllUsers = model.IsAssignedToAllUsers
            };

            return result;
        }

        private CreateDayAssignCommand GetCreateDayAssignCommand(IOperationalTaskModel model, Guid assignId, Guid jobAssignId, int weekNumber)
        {
            DateTime assignedDate = GetAssignedDate(model.Date.Value, model.Time.Value);
            var dayOfWeek = assignedDate.GetWeekDayNumber();

            var result = new CreateDayAssignCommand(assignId, model.JobId, jobAssignId, assignedDate)
            {
                EstimatedMinutes = (int?)model.Estimate,
                DepartmentId = model.DepartmentId,
                GroupId = model.GroupId,
                UserIdList = model.UserIdList.ToList(),
                Address = model.Address,
                StatusId = JobStatus.Pending,
                WeekNumber = weekNumber,
                TeamLeadId = model.TeamLeadId,
                IsAssignedToAllUsers = model.IsAssignedToAllUsers,
                WeekDay = dayOfWeek,
                Comment = model.Comment,
                ResidentName = model.ResidentName,
                ResidentPhone = model.ResidentPhone,
                Type = model.Type,
                IsUrgent = model.IsUrgent
            };

            return result;
        }

        private void FillDayAssignData(IOperationalTaskModel viewModel, IDayAssign dayAssign, JobTypeEnum jobType)
        {
            viewModel.StatusId = dayAssign.StatusId;
            switch (jobType)
            {
                case JobTypeEnum.AdHock:
                    FillAdHocDayAssignData(viewModel, dayAssign);
                    break;
                case JobTypeEnum.Tenant:
                    FillTenantDayAssignData(viewModel, dayAssign);
                    break;
                case JobTypeEnum.Other:
                    FillOtherTaskDayAssignData(viewModel, dayAssign);
                    break;
                default:
                    throw new NotImplementedException($"No such task type {jobType}");
            }
        }

        private void FillAdHocDayAssignData(IOperationalTaskModel viewModel, IDayAssign dayAssign)
        {
            viewModel.DayAssignId = dayAssign.Id;
            viewModel.Estimate = (decimal)dayAssign.EstimatedMinutes;
            viewModel.GroupId = dayAssign.GroupId;
            viewModel.TeamLeadId = dayAssign.TeamLeadId;
            viewModel.IsAssignedToAllUsers = dayAssign.IsAssignedToAllUsers;
            viewModel.UserIdList = dayAssign.UserIdList;
            viewModel.StatusId = dayAssign.StatusId;
            viewModel.Date = dayAssign.Date;
        }

        private void FillOtherTaskDayAssignData(IOperationalTaskModel viewModel, IDayAssign dayAssign)
        {
            viewModel.DayAssignId = dayAssign.Id;
            viewModel.GroupId = dayAssign.GroupId;
            viewModel.TeamLeadId = dayAssign.TeamLeadId;
            viewModel.IsAssignedToAllUsers = dayAssign.IsAssignedToAllUsers;
            viewModel.UserIdList = dayAssign.UserIdList;
            viewModel.Date = !dayAssign.Date.HasValue ? null : (DateTime?)new DateTime(dayAssign.Date.Value.Year, dayAssign.Date.Value.Month, dayAssign.Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            viewModel.Time = !dayAssign.Date.HasValue ? null : (DateTime?)new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                dayAssign.Date.Value.Hour, dayAssign.Date.Value.Minute, dayAssign.Date.Value.Second, DateTimeKind.Utc);
            viewModel.Estimate = (decimal)dayAssign.EstimatedMinutes;
        }

        private void FillTenantDayAssignData(IOperationalTaskModel viewModel, IDayAssign dayAssign)
        {
            viewModel.DayAssignId = dayAssign.Id;
            viewModel.GroupId = dayAssign.GroupId;
            viewModel.TeamLeadId = dayAssign.TeamLeadId;
            viewModel.IsAssignedToAllUsers = dayAssign.IsAssignedToAllUsers;
            viewModel.UserIdList = dayAssign.UserIdList;
            viewModel.Comment = dayAssign.Comment;
            viewModel.ResidentName = dayAssign.ResidentName;
            viewModel.ResidentPhone = dayAssign.ResidentPhone;
            viewModel.Type = dayAssign.TenantType;
            viewModel.IsUrgent = dayAssign.IsUrgent;
            viewModel.Date = !dayAssign.Date.HasValue ? null : (DateTime?)new DateTime(dayAssign.Date.Value.Year, dayAssign.Date.Value.Month, dayAssign.Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            viewModel.Time = !dayAssign.Date.HasValue ? null : (DateTime?)new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                dayAssign.Date.Value.Hour, dayAssign.Date.Value.Minute, dayAssign.Date.Value.Second, DateTimeKind.Utc);
            viewModel.Estimate = (decimal)dayAssign.EstimatedMinutes;
        }

        private void FillJobAssignData(IOperationalTaskModel viewModel, JobAssign jobAssign, Guid departmentId, JobTypeEnum jobType)
        {
            switch (jobType)
            {
                case JobTypeEnum.AdHock:
                    FillAdhocJobAssignData(viewModel, jobAssign, departmentId);
                    break;
                case JobTypeEnum.Tenant:
                    FillTenantJobAssignData(viewModel, jobAssign, departmentId);
                    break;
                case JobTypeEnum.Other:
                    FillOtherTaskJobAssignDate(viewModel, jobAssign, departmentId);
                    break;
                default:
                    throw new NotImplementedException($"No such task type {jobType}");
            }
        }

        private void FillAdhocJobAssignData(IOperationalTaskModel viewModel, JobAssign jobAssign, Guid departmentId)
        {
            viewModel.JobAssignId = jobAssign.Id;
            viewModel.DepartmentId = departmentId;
            viewModel.Description = jobAssign.Description;
            viewModel.Uploads = jobAssign.UploadList;
            SetCorrectUrlForUploadList(viewModel.Uploads, jobAssign.Id);
        }

        private void FillOtherTaskJobAssignDate(IOperationalTaskModel viewModel, JobAssign jobAssign, Guid departmentId)
        {
            FillTenantJobAssignData(viewModel, jobAssign, departmentId);
        }

        private void FillTenantJobAssignData(IOperationalTaskModel viewModel, JobAssign jobAssign, Guid departmentId)
        {
            viewModel.JobAssignId = jobAssign.Id;
            viewModel.DepartmentId = departmentId;
            viewModel.Description = jobAssign.Description;
            viewModel.Uploads = jobAssign.UploadList;
            SetCorrectUrlForUploadList(viewModel.Uploads, jobAssign.Id);
        }

        private void FillJobData(IOperationalTaskModel viewModel, Job job, Guid departmentId, JobTypeEnum jobType)
        {
            switch (jobType)
            {
                case JobTypeEnum.AdHock:
                    FillAdHocJobData(viewModel, job, departmentId);
                    break;
                case JobTypeEnum.Tenant:
                    FillTenantJobData(viewModel, job, departmentId);
                    break;
                case JobTypeEnum.Other:
                    FillOtherTaskJobData(viewModel, job, departmentId);
                    break;
                default:
                    throw new NotImplementedException($"No such task type {jobType}");
            }
        }

        private void FillAdHocJobData(IOperationalTaskModel viewModel, Job job, Guid departmentId)
        {
            viewModel.Id = job.Id;
            viewModel.Category = job.Category;  // should be filled
            viewModel.CategoryId = job.CategoryId;
            viewModel.Title = job.Title;
            viewModel.Address = job.GetAddress(departmentId);
        }

        private void FillOtherTaskJobData(IOperationalTaskModel viewModel, Job job, Guid departmentId)
        {
            FillTenantJobData(viewModel, job, departmentId);
        }

        private void FillTenantJobData(IOperationalTaskModel viewModel, Job job, Guid departmentId)
        {
            viewModel.Id = job.Id;
            viewModel.Title = job.Title;
            viewModel.Address = job.GetAddress(departmentId);
        }

        private void SetCorrectUrlForUploadList(List<UploadFileModel> uploadList, Guid jobAssignId)
        {
            foreach (var upload in uploadList)
            {
                var extension = Path.GetExtension(upload.Path);
                upload.Path = pathHelper.GetJobAssignUploadsPath(jobAssignId, upload.FileId, extension);
            }
        }

        private DateTime GetAssignedDate(DateTime date, DateTime time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc);
        }

        private async Task<IOperationalTaskModel> GetAdHocTask(Guid dayAssignId)
        {
            IOperationalTaskModel model = new OperationalTaskModel();
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);

            Job job = await jobProvider.Get(dayAssign.JobId);
            FillJobData(model, job, dayAssign.DepartmentId, JobTypeEnum.AdHock);

            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(dayAssign.JobId, dayAssign.DepartmentId);
            FillJobAssignData(model, jobAssign, dayAssign.DepartmentId, JobTypeEnum.AdHock);

            FillDayAssignData(model, dayAssign, JobTypeEnum.AdHock);

            FillCategory(model);

            return model;
        }

        private async Task<IOperationalTaskModel> GetTenantTask(Guid dayAssignId)
        {
            IOperationalTaskModel model = new OperationalTaskModel();
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);

            Job job = await jobProvider.Get(dayAssign.JobId);
            FillJobData(model, job, dayAssign.DepartmentId, JobTypeEnum.Tenant);

            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(dayAssign.JobId, dayAssign.DepartmentId);
            FillJobAssignData(model, jobAssign, dayAssign.DepartmentId, JobTypeEnum.Tenant);

            FillDayAssignData(model, dayAssign, JobTypeEnum.Tenant);

            return model;
        }

        private async Task<IOperationalTaskModel> GetOtherTask(Guid dayAssignId)
        {
            IOperationalTaskModel model = new OperationalTaskModel();
            IDayAssign dayAssign = dayAssignService.GetDayAssignById(dayAssignId);

            Job job = await jobProvider.Get(dayAssign.JobId);
            FillJobData(model, job, dayAssign.DepartmentId, JobTypeEnum.Other);

            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(dayAssign.JobId, dayAssign.DepartmentId);
            FillJobAssignData(model, jobAssign, dayAssign.DepartmentId, JobTypeEnum.Other);

            FillDayAssignData(model, dayAssign, JobTypeEnum.Other);

            return model;
        }

        private async Task SetStatusAfterCreation(Guid dayAssignId, Guid? teamLeadId)
        {
            if (teamLeadId.HasValue)
            {
                await jobStatusService.Assigned(dayAssignId, JobStatus.Pending);
            }
            else
            {
                await jobStatusService.Opened(dayAssignId, JobStatus.Pending);
            }
        }
    }
}
