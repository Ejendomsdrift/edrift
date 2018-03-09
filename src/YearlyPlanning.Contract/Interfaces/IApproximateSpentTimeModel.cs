namespace YearlyPlanning.Contract.Interfaces
{
    public interface IApproximateSpentTimeModel
    {
        int ApproximateSpentHours { get; set; }
        int ApproximateSpentMinutes { get; set; }
        int TotalSpentHours { get; set; }
        int TotalSpentMinutes { get; set; }
    }
}