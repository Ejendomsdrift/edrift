using System;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class ChangeStatusModel: IChangeStatusModel
    {
        public bool IsSuccessful { get; set; }
        public Guid DayAssignId { get; set; }
    }
}
