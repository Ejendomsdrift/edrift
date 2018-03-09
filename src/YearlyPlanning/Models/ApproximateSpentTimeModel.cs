using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class ApproximateSpentTimeModel : IApproximateSpentTimeModel
    {
        public int ApproximateSpentHours { get; set; }
        public int ApproximateSpentMinutes { get; set; }
        public int TotalSpentHours { get; set; }
        public int TotalSpentMinutes { get; set; }
    }
}
