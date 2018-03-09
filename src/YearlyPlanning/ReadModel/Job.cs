using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using System.Linq;
using Infrastructure.Extensions;

namespace YearlyPlanning.ReadModel
{
    public class Job: IJob
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public Guid CategoryId { get; set; }

        public string Title { get; set; }

        public bool IsHidden { get; set; }

        public Guid CreatorId { get; set; }

        public DateTime CreationDate { get; set; }

        public JobTypeEnum JobTypeId { get; set; }

        public List<JobAddress> AddressList { get; set; } = new List<JobAddress>();

        public Guid? RelationGroupId { get; set; }

        public List<RelationGroupModel> RelationGroupList { get; set; } = new List<RelationGroupModel>();

        public List<JobAssign> Assigns { get; set; } = new List<JobAssign>();

        public List<IDayAssign> DayAssigns { get; set; } = new List<IDayAssign>();

        public ICategoryModel Category { get; set; }

        public RoleType CreatedByRole { get; set; }

        public string FirstAddress
        {
            get { return AddressList.HasValue() ? AddressList.First().Address : null; }
        }

        public string ParentAddress { get; set; }

        public string GetAddress(Guid departmentId)
        {
            if (!AddressList.HasValue())
            {
                return null;
            }

            var address = AddressList.FirstOrDefault(i => i.HousingDepartmentId == departmentId)?.Address;
            return address;
        }
    }
}