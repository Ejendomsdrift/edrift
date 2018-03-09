using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Models
{
    public class WeekModel
    {
        public int Number { get; set; }

        public WeekChangedBy ChangedBy { get; set; }

        public bool IsDisabled { get; set; }
    }
}
