using System;
using System.Collections.Generic;
using System.Linq;
using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Models;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers.Implementation;
using MemberCore.Contract.Interfaces;
using MongoRepository.Contract.Interfaces;

namespace AbsenceTemplatesCore.Implementation
{
    public class EmployeeAbsenceInfoService : IEmployeeAbsenceInfoService
    {
        private readonly IRepository<EmployeeAbsenceInfo> repository;
        private readonly IMemberService memberService;
        private readonly IAbsenceTemplatesService absenceTemplatesService;


        public EmployeeAbsenceInfoService(
            IRepository<EmployeeAbsenceInfo> repository,
            IMemberService memberService,
            IAbsenceTemplatesService absenceTemplatesService)
        {
            this.repository = repository;
            this.memberService = memberService;
            this.absenceTemplatesService = absenceTemplatesService;
        }

        public IEnumerable<IEmployeeAbsences> GetByManagementDeparmentId(Guid managementDepartmentId)
        {
            var members = memberService.GetEmployeesByManagementDepartment(managementDepartmentId).ToList();
            var membersAbsences = GetByMemberIds(members.Select(m => m.MemberId));

            var result = members.Select(m => new EmployeeAbsences()
            {
                Member = m,
                Absences = membersAbsences.Where(abs => abs.MemberId == m.MemberId && abs.EndDate.Date >= DateTime.UtcNow.Date).OrderBy(a => a.StartDate)
            });

            return result.OrderBy(list => list.Member.Name);
        }

        public IAbsenceCreationResult Add(IEmployeeAbsenceInfoModel absenceInfo)
        {
            var result = new AbsenceCreationResult();
            var userAbsences = GetByMemberId(absenceInfo.MemberId);
            var rangeIntersections = userAbsences.Where(i => isPeriodsIntersects(i.StartDate, i.EndDate, absenceInfo.StartDate, absenceInfo.EndDate));
            if (!rangeIntersections.Any())
            {
                var mappedAbsenceInfo = absenceInfo.Map<EmployeeAbsenceInfo>();
                repository.Save(mappedAbsenceInfo);
                result.Absence = absenceInfo;
                result.Absence.Id = mappedAbsenceInfo.Id;
                result.Absence.AbsenceTemplateId = absenceInfo.AbsenceTemplateId;
                result.IsSucceeded = true;
            }
            return result;
        }

        public void Delete(Guid absenceInfoId)
        {
            repository.UpdateManySingleProperty(t => t.Id == absenceInfoId, m => m.IsDeleted, true);
        }

        public void DeleteAllAbsencesForMember(Guid memberId)
        {
            repository.UpdateManySingleProperty(t => t.MemberId == memberId, m => m.IsDeleted, true);
        }

        public IDictionary<int, bool> GetWeekAbsencesForMember(Guid memberId, int year, int week)
        {
            var userAbsences = GetByMemberId(memberId);
            var firstDayOfWeekDate = CalendarHelper.GetFirstDayOfWeek(year, week);
            var result = Enumerable.Range(1, Constants.DateTime.DaysInWeek)
                .ToDictionary(d => d, d => userAbsences
                    .Any(a => DayIntersectWithPeriod(firstDayOfWeekDate.AddDays(d - 1), a.StartDate, a.EndDate)));
            return result;
        }

        public IDictionary<int, bool> GetWeekAbsencesForMember(IEnumerable<IEmployeeAbsenceInfoModel> absences, int year, int week)
        {
            DateTime firstDayOfWeekDate = CalendarHelper.GetFirstDayOfWeek(year, week);
            Dictionary<int, bool> result = Enumerable.Range(1, Constants.DateTime.DaysInWeek)
                .ToDictionary(d => d, d => absences
                    .Any(a => DayIntersectWithPeriod(firstDayOfWeekDate.AddDays(d - 1), a.StartDate, a.EndDate)));

            return result;
        }

