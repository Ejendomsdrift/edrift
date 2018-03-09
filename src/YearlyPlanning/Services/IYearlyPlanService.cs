using System;
using System.Collections.Generic;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public interface IYearlyPlanService
    {
        DepartmentYearPlanViewModel GetYearPlanCategories();

        IDictionary<string, List<YearPlanWeekData>> GetYearPlanWeekData(Guid departmentId, int year);

        IEnumerable<YearPlanItemViewModel> GetYearPlanDepartments(string taskId, int year);

        HousingDepartmentYearPlanModel GetAllData(Guid departmentId, int year);
    }
}
