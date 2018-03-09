using GroupsContract.Interfaces;
using GroupsContract.Models;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class TimeScheduleService : ITimeScheduleService
    {
        private readonly IDayAssignProvider assignProvider;
        private readonly IGroupService groupService;
        private readonly IMemberService memberService;
        private readonly IManagementDepartmentService managementService;

        public TimeScheduleService(
            IDayAssignProvider assignProvider,
            IGroupService groupService,
            IMemberService memberService,
            IManagementDepartmentService managementService)
        {
            this.assignProvider = assignProvider;
            this.groupService = groupService;
            this.memberService = memberService;
            this.managementService = managementService;
        }

        public IDictionary<Guid, int> GetMembersEstimationsForDay(IEnumerable<Guid> memberIds, Guid managementDepartmentId, int year, int week, int day)
        {
            PeriodMembersEstimationModel periodEstimationModel = GetAssignForPeriod(memberIds, managementDepartmentId, year, week, day);

            IDictionary<Guid, int> userInGroupsCount = periodEstimationModel.UserInGroups.ToDictionary(k => k.Key, v => v.Value.Count());

            IDictionary<Guid, int> dayEstimations = periodEstimationModel.Assigns
                .ToDictionary(pair => pair.Key, pair => FormDayEstimation(pair.Value, periodEstimationModel.UsersCount, userInGroupsCount));

            return dayEstimations;
        }

        public IDictionary<Guid, IDictionary<int, int>> GetMemberEstimationsForWeek(IEnumerable<Guid> memberIds, Guid managementDepartmentId, int year, int week)
        {
            var results = new Dictionary<Guid, IDictionary<int, int>>();

            List<Guid> memberIdList = memberIds.AsList();
            PeriodMembersEstimationModel periodEstimationModel = GetAssignForPeriod(memberIdList, managementDepartmentId, year, week, day: null);

            IDictionary<Guid, int> userInGroupsCount = periodEstimationModel.UserInGroups.ToDictionary(k => k.Key, v => v.Value.Count());

            foreach (var member in memberIdList)
            {
                var memberAssignForWeek = periodEstimationModel.Assigns[member];
                IDictionary<int, int> formWeekEstimations = GetWeekEstimation(memberAssignForWeek, periodEstimationModel.UsersCount, userInGroupsCount);
                results.Add(member, formWeekEstimations);
            }

            return results;
        }

        private IEnumerable<Guid> GetHousingDepartmentIds(Guid managementDepartmentId)
        {
            List<IHousingDepartmentModel> housingDepartments = managementService.GetHousingDepartments(managementDepartmentId).ToList();

            return housingDepartments.Select(x => x.Id);
        }

        private IDictionary<Guid, IEnumerable<Guid>> GetAssignedUserInGroup(IEnumerable<IDayAssign> weekDayAssigns)
        {
            var result = new Dictionary<Guid, IEnumerable<Guid>>();

            Dictionary<Guid, Guid> dayAssignIds = weekDayAssigns.Where(d => d.GroupId.HasValue).ToDictionary(k => k.Id, v => v.GroupId.Value);

            IEnumerable<Guid> groupsIds = dayAssignIds.Select(g => g.Value).Distinct();
            List<IGroupModel> groups = groupService.GetByIds(groupsIds).ToList();

            foreach (var dayAssign in dayAssignIds)
            {
                IGroupModel group = groups.First(g => g.Id == dayAssign.Value);
                IEnumerable<Guid> members = group.MemberIds;

                result.Add(dayAssign.Key, members);
            }

            return result;
        }

        private IDictionary<int, int> GetWeekEstimation(IEnumerable<IDayAssign> weekDayAssigns, int assignedUsersCount, IDictionary<Guid, int> assignedUserInGroupsCount)
        {
            var groupByDay = weekDayAssigns.GroupBy(GetWeekDay);
            var dayEstimatingsDictionary = groupByDay.ToDictionary(g => g.Key, v => AggregateDayAssignEstimations(v, assignedUsersCount, assignedUserInGroupsCount));

            return dayEstimatingsDictionary;
        }

        private int GetWeekDay(IDayAssign dayAssign)
        {
            if (dayAssign.WeekDay.HasValue)
            {
                return dayAssign.WeekDay.Value;
            }

            int weekDay = dayAssign.Date.Value.GetWeekDayNumber();

            return weekDay;
        }

        private int AggregateDayAssignEstimations(IEnumerable<IDayAssign> dayAssigns, int assignedUsersCount, IDictionary<Guid, int> assignedUserInGroupsCount)
        {
            var estimationsWithGroupCorrection = dayAssigns
                .Select(d => GetEstimateWithGroupCorrection(d.EstimatedMinutes.Value, GetInvolvedMembersCount(d, assignedUsersCount, assignedUserInGroupsCount)));
            var singleEstimating = estimationsWithGroupCorrection.Aggregate(0, (total, minutes) => total + minutes);

            return singleEstimating;
        }

        private int GetInvolvedMembersCount(IDayAssign dayAssign, int assignedUsersCount, IDictionary<Guid, int> assignedUserInGroupsCount)
        {
            var result = default(int);
            
            result += !dayAssign.GroupId.HasValue && dayAssign.IsAssignedToAllUsers ? assignedUsersCount : default(int);
            result += GetUsersCountIfTaskAssignedToSomeUsers(dayAssign);
            result += dayAssign.GroupId.HasValue && dayAssign.IsAssignedToAllUsers ? assignedUserInGroupsCount[dayAssign.Id] : default(int);
            result += GetUsersCountIfTaskAssignedToSomeUsersInGroup(dayAssign);

            return result;
        }

        private int GetUsersCountIfTaskAssignedToSomeUsers(IDayAssign dayAssign)
        {
            var assignedUsersCount = default(int);

            if (!dayAssign.GroupId.HasValue && dayAssign.UserIdList.Any())
            {
                assignedUsersCount += dayAssign.UserIdList.Count;
            }

            return assignedUsersCount;
        }

        private int GetUsersCountIfTaskAssignedToSomeUsersInGroup(IDayAssign dayAssign)
        {
            var assignedUsersCount = default(int);

            if (dayAssign.GroupId.HasValue && dayAssign.UserIdList.Any())
            {
                assignedUsersCount += dayAssign.UserIdList.Count;
            }

            return assignedUsersCount;
        }

        private int GetEstimateWithGroupCorrection(int estimated, int numberOfAssigned)
        {
            var estimatePerPerson = numberOfAssigned > 1 ? Math.Ceiling((double)estimated / numberOfAssigned) : estimated;

            return (int)estimatePerPerson;
        }

        private int FormDayEstimation(IEnumerable<IDayAssign> dayDayAssigns, int assignedUsersCount, IDictionary<Guid, int> assignedUserInGroupsCount)
        {
            int result = dayDayAssigns
                .Select(dayAssign => GetEstimateWithGroupCorrection(dayAssign.EstimatedMinutes.Value, GetInvolvedMembersCount(dayAssign, assignedUsersCount, assignedUserInGroupsCount)))
                .Aggregate(0, (estimatedTotal, estimatedMinutes) => estimatedTotal + estimatedMinutes);

            return result;
        }

        private IEnumerable<IDayAssign> GetAssigns(int year, int week, int? day, IEnumerable<Guid> housingDepartmentIds, bool withEstimatedMinutes = false, bool withDate = false)
        {
            var filter = new MemberDayAssignFilterModel
            {
                Year = year,
                Week = week,
                Day = day,
                WithEstimatedMinutes = withEstimatedMinutes,
                WithDate = withDate,
                HousingDepartmentIds = housingDepartmentIds
            };

            return GetDayAssigns(filter);
        }

        private IEnumerable<IDayAssign> GetDayAssigns(MemberDayAssignFilterModel filter)
        {
            IEnumerable<IDayAssign> dayAssigns = assignProvider.GetDayAssignsForMembersByFilter(filter);

            return dayAssigns;
        }

        private IDictionary<Guid, IEnumerable<IDayAssign>> GetMemberDayAssigns(IEnumerable<IDayAssign> dayAssigns, IEnumerable<Guid> memberIds, IDictionary<Guid, IEnumerable<Guid>> userInGroups)
        {
            IDictionary<Guid, IEnumerable<IDayAssign>> result = memberIds.ToDictionary(k => k, v => dayAssigns.Where(d => (!d.GroupId.HasValue && d.UserIdList.Contains(v)) ||
                                                                                                                          (!d.GroupId.HasValue && d.IsAssignedToAllUsers) ||
                                                                                                                          (d.GroupId.HasValue && userInGroups[d.Id].Contains(v))));

            return result;
        }

        private PeriodMembersEstimationModel GetAssignForPeriod(IEnumerable<Guid> memberIds, Guid managementDepartmentId, int year, int week, int? day)
        {
            List<Guid> memberIdList = memberIds.AsList();

            IEnumerable<Guid> housingDepartmentIds = GetHousingDepartmentIds(managementDepartmentId);
            List<IDayAssign> allDayAssigns = GetAssigns(year, week, day, housingDepartmentIds).ToList();
            string managementDepartmentSyncId = managementService.GetManagementDepartmentSyncId(managementDepartmentId);

            var userInGroups = GetAssignedUserInGroup(allDayAssigns);

            var result = new PeriodMembersEstimationModel
            {
                Assigns = GetMemberDayAssigns(allDayAssigns, memberIdList, userInGroups),
                UserInGroups = userInGroups,
                UsersCount = memberService.GetAllJanitorsCount(managementDepartmentSyncId)
            };

            return result;
        }
    }
}