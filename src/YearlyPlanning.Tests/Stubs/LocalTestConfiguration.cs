using YearlyPlanning.Configuration;

namespace YearlyPlanning.Tests.Stubs
{
    internal class LocalTestConfiguration : IYearlyPlanningConfiguration
    {
        public string ConnectionString { get; } = "mongodb://localhost:27017/";
        public string DatabaseName { get; } = "Edrift_MongoEventStore_Tests";
    }
}
