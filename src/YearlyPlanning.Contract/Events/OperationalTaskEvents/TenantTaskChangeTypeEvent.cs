using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Events.OperationalTaskEvents
{
    public class TenantTaskChangeTypeEvent : EventBase
    {
        public TenantTaskTypeEnum Type { get; set; }
    }
}
