using AbsenceTemplatesCore.Contract.Interfaces;
using Infrastructure.Constants;
using Infrastructure.CustomAttributes;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using MemberCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Web.Models;
using YearlyPlanning.Services;

namespace Web.Controllers
{
    [RoutePrefix("api/management")]
    public class ManagementController : ApiController
    {
        private readonly IMemberService memberService;
        private readonly ITimeScheduleService timeSheduleService;
        private readonly IAppSettingHelper appSettingHelper;
        private readonly IEmployeeAbsenceInfoService absenceInfoService;

        public ManagementController(
            IMemberService memberService,
            ITimeScheduleService timeSheduleService,
            IAppSettingHelper appSettingHelper,
            IEmployeeAbsenceInfoService absenceInfoService)
        {
            this.memberService = memberService;
            this.timeSheduleService = timeSheduleService;
            this.appSettingHelper = appSettingHelper;
            this.absenceInfoService = absenceInfoService;
        }

        [PreventSpam]
        [HttpPost, Route("getManagementDepartmentTimeView")]
        public ManagementDepartmentTimeView GetManagementDepartmentTimeView(EmployeeTimeFilterViewModel model)
        {
            List<IMemberModel> members = model.MemberIds.HasValue()
                ? memberService.GetByIds(model.MemberIds).ToList()
                : memberService.GetEmployeesByManagementDepartment(model.ManagementDepartmentId).ToList();            

            var memberTimeViewModels = GetMemberTimeView(members, model.ManagementDepartmentId, model.Year, model.Week);
            var result = new ManagementDepartmentTimeView
            {
                MembersTimeView = memberTimeViewModels,
                MembersTotal = memberTimeViewModels.Select(x => x.WeekTotal).Aggregate(new TimeViewModel(), (total, timeView) => total + timeView)
            };

            return result;
        }

        private List<MemberTimeView> GetMemberTimeView(List<IMemberModel> memberList, Guid managementDepartmentId, int currentYear, int currentWeek)
        {
            List<Guid> memberIdList = memberList.Select(x => x.MemberId).ToList();
            IDictionary<Guid, IDictionary<int,int>> weekDaysEstimationsForWeek = timeSheduleService.GetMemberEstimationsForWeek(memberIdList, managementDepartmentId, currentYear, currentWeek);
            List<MemberTimeView> results = new List<MemberTimeView>();
            IDictionary<int, int> weekDaysWorkingHours = appSettingHelper.GetDictionaryAppSetting<int, int>(Constants.AppSetting.DaysWorkingMinutes);
            IDictionary<Guid, IDictionary<int, bool>> weekDaysMembersAbsences = absenceInfoService.GetWeekAbsencesForMembers(memberIdList, currentYear, currentWeek);

            foreach (var member in memberList)
            {
                IDictionary<int, bool> weekDaysMemberAbsences = weekDaysMembersAbsences[member.MemberId];
                IDictionary<int, int> weekDaysWorkingHoursExeptAbsences = weekDaysWorkingHours.ToDictionary(pair => pair.Key, pair => weekDaysMemberAbsences[pair.Key] ? 0 : pair.Value);

                IEnumerable<TimeViewModel> timeViewForWeek = TimeViewsForWeek(weekDaysEstimationsForWeek[member.MemberId], weekDaysWorkingHoursExeptAbsences);
                
                results.Add(new MemberTimeView
                    {
                        MemberModel = member,
                        WeekTimeView = timeViewForWeek,
                        WeekTotal = timeViewForWeek.Aggregate(new TimeViewModel(), (total, timeView) => total + timeView)
                    }
                );
            }

            return results;
        }

        private IEnumerable<TimeViewModel> TimeViewsForWeek(IDictionary<int, int> weekDaysEstimatings, IDictionary<int, int> weekDaysWorkingHours)
        {
            int value;
            return Enumerable.Range(1, Constants.DateTime.DaysInWeek).Select(day => new TimeViewModel()
            {
                ScheduledMinutes = weekDaysEstimatings.TryGetValue(day, out value) ? value : 0,
                WorkingMinutes = weekDaysWorkingHours.TryGetValue(day, out value) ? value : 0
            });
        }
    }
}