using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;

namespace YearlyPlanning.ReadModel
{
    public interface IDayAssignProvider
    {
        IQueryable<DayAssign> Query { get; }
        DayAssign Get(Guid id);
        Task<DayAssign> GetByJobId(string jobId);
        List<DayAssign> GetByJobIds(IEnumerable<string> jobIds);
        List<IDayAssign> GetDayAssignsForMember(Guid memberId, bool isNotAllowedEmptyDepartment, IEnumerable<Guid> departments = null, int? daysAhead = null);
        List<IDayAssign> GetDayAssignsForGroups(IEnumerable<Guid> groupIds);
        List<IDayAssign> GetDayAssignsForGroupAndTeamLead(Guid groupId, Guid memberId);
        List<DayAssign> GetDayAssigns(Guid jobAssignId, string jobId, Guid departmentId, int weekNumber);
        List<DayAssign> GetDayAssigns(List<Guid> jobAssignList, string jobId, Guid department, List<JobStatus> notAllowedStatusList);
        List<DayAssign> GetForWeek(Guid departmentId, int year, int weekNumber);
        List<DayAssign> GetForWeekForAllDepartments(int year, int weekNumber);
        Task<DayAssign> GetByJobIdAndDepartmentId(string jobId, Guid departmentId);
        List<DayAssign> Find(Expression<Func<DayAssign, bool>> filter);
        List<IDayAssign> GetDayAssignsForMemberByStatuses(Guid memberId, IEnumerable<JobStatus> statuses, int? daysAhead = null, IEnumerable<Guid> departments = null);
        List<Guid> GetDayAssignIds(ITaskDataFilterModel filter);
        Expression<Func<DayAssign, bool>> GetQuery(ITaskDataFilterModel filter);
        IEnumerable<DayAssign> GetList(IEnumerable<Guid> ids);
        IEnumerable<IDayAssign> GetDayAssignsForMembersByFilter(MemberDayAssignFilterModel filter);
        IEnumerable<IDayAssign> GetDayAssignsByJobId(string jobId);
        void UpdateSingleProperty<TProp>(Guid id, Expression<Func<DayAssign, TProp>> property, TProp value);
        bool HasTasks(IWeekPlanFilterModel filter);
        void UpdateEstimate(List<Guid> idList, int estimateMinutes);
        void UpdateTeam(List<Guid> idList, bool isAssignedToAllUser, Guid? groupId, Guid teamLeadId, List<Guid> userIdList);
    }
}