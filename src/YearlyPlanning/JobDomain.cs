using System;
using Infrastructure.EventSourcing.Implementation;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Events.JobEvents;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning
{
    [BsonIgnoreExtraElements]
    public class JobDomain : AggregateBase
    {
        public string ParentId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public bool IsHidden { get; set; }
        public JobTypeEnum JobTypeId { get; set; }
        public Guid CreatorId { get; set; }
        public List<JobAddress> AddressList { get; set; } = new List<JobAddress>();
        public List<RelationGroupModel> RelationGroupList { get; set; } = new List<RelationGroupModel>();
        public RoleType CreatedByRole { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public JobDomain()
        {
            RegisterTransition<JobCreated>(Apply);
            RegisterTransition<JobCategoryChanged>(Apply);
            RegisterTransition<JobTitleChanged>(Apply);
            RegisterTransition<JobVisibilityChanged>(Apply);
            RegisterTransition<JobAddressChanged>(Apply);
        }

        public JobDomain(
            string id, Guid categoryId, string title, JobTypeEnum jobTypeId, Guid creatorId, RoleType createdByRole, 
            List<JobAddress> addressList, List<RelationGroupModel> relationGroupList, string parentId) : this()
        {
            Id = id;

            RaiseEvent(new JobCreated
            {
                ParentId = parentId,
                CategoryId = categoryId,
                Title = title,
                JobTypeId = jobTypeId,
                CreatorId = creatorId,
                AddressList = addressList,
                RelationGroupList = relationGroupList,
                CreatedByRole = createdByRole
            });
        }

        private void Apply(JobCreated e)
        {
            Id = e.SourceId;
            ParentId = e.ParentId;
            CategoryId = e.CategoryId;
            Title = e.Title;
            JobTypeId = e.JobTypeId;
            CreatorId = e.CreatorId;
            AddressList = e.AddressList ?? new List<JobAddress>();
            RelationGroupList = e.RelationGroupList ?? new List<RelationGroupModel>();
            CreatedByRole = e.CreatedByRole;
        }

        public void ChangeCategory(Guid categoryId)
        {
            RaiseEvent(new JobCategoryChanged { CategoryId = categoryId });
        }

        public void ChangeAddress(Guid housingDepartmentId, string address)
        {
            var addressItem = AddressList.FirstOrDefault(x => x.HousingDepartmentId == housingDepartmentId);
            if (addressItem == null)
            {
                addressItem = new JobAddress
                {
                    HousingDepartmentId = housingDepartmentId,
                    Address = address
                };

                AddressList.Add(addressItem);
            }
            else
            {
                addressItem.Address = address;
            }

            RaiseEvent(new JobAddressChanged { AddressList = AddressList });
        }

        private void Apply(JobCategoryChanged e)
        {
            CategoryId = e.CategoryId;
        }

        private void Apply(JobAddressChanged e)
        {
            AddressList = e.AddressList;
        }

        public void ChangeTitle(string title)
        {
            RaiseEvent(new JobTitleChanged { Title = title });
        }

        private void Apply(JobTitleChanged e)
        {
            Title = e.Title;
        }

        public void ChangeVisibility(bool isHidden)
        {
            RaiseEvent(new JobVisibilityChanged { IsHidden = isHidden });
        }

        private void Apply(JobVisibilityChanged e)
        {
            IsHidden = e.IsHidden;
        }

        public static JobDomain Create(
            string id, Guid categoryId, string title, JobTypeEnum jobTypeId, Guid creatorId, RoleType createdByRole, 
            List<JobAddress> addressList, List<RelationGroupModel> relationGroupList, string parentId)
        {
            return new JobDomain(id, categoryId, title, jobTypeId, creatorId, createdByRole, addressList, relationGroupList, parentId);
        }
    }
}
