using System;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Contract.Models
{
    public class TaskCreationInfo : ITaskCreationInfo
    {
        public Guid DayAssignId { get; set; }
        public string CreatorAvatar { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationDate { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public bool IsTaskCanceled { get; set; }
        public bool? IsUrgent { get; set; }
        public string Title { get; set; }
    }
}