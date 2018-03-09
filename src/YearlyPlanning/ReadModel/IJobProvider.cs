using MemberCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.ReadModel
{
    public interface IJobProvider
    {
        IQueryable<Job> Query { get; }

        Task<Job> Get(string id);

        Job GetForManagementDepartment(string id, Guid? managementDepartmentId);

        Job GetForHousingDepartment(string id, Guid departmentId);

        List<Job> GetByCategoryId(Guid categoryId);

        List<Job> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber, bool fillDayAssigns = true, bool fillCategories = true);

        List<Job> GetByYearWeekForAllDepartments(int year, int weekNumber, bool fillDayAssigns = true, bool fillCategories = true);

        List<Job> GetByIds(IEnumerable<string> ids, bool fillCategories = false);

        List<Job> GetHiddenByIds(IEnumerable<string> ids);

        List<Job> GetByIdsWithAssigns(IEnumerable<string> ids, Guid departmentId, int year);

        List<Job> Get(IEnumerable<Guid> categoryIds, bool includeGroupedTasks = true, bool includeHiddenTasks = true, bool onlyFacilityTask = false);

        List<Job> Get(IEnumerable<Guid> categoryIds, IEnumerable<Guid> housingDepartments, bool includeGroupedTasks = true, bool includeHiddenTasks = true);

        List<Job> GetByCategoryIdsForCoordinator(IEnumerable<Guid> categoryIds, IMemberModel currentUser, bool onlyFacilityTask = false);

        List<Job> GetByCategoryIdsWithAssigns(IEnumerable<Guid> categoryIds, Guid departmentId);

        long ClearData();

        void SaveRelationGroupId(string id, RelationGroupModel model);

        List<Job> GetJobsByJobType(JobTypeEnum jobTypeId);

        void UpdateSingleProperty<TProp>(string id, Expression<Func<Job, TProp>> property, TProp value);

        List<Job> GetByParentId(string parentId);
    }
}