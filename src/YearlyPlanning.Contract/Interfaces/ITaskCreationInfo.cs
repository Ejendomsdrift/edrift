using System;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface ITaskCreationInfo
    {
        Guid DayAssignId { get; set; }
        string CreatorAvatar { get; set; }
        string CreatorName { get; set; }
        DateTime CreationDate { get; set; }
        JobTypeEnum JobTypeId { get; set; }
        bool IsTaskCanceled { get; set; }
        bool? IsUrgent { get; set; }
        string Title { get; set; }
    }
}