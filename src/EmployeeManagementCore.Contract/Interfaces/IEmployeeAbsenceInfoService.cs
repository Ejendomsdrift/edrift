using System;
using System.Collections.Generic;

namespace AbsenceTemplatesCore.Contract.Interfaces
{
    public interface IEmployeeAbsenceInfoService
    {
        IEnumerable<IEmployeeAbsences> GetByManagementDeparmentId(Guid managementDepartmentId);
        IEnumerable<IEmployeeAbsenceInfoModel> GetlByIds(IEnumerable<Guid> absencesIdList);
        IAbsenceCreationResult Add(IEmployeeAbsenceInfoModel absenceInfo);
        void Delete(Guid absenceInfoId);
        void DeleteAllAbsencesForMember(Guid memberId);
        IDictionary<int, bool> GetWeekAbsencesForMember(Guid memberId, int year, int week);
        IDictionary<int, bool> GetWeekAbsencesForMember(IEnumerable<IEmployeeAbsenceInfoModel> absences, int year, int week);
        IDictionary<Guid, IDictionary<int, bool>> GetWeekAbsencesForMembers(IEnumerable<Guid> membersIds, int year, int week);
        IDictionary<Guid, bool> GetDayAbsencesForMembers(IEnumerable<Guid> memberIds, int year, int week, int weekDay);
        IEnumerable<IEmployeeAbsenceInfoModel> GetByMemberIdsForPeriod(IEnumerable<Guid> memberIds, DateTime startDate, DateTime endDate);
        bool IsMemberHasAbsenceForDate(Guid memberId, DateTime date);
    }
}