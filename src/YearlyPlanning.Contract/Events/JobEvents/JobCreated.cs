using System;
using Infrastructure.EventSourcing.Implementation;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobEvents
{
    [BsonIgnoreExtraElements]
    public class JobCreated : EventBase
    {
        public string ParentId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public Guid CreatorId { get; set; }
        public List<JobAddress> AddressList { get; set; } 
        public List<RelationGroupModel> RelationGroupList { get; set; } 
        public RoleType CreatedByRole { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
