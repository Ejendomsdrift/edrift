using AutoMapper;
using CategoryCore.Contract.Interfaces;
using CategoryCore.Models;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using ManagementDepartmentCore.Models;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using MemberCore.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Tests
{
    public class YearlyPlanServiceTestsContext
    {
        protected List<DayAssign> DayAssigns;
        protected List<ICategoryModel> CategoriesTree;
        protected List<Job> Jobs;
        protected Job Job;
        protected Job Job2;
        protected ManagementDepartment ManagementDepartment;
        protected IEnumerable<IManagementDepartmentModel> ManagementDepartmentListModel;
        protected IMemberModel CurrentUser;

        [SetUp]
        public void SetUp()
        {
            var housingDepartmentId = Guid.NewGuid();
            var housingDepartment = FillHousingDepartment(housingDepartmentId);
            FillCategoriesTree();
            FillJob(housingDepartmentId);
            FillDayAssigns();
            FillManagementDepartment(housingDepartment);
            FillManagementDepartmentListModel();
            FillCurrentMember();


            var profiles = typeof(IMapProfile).GetInheritedClasses();
            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });
        }



        private void FillCategoriesTree()
        {
            var parent = new CategoryModel()
            {
                Id = Guid.NewGuid(),
                Name = "parent",
                ParentId = null
            };

            var child1 = new CategoryModel()
            {
                Id = Guid.NewGuid(),
                Parent = parent,
                ParentId = parent.ParentId,
                Name = "child1"
            };

            var child2 = new CategoryModel()
            {
                Id = Guid.NewGuid(),
                Parent = parent,
                ParentId = parent.ParentId,
                Name = "child2"
            };

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            CategoriesTree = new List<ICategoryModel>() { parent };
        }

        private void FillDayAssigns()
        {
            DayAssigns = new List<DayAssign>()
            {
                new DayAssign()
                {
                    Id = Guid.NewGuid(),
                    JobId = "JobId1"
                }
            };
        }

        private void FillManagementDepartment(HousingDepartment housingDepartment)
        {
            ManagementDepartment = new ManagementDepartment()
            {
                Id = Guid.NewGuid(),
                Name = "management1",
                HousingDepartmentList = new List<HousingDepartment>()
                {
                    housingDepartment,
                    new HousingDepartment()
                    {
                        Id = Guid.NewGuid(),
                        Name = "housing12"
                    }
                }
            };
        }

        private void FillManagementDepartmentListModel()
        {
            ManagementDepartmentListModel = new List<ManagementDepartmentModel>
            {
                new ManagementDepartmentModel
                {
                    Id = Guid.NewGuid(),
                    Name = "FirstMD",
                    HousingDepartmentList = new List<IHousingDepartmentModel>
                    {
                        new HousingDepartmentModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "housing1"
                        },
                        new HousingDepartmentModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "housing2"
                        },
                        new HousingDepartmentModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "housing3"
                        }
                    }
                },
                new ManagementDepartmentModel
                {
                    Id = Guid.NewGuid(),
                    Name = "SecondMD",
                    HousingDepartmentList = new List<IHousingDepartmentModel>
                    {
                        new HousingDepartmentModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "housing4"
                        }
                    }
                }
            };
        }

        private HousingDepartment FillHousingDepartment(Guid housingDepartmentId)
        {
            return new HousingDepartment()
            {
                Id = housingDepartmentId,
                Name = "housing11"
            };
        }

        private void FillJob(Guid housingDepartmentId)
        {
            var categoryId = CategoriesTree.First().Children.First().Id;

            Job = new Job()
            {
                Id = "JobId1",
                CategoryId = categoryId,
                Assigns = new List<JobAssign>()
                {
                    new JobAssign()
                    {
                        Id = Guid.NewGuid(),
                        HousingDepartmentIdList = new List<Guid>() {housingDepartmentId},
                        WeekList = new List<WeekModel>()
                        {
                            new WeekModel() {ChangedBy = WeekChangedBy.Coordinator, Number = 2}
                        }
                    }
                }
            };
            Job2 = new Job()
            {
                Id = "JobId2",
                CategoryId = categoryId,
                Assigns = new List<JobAssign>()
                {
                    new JobAssign()
                    {
                        Id = Guid.NewGuid(),
                        HousingDepartmentIdList = new List<Guid>() {housingDepartmentId},
                        WeekList = new List<WeekModel>()
                        {
                            new WeekModel() {ChangedBy = WeekChangedBy.Coordinator, Number = 2}
                        }
                    }
                },
                IsHidden = true
            };
            Jobs = new List<Job>() { Job, Job2 };
        }

        private void FillCurrentMember()
        {
            CurrentUser = new MemberModel()
            {
                MemberId = Guid.NewGuid(),
                CurrentRole = RoleType.Coordinator,
                LazyManagementsToActiveRolesRelation = new Lazy<IDictionary<RoleType, IEnumerable<Guid>>>(() =>
                {
                    return new Dictionary<RoleType, IEnumerable<Guid>>() { { RoleType.Coordinator, new[] { ManagementDepartment.Id } } };
                })
            };
        }

        protected Mock<IDayAssignProvider> BuildDayAssignProvider()
        {
            var mock = new Mock<IDayAssignProvider>();
            mock.SetupGet(p => p.Query)
                .Returns(DayAssigns.AsQueryable());
            return mock;
        }

        protected Mock<IManagementDepartmentService> BuildManagementDepartmentService()
        {
            var mock = new Mock<IManagementDepartmentService>();

            mock.Setup(p => p.GetHousingDepartments(It.IsAny<Guid>()))
                .Returns<Guid>(id =>
                    id == ManagementDepartment.Id
                        ? ManagementDepartment.HousingDepartmentList.Select(h => MapHousingDepartment(h, id))
                        : Enumerable.Empty<IHousingDepartmentModel>());

            mock.Setup(p => p.GetAllHousingDepartments())
                .Returns(ManagementDepartment.HousingDepartmentList
                    .Select(h => MapHousingDepartment(h, ManagementDepartment.Id)));

            mock.Setup(p => p.GetHousingDepartmentsByManagementIds(It.IsAny<IEnumerable<Guid>>()))
                .Returns<IEnumerable<Guid>>(managementDepartmentIds =>
                {
                    return managementDepartmentIds.FirstOrDefault(m => m == ManagementDepartment.Id) != null
                        ? ManagementDepartment.HousingDepartmentList.Select(
                            h => MapHousingDepartment(h, ManagementDepartment.Id))
                        : Enumerable.Empty<IHousingDepartmentModel>();
                });
            return mock;
        }

        protected Mock<ICategoryService> BuildCategoryService()
        {
            var mock = new Mock<ICategoryService>();

            mock.Setup(s => s.GetTree())
                .Returns(CategoriesTree);
            return mock;
        }

        protected Mock<IJobAssignProvider> BuildJobAssignProvider()
        {
            var mock = new Mock<IJobAssignProvider>();
            mock.Setup(a => a.GetByHousingDepartmentForYear(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns<Guid, int>((id, year) =>
                    Jobs.SelectMany(job => job.Assigns)
                        .Where(assign => assign.HousingDepartmentIdList.Contains(id)).ToList());
            return mock;
        }

        protected Mock<IJobProvider> BuildJobProvider()
        {
            var mock = new Mock<IJobProvider>();

            mock.Setup(p => p.GetForManagementDepartment(It.IsAny<string>(), It.IsAny<Guid?>()))
                .Returns<string, Guid?>((jobId, managementId) => Job);

            mock.Setup(p => p.GetByCategoryIdsForCoordinator(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IMemberModel>(), false))
                .Returns<IEnumerable<Guid>, IMemberModel>((categoryIds, user) =>
                Jobs.Where(job => categoryIds.Contains(job.CategoryId)).ToList());

            return mock;
        }

        protected Mock<IMemberService> BuildMemberService(bool isEmptyHousingDepartmentList = false)
        {
            var mock = new Mock<IMemberService>();

            mock.Setup(p => p.GetCurrentUser()).Returns(CurrentUser);
            if (isEmptyHousingDepartmentList)
            {
                mock.Setup(p => p.GetUserManagementDepartments(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(Enumerable.Empty<IManagementDepartmentModel>());
            }
            else
            {
                mock.Setup(p => p.GetUserManagementDepartments(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(ManagementDepartmentListModel);
            }

            return mock;
        }

        private IHousingDepartmentModel MapHousingDepartment(HousingDepartment data, Guid managementId)
        {
            var model = data.Map<IHousingDepartmentModel>();
            model.ManagementDepartmentId = managementId;
            return model;
        }
    }
}