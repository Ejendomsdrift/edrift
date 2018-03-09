using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IOperationalTaskService
    {
        Task<CreateOperationalTaskViewModel> CreateAdHocTask(IOperationalTaskModel model);
        Task<CreateOperationalTaskViewModel> CreateTenantTask(IOperationalTaskModel model);
        Task<CreateOperationalTaskViewModel> CreateOtherTask(IOperationalTaskModel model);
        Task ChangeTenantTaskType(Guid dayAssignId, TenantTaskTypeEnum type);
        Task ChangeTenantTaskUrgency(Guid dayAssignId, bool isUrgent);
        Task ChangeTaskDate(string jobId, Guid dayAssignId, DateTime date);
        Task ChangeTaskTime(Guid dayAssignId, DateTime time);
        Task ChangeCategory(IdValueModel<string, Guid> model);
        Task ChangeEstimate(IdValueModel<Guid, int> model);
        Task ChangeTitle(IdValueModel<string, string> model);
        Task ChangeResidentName(IdValueModel<Guid, string> model);
        Task ChangeResidentPhone(IdValueModel<Guid, string> model);
        Task ChangeTenantTaskComment(IdValueModel<Guid, string> model);
        Task ChangeDescription(IdValueModel<string, string> model);
        Task AssignEmployees(MemberAssignModel model);
        Task UnassignEmployees(MemberAssignModel model);
        Task<List<IOperationalTaskModel>> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber);
        Task<ITaskCreationInfo> GetTaskCreationInfo(string jobId);
        Task<IOperationalTaskModel> GetTask(Guid dayAssignId, JobTypeEnum type);
        IEnumerable<UploadFileModel> GetUploads(Guid jobAssignId);
    }
}
