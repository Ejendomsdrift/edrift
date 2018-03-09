using System.Collections.Generic;
using Infrastructure.Models;

namespace Web.Models
{
    public class YearWeeksViewModel
    {
        public YearWeeksViewModel()
        {
            MonthWeeks = new List<MonthWeeksModel>();
        }

        public int Year { get; set; }

        public List<MonthWeeksModel> MonthWeeks { get; set; }
    }
}