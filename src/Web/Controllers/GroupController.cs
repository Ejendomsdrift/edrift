using AbsenceTemplatesCore.Contract.Interfaces;
using GroupsContract.Commands;
using GroupsContract.Interfaces;
using GroupsContract.Models;
using Infrastructure.Constants;
using Infrastructure.CustomAttributes;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Messaging;
using MemberCore.Contract.Interfaces;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Models;
using Web.Models.Group;
using YearlyPlanning.ReadModel;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [RoutePrefix("api/group")]
    public class GroupController : ApiController
    {
        private readonly IMessageBus messageBus;
        private readonly IGroupService groupService;
        private readonly IMemberService memberService;
        private readonly ITimeScheduleService timeScheduleService;
        private readonly IAppSettingHelper settingHelper;
        private readonly IDayAssignProvider dayAssignProvider;
        private readonly IEmployeeAbsenceInfoService absenceInfoService;

        public GroupController(IMessageBus messageBus,
            IGroupService groupService,
            IMemberService memberService,
            ITimeScheduleService timeScheduleService,
            IAppSettingHelper settingHelper,
            IDayAssignProvider dayAssignProvider,
            IEmployeeAbsenceInfoService absenceInfoService)
        {
            this.messageBus = messageBus;
            this.groupService = groupService;
            this.memberService = memberService;
            this.timeScheduleService = timeScheduleService;
            this.settingHelper = settingHelper;
            this.dayAssignProvider = dayAssignProvider;
            this.absenceInfoService = absenceInfoService;
        }

        [HttpGet, Route("getExistingGroups")]
        public List<GroupMemberViewModel> GetExistingGroups()
        {
            var currentUser = memberService.GetCurrentUser();
            var groups = groupService.GetByManagementId(currentUser.ActiveManagementDepartmentId.Value);
            var memberIds = groups.SelectMany(g => g.MemberIds).Distinct();
            var members = memberService.GetAllowedMembersForJobByIds(memberIds).ToList();
            var dayAssigns = dayAssignProvider.GetDayAssignsForGroups(groups.Select(g => g.Id));
            var result = new List<GroupMemberViewModel>();
            foreach (var group in groups)
            {
                var model = group.Map<GroupMemberViewModel>();
                model.IsCanBeDeleted = dayAssigns.All(d => d.GroupId != group.Id);
                model.Members = members
                    .FindAll(m => group.MemberIds.Contains(m.MemberId))
                    .Map<IEnumerable<MemberModelWithTimeView>>();
                result.Add(model);
            }
            return result;
        }

        [HttpPost, Route("getGroupedMembers")]
        public IEnumerable<GroupMemberViewModel> GetGroupedMembers([FromBody] DateTime? timeViewDayScope)
        {
            var currentUser = memberService.GetCurrentUser();
            var weekSchedule = settingHelper.GetDictionaryAppSetting<int, int>(Constants.AppSetting.DaysWorkingMinutes);

            var members = GetMembers(currentUser);
            var membersWithTimeViews = members.Map<IEnumerable<MemberModelWithTimeView>>();

            if (timeViewDayScope.HasValue)
            {
                IEnumerable<Guid> memberIds = members.Select(m => m.MemberId);
                var year = timeViewDayScope.Value.Year;
                var week = timeViewDayScope.Value.GetWeekNumber();
                var day = timeViewDayScope.Value.GetWeekDayNumber();
                IDictionary<Guid, int> membersEstimationsForDay = timeScheduleService.GetMembersEstimationsForDay(memberIds, currentUser.ActiveManagementDepartmentId.Value, year, week, day);
                IDictionary<Guid, bool> membersAbsencesForDay = absenceInfoService.GetDayAbsencesForMembers(memberIds, year, week, day);

                foreach (var memberModel in membersWithTimeViews)
                {
                    memberModel.TimeView = new TimeViewModel()
                    {
                        IsAbsent = membersAbsencesForDay [memberModel.MemberId],
                        ScheduledMinutes = membersEstimationsForDay[memberModel.MemberId],
                        WorkingMinutes = weekSchedule[timeViewDayScope.Value.GetWeekDayNumber()]
                    };
                }
            }

            var result = new List<GroupMemberViewModel>();
            var groups = GetGroups(currentUser);
            foreach (var group in groups)
            {
                var model = group.Map<GroupMemberViewModel>();
                model.Members = membersWithTimeViews.Where(i => group.MemberIds.Contains(i.MemberId));
                result.Add(model);
            }

            result.Add(new GroupMemberViewModel
            {
                Id = null,
                Members = membersWithTimeViews
            });

            return result;
        }

        [HttpGet, Route("get")]
        public GroupViewModel Get(Guid id)
        {
            var group = groupService.Get(id);
            var members = memberService.GetAllowedMembersForJobByDepartmentId(group.ManagementId);

            var result = group.Map<GroupViewModel>();
            result.AllowedMembers = members.Map<IEnumerable<MemberViewModel>>();

            return result; 
        }

        [HttpGet, Route("createGroup")]
        public async Task<string> CreateGroup(string name)
        {
            var currentMember = memberService.GetCurrentUser();
            var id = Guid.NewGuid().ToString();
            name = Regex.Replace(name, @"\s+", " ");
            await messageBus.Publish(new CreateGroup(id, name, currentMember.ActiveManagementDepartmentId.Value, false));
            return id;
        }

        [HttpPost, Route("delete")]
        public async Task Delete(string id)
        {
            await messageBus.Publish(new DeleteGroup(id, true));
        }

        [HttpPost, Route("assign")]
        public async Task Assign(GroupAssignMemberModel model)
        {
            await messageBus.Publish(new MembersAssign(model.GroupId.ToString(), model.MemberIds));
        }

        [HttpPost, Route("removeMember")]
        public async Task RemoveMember(GroupValueModel<string, Guid> model)
        {
            await messageBus.Publish(new MemberUnassign(model.GroupId, model.Value));
        }

        [HttpGet, Route("isUniqueName")]
        public bool IsUniqueName(string name)
        {
            var currentMember = memberService.GetCurrentUser();
            var groupName = Regex.Replace(name, @"\s+", " ");
            return groupService.IsUniqueName(currentMember.ActiveManagementDepartmentId.Value, groupName.ToLowerInvariant());
        }

        [HttpGet, Route("isCanRemoveMember")]
        public bool IsCanRemoveMember(Guid groupId, Guid memberId)
        {
            var statuses = new List<JobStatus> {JobStatus.Assigned, JobStatus.InProgress, JobStatus.Paused};
            var dayAssigns = dayAssignProvider.GetDayAssignsForGroupAndTeamLead(groupId, memberId)
                .Where(d => statuses.Contains(d.StatusId));

            return !dayAssigns.Any();
        }

        [HttpPost, Route("changeName")]
        public async Task ChangeName(GroupValueModel<string, string> model)
        {
            await messageBus.Publish(new UpdateGroup(model.GroupId, model.Value.Trim()));
        }

        private IEnumerable<IGroupModel> GetGroups(IMemberModel member)
        {
            if (member.IsAdmin())
            {
                return groupService.GetAll();
            }

            return groupService.GetByManagementId(member.ActiveManagementDepartmentId.Value);
        }

        private IEnumerable<IMemberModel> GetMembers(IMemberModel member)
        {
            if (member.IsAdmin())
            {
                return memberService.GetAll();
            }

            return memberService.GetAllowedMembersForJobByDepartmentId(member.ActiveManagementDepartmentId.Value);
        }
    }
}