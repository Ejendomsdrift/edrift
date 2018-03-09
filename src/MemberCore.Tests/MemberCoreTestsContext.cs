using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using MemberCore.Models;
using MongoRepository.Contract.Interfaces;
using Moq;
using NUnit.Framework;

namespace MemberCore.Tests
{
    public class MemberCoreTestsContext
    {
        protected List<Member> Members { get; set; }
        protected Dictionary<string, byte[]> Files { get; set; }

        [SetUp]
        public void SetUp()
        {
            Members = new List<Member>()
            {
                new Member()
                {
                    Id = Guid.NewGuid(),
                    Name = "memberName1",
                    UserName = "memberUserName1",
                    Email = "member1@test.dk",
                    IsDeleted = false,
                    RoleList = new List<Role>()
                    {
                        new Role()
                        {
                            Id = Guid.NewGuid(),
                            IsDeleted = false,
                            ManagementDepartmentId = "managementDepartment1",
                            RoleId = 1
                        }
                    }
                },
                new Member()
                {
                    Id = Guid.NewGuid(),
                    Name = "memberName2",
                    UserName = "memberUserName2",
                    Email = "member2@test.dk",
                    IsDeleted = false,
                    RoleList = new List<Role>()
                    {
                        new Role()
                        {
                            Id = Guid.NewGuid(),
                            IsDeleted = false,
                            ManagementDepartmentId = "managementDepartment2",
                            RoleId = 2
                        }
                    }
                }
            };
            Files = Members.ToDictionary(m => m.Id.ToString(), m => m.Id.ToByteArray());
            var profiles = typeof(IMapProfile).GetInheritedClasses();
            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });
        }

        protected Mock<IRepository<Member>> BuildMemberRepository()
        {
            var moqRepo = new Mock<IRepository<Member>>();

            moqRepo.Setup(r => r.Save(It.IsAny<Member>())).Callback<Member>(m =>
            {
                var entry = Members.FirstOrDefault(e => e.Id == m.Id);
                if (entry != null)
                {
                    entry.IsDeleted = m.IsDeleted;
                    entry.RoleList = m.RoleList;
                    entry.UserName = m.UserName;
                    entry.Name = m.Name;
                }
                else Members.Add(m);
            }).Verifiable();

            moqRepo.Setup(r => r.FindOne(It.IsAny<Expression<Func<Member, bool>>>()))
                .Returns<Expression<Func<Member, bool>>>(f => Members.FirstOrDefault(f.Compile())).Verifiable();

            moqRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<IQueryOptions<Member>>()))
                .Returns<Expression<Func<Member, bool>>, IQueryOptions<Member>>(
                    (fun, filter) => Members.Where(fun.Compile()));
            moqRepo.Setup(r => r.GetAll()).Returns(Members);

            moqRepo.Setup(
                    r =>
                        r.UpdateManySingleProperty(It.IsAny<Expression<Func<Member, bool>>>(),
                            It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<bool>()))
                .Callback<Expression<Func<Member, bool>>, Expression<Func<Member, bool>>, bool>(
                    (f, p, v) =>
                    {
                        var filteredAndMappedMembers = Members.Where(f.Compile());
                        foreach (var m in filteredAndMappedMembers)
                        {
                            m.IsDeleted = v;
                        }
                    }).Verifiable();

            return moqRepo;
        }

        protected Mock<IFileHelper> BuildFileHelper()
        {
            var fileHelper = new Mock<IFileHelper>();
            fileHelper.Setup(h => h.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((p, c) => Files.Add(p, c))
                .Verifiable();
            return fileHelper;
        }

        protected Mock<IPathHelper> BuildPathHelper()
        {
            var urlHalper = new Mock<IPathHelper>();
            urlHalper.Setup(h => h.GetAvatarPath(It.IsAny<Guid>()))
                .Returns<Guid>(id => id.ToString()).Verifiable();
            return urlHalper;
        }
    }
}