using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;

namespace Web.Models
{
    public class WeekTaskViewModel
    {
        public List<Job> FacilityTasks { get; set; }
        public List<IOperationalTaskModel> OperationalTasks { get; set; }
    }
}
