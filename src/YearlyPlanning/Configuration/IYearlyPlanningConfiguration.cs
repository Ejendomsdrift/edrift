namespace YearlyPlanning.Configuration
{
    public interface IYearlyPlanningConfiguration
    {
        string ConnectionString { get; }

        string DatabaseName { get; }
    }
}
