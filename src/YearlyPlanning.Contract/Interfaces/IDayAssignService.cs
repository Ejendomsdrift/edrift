using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IDayAssignService
    {
        Task<Guid> CreateDayAssign(INewDayAssignModel model);

        Task<Guid> CreateDayAssignWithEstimate(INewDayAssignModel model);

        Task<Guid> ChangeDayAssignDate(INewDayAssignModel model);

        Task<IChangeStatusModel> AssignsMembersGroup(INewDayAssignModel model);

        List<IDayAssign> GetDayAssignsForCurrentUserHousingDepartments(IEnumerable<Guid> departmentIds = null);

        Task AssignJob(Guid dayAssignId);

        Task UnassignJob(Guid dayAssignId, string changeStatusComment, List<IMemberSpentTimeModel> members, JobStatus newJobStatus, Guid? selectedCancellingId, Guid[] uploadedFileIds);

        Task ReopenJob(Guid dayAssignId);

        bool IsAllowChangeDayAssignStatus(IDayAssign dayAssign);

        IDayAssign GetDayAssignById(Guid dayAssignId);

        Task<IDayAssign> GetByJobId(string jobId);

        Task<Guid> CancelJob(INewDayAssignModel model);

        IEnumerable<IDayAssign> GetForStatisticByIdsWithRestrictions(List<Guid> ids, IEnumerable<Guid> housingDepartmentsIds = null);

        List<IDayAssign> GetForStatisticTimeSpan(DateTime? startDate, DateTime? endDate, IEnumerable<JobStatus> allowedJobStatusList = null, IEnumerable<Guid> housingDepartmentsIds = null);

        List<IDayAssign> GetDayAssigns(Guid jobAssignId, string jobId, Guid departmentId, int weekNumber);

        void UpdateDayAssignEstimate(List<Guid> jobAssignIdList, string jobId, Guid departmentId, int estimateMinutes);

        void UpdateDayAssignTeam(List<Guid> jobAssignIdList, string jobId, Guid departmentId, bool isAssignedToAllUser, Guid? groupId, Guid teamLeadId, List<Guid> userIdList);

        List<IDayAssign> GetForStatisticTenantTask(DateTime startDate, DateTime endDate, IEnumerable<JobStatus> jobStatusList, IEnumerable<Guid> housingDepartmentIds);

        IEnumerable<int> GetWeeksWithExpiredTasks(Guid jobAssignId);

        IEnumerable<Guid> GetDayAssignIds(ITaskDataFilterModel filter);

        IEnumerable<IDayAssign> GetByIds(IEnumerable<Guid> ids);

        IEnumerable<IDayAssign> GetByJobIds(IEnumerable<string> jobIds);

        List<IDayAssign> GetListForCurrentUser(IEnumerable<Guid> housingDepartmentIds, IEnumerable<Guid> groupIds, IEnumerable<JobStatus> statuses);

        int GetPreviousNotEmptyWeekNumber(IWeekPlanFilterModel filter);
    }
}