        public IDictionary<Guid, IDictionary<int, bool>> GetWeekAbsencesForMembers(IEnumerable<Guid> memberIds, int yead, int week)
        {
            IDictionary<Guid, IDictionary<int, bool>> result = new Dictionary<Guid, IDictionary<int, bool>>();
            List<IEmployeeAbsenceInfoModel> absences = GetByMembersIds(memberIds).ToList();

            foreach (var id in memberIds)
            {
                IEnumerable<IEmployeeAbsenceInfoModel> memberAbsences = absences.Where(i => i.MemberId == id);
                result.Add(id, GetWeekAbsencesForMember(memberAbsences, yead, week));
            }

            return result;
        }

        public IDictionary<Guid, bool> GetDayAbsencesForMembers(IEnumerable<Guid> memberIds, int year, int week, int weekDay)
        {
            var date = CalendarHelper.GetDateByWeekAndDayNumber(year, week, weekDay);
            var membersAbsences = memberIds.ToDictionary(memberId => memberId, memberId => IsMemberAbsentAtDate(memberId, date));
            return membersAbsences;
        }

        public bool IsMemberAbsentAtDate(Guid memberId, DateTime date)
        {
            return GetByMemberId(memberId).Any(a => DayIntersectWithPeriod(date, a.StartDate, a.EndDate));
        }

        public IEnumerable<IEmployeeAbsenceInfoModel> GetByMemberIdsForPeriod(IEnumerable<Guid> memberIds, DateTime startDate, DateTime endDate)
        {
            var result = repository.Find(i => memberIds.Contains(i.MemberId) && !i.IsDeleted && i.StartDate <= endDate && i.EndDate >= startDate);
            var mappedResult = result.Map<IEnumerable<EmployeeAbsenceInfoModel>>();
            return mappedResult;
        }

        public bool IsMemberHasAbsenceForDate(Guid memberId, DateTime date)
        {
            // in db we store absences date like 2017-03-29 00:00:00.000Z, and parameter date might be 2017-03-29 05:00:00.000Z
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc); 
            DateTime endDate = startDate;
            return GetByMemberIdsForPeriod(new List<Guid>{ memberId }, startDate, endDate).Any();
        }

        public IEnumerable<IEmployeeAbsenceInfoModel> GetlByIds(IEnumerable<Guid> absencesIdList)
        {
            IEnumerable<EmployeeAbsenceInfo> result = repository.Find(i => absencesIdList.Contains(i.Id));
            var mappedResult = result.Map<IEnumerable<EmployeeAbsenceInfoModel>>();
            return mappedResult;
        }

        private bool isPeriodsIntersects(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 >= start2 && start1 <= end2 ||
                   end1 >= start2 && end1 <= end2 ||
                   start1 <= start2 && end1 >= end2;
        }

        private IEnumerable<IEmployeeAbsenceInfoModel> GetByMemberIds(IEnumerable<Guid> memberIds)
        {
            var result = repository.Find(i => memberIds.Contains(i.MemberId) && !i.IsDeleted);
            var mappedResult = result.Map<IEnumerable<EmployeeAbsenceInfoModel>>();
            return mappedResult;
        }

        private IEnumerable<IEmployeeAbsenceInfoModel> GetByMemberId(Guid memberId)
        {
            var result = repository.Find(i => memberId == i.MemberId && !i.IsDeleted);
            var mappedResult = result.Map<IEnumerable<EmployeeAbsenceInfoModel>>();
            return mappedResult;
        }

        private IEnumerable<IEmployeeAbsenceInfoModel> GetByMembersIds(IEnumerable<Guid> memberIds)
        {
            var result = repository.Find(i => memberIds.Contains(i.MemberId) && !i.IsDeleted);
            var mappedResult = result.Map<IEnumerable<EmployeeAbsenceInfoModel>>();
            return mappedResult;
        }

        private bool DayIntersectWithPeriod(DateTime day, DateTime periodStart, DateTime periodEnd)
        {
            return periodStart <= day && day <= periodEnd;
        }
    }
}