using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementDepartmentCore.Contract.Interfaces;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJobService
    {
        List<JobAssign> GetAllByJobId(string jobId, Guid? managementDepartmentId = null);
        IFormattedJobAssign GetAllByJobIdFormatted(string jobId, Guid? managementDepartmentId = null);
        IEnumerable<IHousingDepartmentModel> GetAssignedDepartments(string jobId);
        JobAssign GetJobAssignById(Guid id);
        Task<IJob> GetJobById(string id);
        JobAssign GetJobAsignByJobIdAndAssignedDepartment(string jobId, Guid assignedDepartmentId, bool isGlobal);
        IJobDetailsModel GetJobDetails(Guid dayAssignId);
        Guid GetJobAssignId(string jobId, Guid departmentId);
        IEnumerable<Guid> GetUserIdsFromActiveJobs();
        IEnumerable<IJobHeaderModel> GetOpenedJobsHeaderList();
        IEnumerable<IJobHeaderModel> GetMyJobsHeaderList();
        IEnumerable<IJobHeaderModel> GetCompletedJobsHeaderList();
        IJobCounterModel GetJobCounters();
        bool IsAllowedTaskGrouping(string jobId);
        Task<string> AddTaskToRelationGroup(string jobId, Guid housingDepartmentId);
        bool IsGroupedTask(string jobId);
        bool IsChildGroupedTask(string jobId);
        Task SaveTitle(IdValueModel<string, string> model);
        Task SaveCategory(IdValueModel<string, Guid> model);
        List<JobAssign> GetJobAssignList(IEnumerable<Guid> relationGroupIdList);
        IEnumerable<IdValueModel<string, string>> GetRelatedAddressListForHousingDepartment(IEnumerable<Guid> relationGroupIdList, Guid housingDepartmentId, bool isParent);
        Task<JobAssign> CreateOrGetJobAssign(Guid assignId, string jobId, Guid housingDepartmentId);
        Task CopyGlobalJobAssignToLocalIfLocalIntervalWasntChanged(Guid globalJobAssignId, string jobId, Guid housingDepartmentId);
        Task<string> CreateTaskRelationGroup(string jobId, Guid housingDepartmentId);
        IEnumerable<IHousingDepartmentModel> GetHousingDepartmentsForGroupingTasks(string jobId);
        IEnumerable<string> GetHousingDepartmentAddressesOrAllAddressesForUserManagementDepartment(Guid? departmentId);
        IEnumerable<IJob> GetJobsByJobType(JobTypeEnum jobTypeId);
        IEnumerable<IJob> GetTenantTasksByAddress(string address);
        IEnumerable<string> GetAddressesForManagementDepartment(Guid? managementDepartmentId);
        IEnumerable<IJob> GetByIds(IEnumerable<string> ids);
        void UpdateAddresses(string jobId, List<JobAddress> addresses);
        int GetPreviousNotEmptyWeekNumber(IWeekPlanFilterModel filter);
        IEnumerable<IJobRelatedByAddressModel> GetTenantJobsRelatedByAddress(string jobId);
        Dictionary<Guid, IApproximateSpentTimeModel> GetMembersApproximateSpentTimeList(Guid dayAssignId, DateTime currentDate);
        IEnumerable<IHousingDepartmentModel> GetGroupedAssignedDepartments(string jobId);
        JobAssign GetParentGlobalAssignForGroupedTask(IJob job);
        Task CopyParentGlobalJobAssignToChildren(Guid globalJobAssignId, string jobId);
        IEnumerable<IJob> GetJobsByRelationGroupIds(IEnumerable<Guid> relationGroupId);
        IEnumerable<IJob> GetChildJobsForHousingDepartment(string jobId, Guid housingDepartmentId);
        bool IsRelationListMatchJobAssignList(IJob job, IEnumerable<JobAssign> jobAssignList);
        bool CheckIsAllChildGroupedTaskHided(IJob job);
        bool IsPossibleToHideChildTask(string parentJobId, Guid housingDepartmentId);
        JobAssign GetGlobalJobAssignForGroupedTask(IJob job);
        void ChangeJobAssignEstimate(string jobId, int estimateInMinutes, Guid housingDepartmentId);
        int GetJobAssignEstimate(string jobId, Guid housingDepartmentId);
        void ChangeAssignTeam(string jobId, Guid housingDepartmentId, List<Guid> userIdList, bool isAssignedToAllUsers, Guid? groupId, Guid teamLeadId);
        Responsible GetAssignTeam(string jobId, Guid housingDepartmentId);
        void FillCorrectPathForJobAssignsUploads(List<JobAssign> jobAssigns);
        List<IJob> GetByCategoryIds(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true);
        IEnumerable<string> GetAllIdsByParentId(string parentJobId);
    }
}
