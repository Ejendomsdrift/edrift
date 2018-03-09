using System.Collections.Generic;

namespace Web.Models
{
    public class TotalWeeksViewModel
    {
        public int CurrentWeek { get; set; }

        public IEnumerable<int> TotalWeeks { get; set; }
    }
}