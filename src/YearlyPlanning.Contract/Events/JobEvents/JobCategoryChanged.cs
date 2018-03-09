using System;
using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobEvents
{
    public class JobCategoryChanged : EventBase
    {
        public Guid CategoryId { get; set; }
    }
}