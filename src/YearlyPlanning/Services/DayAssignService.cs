using AutoMapper;
using GroupsContract.Interfaces;
using GroupsContract.Models;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers.Implementation;
using Infrastructure.Messaging;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ManagementDepartmentCore.Contract.Interfaces;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class DayAssignService : IDayAssignService
    {
        private readonly IJobAssignProvider jobAssignProvider;
        private readonly IMessageBus messageBus;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly IJobStatusService jobStatusService;
        private readonly IMemberService memberService;
        private readonly IGroupService groupService;
        private readonly IRepository<DayAssign> dayAssignRepository;

        public DayAssignService(
            IMessageBus messageBus, 
            IJobAssignProvider jobAssignProvider,
            IDayAssignProvider dayAssignProvider, 
            IJobStatusService jobStatusService, 
            IMemberService memberService,
            IGroupService groupService,
            IRepository<DayAssign> dayAssignRepository)
        {
            this.messageBus = messageBus;
            this.jobAssignProvider = jobAssignProvider;
            this.dayAssignProvider = dayAssignProvider;
            this.jobStatusService = jobStatusService;
            this.memberService = memberService;
            this.groupService = groupService;
            this.dayAssignRepository = dayAssignRepository;
        }

        public IEnumerable<Guid> GetDayAssignIds(ITaskDataFilterModel filter)
        {
            var ids = dayAssignProvider.GetDayAssignIds(filter);
            return ids;
        }

        public async Task<Guid> CreateDayAssign(INewDayAssignModel model)
        {
            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(model.JobId, model.DepartmentId);

            if (jobAssign == null && model.JobAssignId != Guid.Empty)
            {
                jobAssign = jobAssignProvider.GetById(model.JobAssignId);
            }

            var day = jobAssign?.DayPerWeekList.FirstOrDefault(dp => dp.WeekDay == model.CurrentWeekDay);

            model.Id = Guid.NewGuid();
            model.JobAssignId = jobAssign.Id;

            Guid dayPerWeekId = model.DayPerWeekId ?? day?.Id ?? Guid.Empty;
            var command = Mapper.Map(model, new CreateDayAssignCommand(dayPerWeekId));
            await messageBus.Publish(command);

            return model.Id;
        }

        public async Task<Guid> ChangeDayAssignDate(INewDayAssignModel model)
        {
            var year = model.Year != default(int) ? model.Year : DateTime.UtcNow.Year;
            if (model.WeekDay.HasValue)
            {
                model.Date = CalendarHelper.GetDateByWeekAndDayNumber(year, model.WeekNumber, model.WeekDay.Value);
            }

            var dayAssign = model.DayAssignId.HasValue ? dayAssignProvider.Get(model.DayAssignId.Value) : null;
            if (dayAssign == null)
            {
                return await CreateDayAssign(model);
            }
            else
            {
                model.Id = model.DayAssignId.Value;
                await messageBus.Publish(model.Map<ChangeDayAssignDateCommand>());

                if (dayAssign.StatusId == JobStatus.Expired)
                {
                    await jobStatusService.Pending(dayAssign.Id, dayAssign.StatusId);
                }

                return model.DayAssignId.Value;
            }
        }

        public async Task<Guid> CreateDayAssignWithEstimate(INewDayAssignModel model)
        {
            JobAssign jobAssign = jobAssignProvider.GetAssignByJobIdAndDepartmentId(model.JobId, model.DepartmentId);

            if (jobAssign == null && model.JobAssignId != Guid.Empty)
            {
                jobAssign = jobAssignProvider.GetById(model.JobAssignId);
            }

            var jobResponsible = jobAssign.JobResponsibleList.FirstOrDefault(j => j.JobId == model.JobId && j.HousingDepartmentId == model.DepartmentId);

            if (jobResponsible != null)
            {
                var day = jobAssign?.DayPerWeekList.FirstOrDefault(dp => dp.WeekDay == model.CurrentWeekDay);

                model.Id = Guid.NewGuid();
                model.JobAssignId = jobAssign.Id;
                model.EstimatedMinutes = (int)jobResponsible?.EstimateInMinutes;
                model.IsAssignedToAllUsers = jobResponsible?.IsAssignedToAllUsers ?? default(bool);
                model.WeekDay = day.WeekDay;

                if (jobResponsible.GroupId != Guid.Empty)
                {
                    model.GroupId = jobResponsible.GroupId;
                }

                if (jobResponsible.TeamLeadId != Guid.Empty)
                {
                    model.TeamLeadId = jobResponsible.TeamLeadId;
                }

                model.UserIdList = jobResponsible?.UserIdList ?? new List<Guid>();
                Guid dayPerWeekId = model.DayPerWeekId ?? day?.Id ?? Guid.Empty;

                var command = Mapper.Map(model, new CreateDayAssignCommand(dayPerWeekId));
                await messageBus.Publish(command);

                return model.Id;
            }
            return Guid.Empty;
        }

        public async Task<IChangeStatusModel> AssignsMembersGroup(INewDayAssignModel model)
        {
            IChangeStatusModel moveToStatusResultModel = new ChangeStatusModel();

            DayAssign dayAssign = await CreateOrGetDayAssign(model);

            try
            {
                if (model.IsUnassignedAll)
                {
                    await jobStatusService.Pending(model.DayAssignId.Value, dayAssign.StatusId);
                }
                else if (dayAssign.StatusId != JobStatus.Assigned && model.TeamLeadId.HasValue)
                {
                    await jobStatusService.Assigned(dayAssign.Id, dayAssign.StatusId);
                }
                else if (dayAssign.StatusId != JobStatus.Opened && !model.TeamLeadId.HasValue)
                {
                    await jobStatusService.Opened(dayAssign.Id, dayAssign.StatusId);
                }

                model.Id = dayAssign.Id;
                await messageBus.Publish(model.Map<ChangeDayAssignMembersComand>());

                moveToStatusResultModel.IsSuccessful = true;
            }
            catch (NotImplementedException)
            {
                moveToStatusResultModel.IsSuccessful = false;
            }

            moveToStatusResultModel.DayAssignId = dayAssign.Id;
            return moveToStatusResultModel;

        }

        private async Task<DayAssign> CreateOrGetDayAssign(INewDayAssignModel model)
        {
            DayAssign dayAssign = model.DayAssignId.HasValue ? dayAssignProvider.Get(model.DayAssignId.Value) : null;
            if (dayAssign == null)
            {
                Guid dayAssignId = await CreateDayAssign(model);
                dayAssign = dayAssignProvider.Get(dayAssignId);
            }
            return dayAssign;
        }

        public async Task<IDayAssign> GetByJobId(string jobId)
        {
            IDayAssign result = await dayAssignProvider.GetByJobId(jobId);
            return result;
        }

        public async Task<Guid> CancelJob(INewDayAssignModel model)
        {
            if (!model.DayAssignId.HasValue || model.DayAssignId == Guid.Empty)
            {
                model.DayAssignId = await CreateDayAssign(model);
            }

            IDayAssign dayAssign = GetDayAssignById(model.DayAssignId.Value);
            await jobStatusService.Cancel(model.DayAssignId.Value, dayAssign.StatusId, model.CancellationReasonId);
            return model.DayAssignId.Value;
        }

        public async Task ReopenJob(Guid dayAssignId)
        {
            IDayAssign dayAssign = GetDayAssignById(dayAssignId);
            await jobStatusService.Assigned(dayAssignId, dayAssign.StatusId);
        }

        public List<IDayAssign> GetDayAssignsForCurrentUserHousingDepartments(IEnumerable<Guid> departmentIds = null)
        {
            return dayAssignProvider.Find(da => departmentIds.Contains(da.DepartmentId)).ToList<IDayAssign>();
        }

        public List<IDayAssign> GetListForCurrentUser(IEnumerable<Guid> housingDepartmentIds, IEnumerable<Guid> groupIds, IEnumerable<JobStatus> statuses)
        {
            housingDepartmentIds = housingDepartmentIds ?? Enumerable.Empty<Guid>();
            groupIds = groupIds ?? Enumerable.Empty<Guid>();

            var result = dayAssignProvider.Query.Where(i =>
                                i.WeekDay != null &&
                                (i.GroupId == null || groupIds.Contains(i.GroupId.Value)) && 
                                housingDepartmentIds.Contains(i.DepartmentId) && 
                                statuses.Contains(i.StatusId))
                                .ToList<IDayAssign>();

            return result;
        }

        public async Task AssignJob(Guid dayAssignId)
        {
            IMemberModel member = memberService.GetCurrentUser();
            DayAssign dayAssign = dayAssignProvider.Get(dayAssignId);

            var userIds = dayAssign.UserIdList ?? new List<Guid>();
            userIds.Add(member.MemberId);

            var memberModel = new ChangeDayAssignMembersComand(dayAssign.Id.ToString())
            {
                UserIdList = userIds,
                GroupId = dayAssign.GroupId,
                TeamLeadId = dayAssign.TeamLeadId.HasValue ? dayAssign.TeamLeadId : member.MemberId,
                IsAssignedToAllUsers = dayAssign.IsAssignedToAllUsers && dayAssign.TeamLeadId.HasValue
            };

            await messageBus.Publish(memberModel);

            await jobStatusService.Assigned(dayAssignId, dayAssign.StatusId);
        }

        public async Task UnassignJob(Guid dayAssignId, string changeStatusComment, List<IMemberSpentTimeModel> members, JobStatus newJobStatus, Guid? selectedCancellingId, Guid[] uploadedFileIds)
        {
            if (!selectedCancellingId.HasValue)
            {
                throw new InvalidOperationException("Wrong parameters passed");
            }

            DayAssign dayAssign = dayAssignProvider.Get(dayAssignId);

            if (IsAllowChangeDayAssignStatus(dayAssign))
            {
                var memberModel = new ChangeDayAssignMembersComand(dayAssign.Id.ToString())
                {
                    UserIdList = new List<Guid>(),
                    GroupId = newJobStatus != JobStatus.Pending ? dayAssign.GroupId : null,
                    TeamLeadId = null,
                    IsAssignedToAllUsers = newJobStatus != JobStatus.Pending && dayAssign.IsAssignedToAllUsers
                };

                await messageBus.Publish(memberModel);

                if (newJobStatus == JobStatus.Pending)
                {
                    await jobStatusService.Pending(dayAssignId, dayAssign.StatusId, changeStatusComment, members, selectedCancellingId, uploadedFileIds);
                }
                else
                {
                    await jobStatusService.Rejected(dayAssignId, dayAssign.StatusId, changeStatusComment, members, selectedCancellingId, uploadedFileIds);
                }
            }
        }

        public bool IsAllowChangeDayAssignStatus(IDayAssign dayAssign)
        {
            bool isMultiAssign = IsMultiAssign(dayAssign);
            return IsCurrentUserTeamLead(dayAssign) && isMultiAssign || !isMultiAssign;
        }

        public IDayAssign GetDayAssignById(Guid dayAssignId)
        {
            return dayAssignProvider.Get(dayAssignId);
        }

        public IEnumerable<IDayAssign> GetForStatisticByIdsWithRestrictions(List<Guid> dayAssignsIdList, IEnumerable<Guid> housingDepartmentsIds = null)
        {
            Expression<Func<IDayAssign, bool>> filter = d => dayAssignsIdList.Contains(d.Id);
            filter = filter.And(d => !d.JobId.StartsWith(Constants.TaskId.Other));

            if (housingDepartmentsIds != null)
            {
                filter = filter.And(d => housingDepartmentsIds.Contains(d.DepartmentId));
            }
            else
            {
                filter = filter.And(d => d.DepartmentId != Guid.Empty);
            }

            IEnumerable<IDayAssign> dayAssignList = dayAssignProvider.Query.Where(filter);
            return dayAssignList;
        }

        public List<IDayAssign> GetForStatisticTimeSpan(DateTime? startDate, DateTime? endDate, IEnumerable<JobStatus> allowedJobStatusList = null, 
            IEnumerable<Guid> housingDepartmentsIds = null)
        {
            Expression<Func<IDayAssign, bool>> filter = d => d.Date.HasValue;
            filter = filter.And(d => !d.JobId.StartsWith(Constants.TaskId.Other));
            filter = filter.And(d => d.DepartmentId != Guid.Empty);

            if (startDate.HasValue)
            {
                filter = filter.And(d => d.Date.Value >= startDate);
            }

            if (endDate.HasValue)
            {
                filter = filter.And(d => d.Date.Value <= endDate);
            }

            if (housingDepartmentsIds != null && housingDepartmentsIds.HasValue())
            {
                filter = filter.And(d => housingDepartmentsIds.Contains(d.DepartmentId));
            }

            if (allowedJobStatusList != null && allowedJobStatusList.HasValue())
            {
                filter = filter.And(d => allowedJobStatusList.Contains(d.StatusId));
            }

            var result = dayAssignProvider.Query.Where(filter).ToList();
            return result;
        }

        public List<IDayAssign> GetDayAssigns(Guid jobAssignId, string jobId, Guid departmentId, int weekNumber)
        {
            List<DayAssign> result = dayAssignProvider.GetDayAssigns(jobAssignId, jobId, departmentId, weekNumber);
            return result.ToList<IDayAssign>();
        }

        public List<IDayAssign> GetForStatisticTenantTask(DateTime startDate, DateTime endDate, IEnumerable<JobStatus> jobStatusList, IEnumerable<Guid> housingDepartmentIds)
        {
            Expression<Func<IDayAssign, bool>> filter = d => d.Date.HasValue;

            if (housingDepartmentIds.HasValue())
            {
                filter = filter.And(d => housingDepartmentIds.Contains(d.DepartmentId));
            }

            filter = filter.And(d => d.JobId.StartsWith(Constants.TaskId.Tenant));
            filter = filter.And(d => d.Date.Value >= startDate && d.Date.Value <= endDate);
            filter = filter.And(d => jobStatusList.Contains(d.StatusId));
            filter = filter.And(d => d.TenantType != null);

            List<IDayAssign> dayAssignList = dayAssignProvider.Query.Where(filter).ToList();
            return dayAssignList;
        }

        public IEnumerable<int> GetWeeksWithExpiredTasks(Guid jobAssignId)
        {
            var weeks = dayAssignProvider.Query
                                         .Where(i => i.JobAssignId == jobAssignId && i.ExpiredDayAssignId != null)
                                         .Select(i => i.WeekNumber).ToList();
            return weeks;
        }

        public IEnumerable<IDayAssign> GetByIds(IEnumerable<Guid> ids)
        {
            var result = dayAssignProvider.Query.Where(d => ids.Contains(d.Id));
            return result;
        }

        public IEnumerable<IDayAssign> GetByJobIds(IEnumerable<string> jobIds)
        {
            IEnumerable<IDayAssign> result = dayAssignProvider.Query.Where(d => jobIds.Contains(d.JobId));
            return result;
        }

        public int GetPreviousNotEmptyWeekNumber(IWeekPlanFilterModel filter)
        {
            int result = default(int);
            IWeekPlanFilterModel clonedFilter = filter.Clone();

            while (clonedFilter.Week > default(int))
            {
                if (dayAssignProvider.HasTasks(clonedFilter))
                {
                    result = clonedFilter.Week;
                    break;
                }

                --clonedFilter.Week;
            }

            return result;
        }

        public void UpdateDayAssignEstimate(List<Guid> jobAssignIdList, string jobId, Guid departmentId, int estimateMinutes)
        {
            var notAllowedStatuseList = new List<JobStatus>
            {
                JobStatus.Completed,
                JobStatus.Expired,
                JobStatus.Paused
            };

            List<DayAssign> dayAssignList = dayAssignProvider.GetDayAssigns(jobAssignIdList, jobId, departmentId, notAllowedStatuseList);
            List<Guid> dayAssignIds = dayAssignList.Select(d => d.Id).ToList();
            dayAssignProvider.UpdateEstimate(dayAssignIds, estimateMinutes);
        }

        public void UpdateDayAssignTeam(List<Guid> jobAssignIdList, string jobId, Guid departmentId,
            bool isAssignedToAllUser, Guid? groupId, Guid teamLeadId, List<Guid> userIdList)
        {
            var notAllowedStatuseList = new List<JobStatus>
            {
                JobStatus.Completed,
                JobStatus.Expired,
                JobStatus.Paused
            };

            List<DayAssign> dayAssignList = dayAssignProvider.GetDayAssigns(jobAssignIdList, jobId, departmentId, notAllowedStatuseList);
            List<Guid> dayAssignIds = dayAssignList.Select(d => d.Id).ToList();
            dayAssignProvider.UpdateTeam(dayAssignIds, isAssignedToAllUser, groupId, teamLeadId, userIdList);
        }

        private bool IsMultiAssign(IDayAssign dayAssign)
        {
            bool isGroupContainMoreThenOneUser = false;
            if (dayAssign.GroupId.HasValue)
            {
                IGroupModel group = groupService.Get(dayAssign.GroupId.Value);
                isGroupContainMoreThenOneUser = group.MemberIds.Count() > 1;
            }

            return dayAssign.UserIdList.Count > 1 || isGroupContainMoreThenOneUser || dayAssign.IsAssignedToAllUsers;
        }

        private bool IsCurrentUserTeamLead(IDayAssign dayAssign)
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            return dayAssign.TeamLeadId == currentUser.MemberId;
        }
    }
}
