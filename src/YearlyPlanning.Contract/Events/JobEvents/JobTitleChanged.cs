using Infrastructure.EventSourcing.Implementation;

namespace YearlyPlanning.Contract.Events.JobEvents
{
    public class JobTitleChanged : EventBase
    {
        public string Title { get; set; }
    }
}