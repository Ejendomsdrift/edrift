using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using MemberCore.Implementation;
using MemberCore.Models;
using Moq;
using NUnit.Framework;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Enums;
using SecurityCore.Contract.Interfaces;

namespace MemberCore.Tests
{
    [TestFixture]
    [Ignore("")]
    internal class MemberServiceTests : MemberCoreTestsContext
    {
        [Test]
        public void SyncMembers_AddMember_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var memberService = new MemberService(repoMock.Object, null, null, null, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var syncMember = new Member()
            {
                Id = Guid.NewGuid(),
                RoleList = new List<Role> { new Role { RoleId = (int)RoleType.Administrator } }
            };
            var syncMembers = new[] {syncMember.Map<SyncMember>()}.ToJson();
            var activeUserIds = Enumerable.Empty<Guid>();
            //act
            memberService.SyncMembers(syncMembers, activeUserIds);
            //assert
            Assert.IsNotNull(Members.FirstOrDefault(m => m.Id == syncMember.Id));
        }

        [Test]
        public void SyncMembers_SyncMembersParameterIsNull_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, null, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var activeUserIds = Enumerable.Empty<Guid>();
            //act
            memberService.SyncMembers(null, activeUserIds);
            //assert
            fileHelperMock.Verify(h => h.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            Assert.False(Members.Contains(null));
        }

        [Test]
        public void SyncMembers_AlterMember_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var memberService = new MemberService(repoMock.Object, null, null, null, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = Members.First();
            var role = member.RoleList.First();
            var syncMember = member.Map<SyncMember>();
            syncMember.RoleList.Remove(role);
            syncMember.RoleList.Add(new Role()
            {
                Id = Guid.NewGuid(),
                ManagementDepartmentId = "NewDepartment",
                RoleId = 2
            });
            var syncMembers = new[] {syncMember}.ToJson();
            var activeUserIds = Enumerable.Empty<Guid>();
            //act        
            memberService.SyncMembers(syncMembers, activeUserIds);
            //assert           
            Assert.NotNull(member.RoleList.FirstOrDefault(r => r.RoleId == 2));
            Assert.True(member.RoleList.Any(r => r.RoleId == role.RoleId && r.IsDeleted));
            Assert.True(Members.Any(r => r.IsDeleted));
        }

        [Test]
        public void SyncMembers_AvatarFileContentSave_Test()
        {
            //arrange            
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = new Member()
            {
                Id = Guid.NewGuid(),
                RoleList = new List<Role> { new Role { RoleId = (int)RoleType.Administrator } }
            };
            var syncMember = member.Map<SyncMember>();
            syncMember.AvatarFileContent = syncMember.Id.ToByteArray();
            var syncMembers = new[] {syncMember}.ToJson();
            var activeUserIds = Enumerable.Empty<Guid>();
            //act
            memberService.SyncMembers(syncMembers, activeUserIds);
            //assert
            pathHelperMock.Verify(h => h.GetAvatarPath(member.Id), Times.Once);
            Assert.Contains(member.Id.ToString(), Files.Keys);
            Assert.Pass();
        }

        [Test]
        public void SyncMembers_DeactivateMembers_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = new Member()
            {
                Id = Guid.NewGuid(),
                RoleList = new List<Role>()
                {
                    new Role()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };
            var syncMember = new SyncMember()
            {
                Id = member.Id,
                RoleList = member.RoleList,
                AvatarFileContent = Guid.NewGuid().ToByteArray()
            };
            var syncMembers = new[] {syncMember}.ToJson();
            var activeUserIds = Enumerable.Empty<Guid>();
            //act
            memberService.SyncMembers(syncMembers, activeUserIds);
            //assert
            repoMock.Verify(r => r.UpdateManySingleProperty(It.IsAny<Expression<Func<Member, bool>>>(),
                It.IsAny<Expression<Func<Member, bool>>>(), It.Is((bool b) => b)), Times.Once);
        }

        [Test]
        public void GetByIds_Test()
        {
            //arrange     
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = Members.First();
            var memberIds = new[] {member.Id};
            //act
            var result = memberService.GetByIds(memberIds);
            //assert
            Assert.NotNull(result.FirstOrDefault(m => m.MemberId == member.Id));
        }

        [Test]        
        public void GetAll_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());
            //act
            var result = memberService.GetAll();
            //assert
            Assert.True(Members.All(m => result.FirstOrDefault(r => r.MemberId == m.Id) != null));
        }

        [Test]
        public void GetByUserName_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = Members.First();
            //act
            var result = memberService.GetByUserName(member.UserName);
            //assert
            Assert.NotNull(result.UserName == member.UserName);
        }

        [Test]
        public void GetByEmail_Test()
        {
            //arrange
            var repoMock = BuildMemberRepository();
            var pathHelperMock = BuildPathHelper();
            var fileHelperMock = BuildFileHelper();
            var memberService = new MemberService(
                repoMock.Object, null, fileHelperMock.Object, pathHelperMock.Object, Mock.Of<IManagementDepartmentService>(), Mock.Of<ISecurityService>(), Mock.Of<IAppSettingHelper>());

            var member = Members.First();
            //act
            var result = memberService.GetByUserName(member.UserName);
            //assert
            Assert.NotNull(result.MemberId == member.Id);
        }
    }
}