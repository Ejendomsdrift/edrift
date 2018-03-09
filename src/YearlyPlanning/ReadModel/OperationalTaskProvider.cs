using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryCore.Contract.Interfaces;
using MongoDB.Driver;
using YearlyPlanning.Configuration;
using YearlyPlanning.Models;

namespace YearlyPlanning.ReadModel
{
    public class OperationalTaskProvider : IOperationalTaskProvider
    {
        private readonly IMongoCollection<OperationalTaskModel> collection;
        private readonly ICategoryService categoryService;

        public OperationalTaskProvider(IYearlyPlanningConfiguration configuration, ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            var client = new MongoClient(configuration.ConnectionString);
            var database = client.GetDatabase(configuration.DatabaseName);
            collection = database.GetCollection<OperationalTaskModel>(nameof(OperationalTaskModel));
        }

        public Task<OperationalTaskModel> Get(string id)
        {
            using (var cursor = collection.FindSync(f => f.Id == id, new FindOptions<OperationalTaskModel> { Limit = 1 }))
            {
                return cursor.FirstOrDefaultAsync();
            }
        }

        public List<OperationalTaskModel> GetByDepartmentIdYearWeek(Guid departmentId, int year, int weekNumber)
        {
            using (var cursor = collection.FindSync(f => f.DepartmentId == departmentId && f.Year == year && f.Week == weekNumber))
            {
                var filteredResults = cursor.ToList();
                return FillCategories(filteredResults).ToList();
            }
        }

        private IEnumerable<OperationalTaskModel> FillCategories(IEnumerable<OperationalTaskModel> operationalTasks)
        {
            var categories = categoryService.GetByIds(operationalTasks.Select(f => f.CategoryId));
            foreach (var operationalTask in operationalTasks)
            {
                operationalTask.Category = categories.FirstOrDefault(c => c.Id == operationalTask.CategoryId);
            }
            return operationalTasks;
        }
    }
}
