using System;
using System.Collections.Generic;

namespace YearlyPlanning.Services
{
    public interface ITimeScheduleService
    {
        IDictionary<Guid, int> GetMembersEstimationsForDay(IEnumerable<Guid> memberIds, Guid managementDepartmentId, int year, int week, int day);
        IDictionary<Guid, IDictionary<int, int>> GetMemberEstimationsForWeek(IEnumerable<Guid> memberIds, Guid managementDepartmentId, int year, int week);
    }
}