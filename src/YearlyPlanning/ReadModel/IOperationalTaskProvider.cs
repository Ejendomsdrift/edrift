using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YearlyPlanning.Models;

namespace YearlyPlanning.ReadModel
{
    public interface IOperationalTaskProvider
    {
        Task<OperationalTaskModel> Get(string id);
        List<OperationalTaskModel> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber);
    }
}
