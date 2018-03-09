using System;
using System.Collections.Generic;
using System.Linq;
using Groups.Models;
using GroupsContract.Interfaces;
using GroupsContract.Models;
using ManagementDepartmentCore.Contract.Interfaces;
using ManagementDepartmentCore.Models;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using MemberCore.Models;
using Moq;
using NUnit.Framework;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Tests
{
    public class TimeScheduleServiceTestsContext
    {
        protected List<IDayAssign> DayAssigns;
        protected List<IGroupModel> Groups;
        protected List<IMemberModel> Members;
        protected IManagementDepartmentModel ManagementDepartment;

        [SetUp]
        public void SetUp()
        {
            ManagementDepartment = new ManagementDepartmentModel()
            {
                
            };
            Members = new List<IMemberModel>()
            {
                new MemberModel()
                {
                    MemberId = Guid.NewGuid(),
                    Roles =  new List<RoleType>() { RoleType.Coordinator }
                },
                 new MemberModel()
                {
                    MemberId = Guid.NewGuid(),
                    Roles =  new List<RoleType>() { RoleType.Coordinator }
                }
            };

            var groupId = Guid.NewGuid();
            Groups = new List<IGroupModel>()
            {
                new GroupModel()
                {
                    Id = groupId,
                    MemberIds = Members.Select(m=>m.MemberId)
                }
            };

            DayAssigns = new List<IDayAssign>()
            {
                new DayAssign()
                {
                    Id = Guid.NewGuid(),
                    Date = new DateTime(2017, 1, 1),
                    WeekNumber = 1,
                    WeekDay = 1,
                    EstimatedMinutes = 30,
                    GroupId = groupId
                },
                new DayAssign()
                {
                    Id = Guid.NewGuid(),
                    Date = new DateTime(2017, 1, 1),
                    WeekNumber = 1,
                    WeekDay = 2,
                    EstimatedMinutes = 30,
                    GroupId = groupId
                }
            };
        }

        public Mock<IDayAssignProvider> BuildDayAssignProvider()
        {
            var mock = new Mock<IDayAssignProvider>();
            mock.Setup(r => r.GetDayAssignsForMember(It.IsAny<Guid>(), It.IsAny<bool>(), null, null))
                .Returns<Guid, int?, IEnumerable<Guid>>((memberId, daysAhead, departments) => DayAssigns)
                .Verifiable();
            return mock;
        }

        public Mock<IGroupService> BuildGroupService()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(r => r.Get(It.IsAny<Guid>()))
                .Returns<Guid>(id => Groups.First(group => group.Id == id))
                .Verifiable();
            return mock;
        }

        public Mock<IMemberService> BuildMemberService()
        {
            var mock = new Mock<IMemberService>();
            mock.Setup(s => s.GetAll())
                .Returns(Members)
                .Verifiable();
            return mock;
        }

        public Mock<IManagementDepartmentService> BuildManagementDepartmentService()
        {
            var mock = new Mock<IManagementDepartmentService>();
            mock.Setup(s => s.GetParentSyncDepartmentId(It.IsAny<Guid>()))
                .Verifiable();
            return mock;
        }
    }
}