using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IJob
    {
        string Id { get; set; }

        string ParentId { get; set; }

        Guid CategoryId { get; set; }

        string Title { get; set; }

        bool IsHidden { get; set; }

        Guid CreatorId { get; set; }

        DateTime CreationDate { get; set; }

        JobTypeEnum JobTypeId { get; set; }

        Guid? RelationGroupId { get; set; }

        List<RelationGroupModel> RelationGroupList { get; set; }

        List<JobAssign> Assigns { get; set; }

        List<IDayAssign> DayAssigns { get; set; }

        ICategoryModel Category { get; set; }

        RoleType CreatedByRole { get; set; }

        List<JobAddress> AddressList { get; set; }

        string FirstAddress { get; }

        string ParentAddress { get; set; }

        string GetAddress(Guid departmentId);
    }
}
