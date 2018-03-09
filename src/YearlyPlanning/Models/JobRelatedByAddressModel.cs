using System;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class JobRelatedByAddressModel: IJobRelatedByAddressModel
    {
        public string JobId { get; set; }

        public Guid DayAssignId { get; set; }

        public DateTime? Date { get; set; }

        public string Address { get; set; }

        public string Title { get; set; }
        
    }
}
