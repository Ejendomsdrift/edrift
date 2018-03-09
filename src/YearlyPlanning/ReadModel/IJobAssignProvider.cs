using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public interface IJobAssignProvider
    {
        IQueryable<JobAssign> Query { get; }
        JobAssign GetById(Guid id);
        JobAssign GetGlobalAssignByJobId(string id);
        JobAssign GetAssignByJobIdAndDepartmentId(string jobId, Guid departmentId);
        JobAssign GetAssignByFilters(string jobId, Guid departmentId);
        JobAssign GetByJobId(string jobId);
        List<JobAssign> GetAllByJobId(string jobId, bool showWithDisabledAssigns = false);
        List<JobAssign> GetByJobsDepartments(IEnumerable<string> jobIds, List<Guid> departmentIdList, int? year = null);
        List<JobAssign> GetAllJobAssignByFilters(IEnumerable<string> jobIds, IEnumerable<Guid> departmentIds, int? year = null);
        List<JobAssign> GetByJobsDepartmentsWithGlobal(IEnumerable<string> jobIds, List<Guid> departmentIdList);
        List<JobAssign> GetForWeek(Guid departmentId, int year, int weekNumber, IEnumerable<Guid> jobAssignIds);
        List<JobAssign> GetByHousingDepartmentForYear(Guid housingDepartmentId, int year);
        List<JobAssign> GetForWeekForAllDepartments(int year, int weekNumber, IEnumerable<Guid> jobAssignIds);
        List<JobAssign> GetByDepartmentWeekAndYear(Guid departmentId, int year, int weekNumber);
        List<JobAssign> GetAllByJobAssignIdList(IEnumerable<Guid> jobAssignIdList);
        List<JobAssign> GetByJobIds(IEnumerable<string> jobIds);
        List<Guid> ChangeEstimate(string jobId, int estimateInMinutes, Guid housingDepartmentId);
        int GetEstimate(string jobId, Guid housingDepartmentId);
        List<Guid> ChangeAssignTeam(string jobId, Guid housingDepartmentId, List<Guid> userIdList, bool isAssignedToAllUsers, Guid? groupId, Guid teamLeadId);
        Responsible GetAssignTeam(string jobId, Guid housingDepartmentId);
    }
}