using System;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public class CreateJobCommand : JobCommand
    {
        public string ParentId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public Guid CreatorId { get; set; }
        public List<JobAddress> AddressList { get; set; }
        public List<RelationGroupModel> RelationGroupList { get; set; }
        public RoleType CreatedByRole { get; set; }

        public CreateJobCommand(string id, Guid categoryId, string title, JobTypeEnum jobTypeId, Guid creatorId, RoleType createdByRole, List<JobAddress> addressList, List<RelationGroupModel> relationGroupList, string parentId) : base(id)
        {
            ParentId = parentId;
            CategoryId = categoryId;
            Title = title;
            JobTypeId = jobTypeId;
            CreatorId = creatorId;
            AddressList = addressList;
            RelationGroupList = relationGroupList;
            CreatedByRole = createdByRole;
        }
    }
}