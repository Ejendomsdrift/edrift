using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.ReadModel
{
    public  class YearPlanWeekData
    {
        public int WeekNumber { get; set; }
        public WeekChangedBy ChangedBy { get; set; }
        public YearTaskStatus Status { get; set; }
        public bool IsDisabled { get; set; }
    }
}
