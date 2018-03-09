using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Models;
using MongoRepository.Contract.Interfaces;
using Moq;
using NUnit.Framework;

namespace ManagementDepartmentCore.Tests
{
    public class ManagmentDepartmentTestsContext
    {
        protected List<ManagementDepartment> Departments { get; set; }

        [SetUp]
        public void SetUp()
        {
            Departments = new List<ManagementDepartment>()
            {
                new ManagementDepartment()
                {
                    Id = Guid.NewGuid(),
                    Name = "DepartmentName1",
                    ManagementDepartmentRefId = "RefId1",
                    SyncDepartmentId = "SyncDepartmentId1",
                    HousingDepartmentList = new List<HousingDepartment>()
                    {
                        new HousingDepartment()
                        {
                            Id = new Guid(),
                            Name = "HousingDepartmentName1",
                            SyncDepartmentId = "SyncDepartmentId1"

                        }
                    }
                },
                 new ManagementDepartment()
                {
                    Id = Guid.NewGuid(),
                    Name = "DepartmentName2",
                    ManagementDepartmentRefId = "RefId2",
                    SyncDepartmentId = "SyncDepartmentId2",
                    HousingDepartmentList = new List<HousingDepartment>()
                    {
                        new HousingDepartment()
                        {
                            Id = new Guid(),
                            Name = "HousingDepartmentName2",
                            SyncDepartmentId = "SyncDepartmentId2"

                        }
                    }
                }
            };
            var profiles = typeof(IMapProfile).GetInheritedClasses();
            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });
        }

        public Mock<IRepository<ManagementDepartment>> BuildDepartmentsRepository()
        {
            var moqRepo = new Mock<IRepository<ManagementDepartment>>();

            moqRepo.Setup(r => r.Save(It.IsAny<ManagementDepartment>()))
                .Callback<ManagementDepartment>(m =>
                {
                    var entry = Departments.FirstOrDefault(e => e.Id == m.Id);
                    if (entry != null)
                    {
                        entry.IsDeleted = m.IsDeleted;
                        entry.HousingDepartmentList = m.HousingDepartmentList;
                        entry.ManagementDepartmentRefId = m.ManagementDepartmentRefId;
                        entry.SyncDepartmentId = m.SyncDepartmentId;
                        entry.Name = m.Name;
                    }
                    else Departments.Add(m);
                }).Verifiable();

            moqRepo.Setup(
                    r =>
                        r.FindOne(
                            It.IsAny<Expression<Func<ManagementDepartment, bool>>>()))
                .Returns<Expression<Func<ManagementDepartment, bool>>>(
                    f => Departments.FirstOrDefault(f.Compile())).Verifiable();

            moqRepo.Setup(
                    r =>
                        r.Find(
                            It.IsAny<Expression<Func<ManagementDepartment, bool>>>(),
                            It.IsAny<IQueryOptions<ManagementDepartment>>()))
                .Returns
                <Expression<Func<ManagementDepartment, bool>>,
                    IQueryOptions<ManagementDepartment>>(
                    (fun, filter) => Departments.Where(fun.Compile()));
            moqRepo.Setup(r => r.GetAll()).Returns(Departments);

            moqRepo.Setup(
                    r =>
                        r.UpdateManySingleProperty(
                            It.IsAny<Expression<Func<ManagementDepartment, bool>>>(),
                            It.IsAny<Expression<Func<ManagementDepartment, bool>>>(),
                            It.IsAny<bool>()))
                .Callback
                <Expression<Func<ManagementDepartment, bool>>,
                    Expression<Func<ManagementDepartment, bool>>, bool>(
                    (f, p, v) =>
                    {
                        var filteredAndMappedMembers = Departments.Where(f.Compile());
                        foreach (var m in filteredAndMappedMembers)
                        {
                            m.IsDeleted = v;
                        }
                    }).Verifiable();
            return moqRepo;
        }
    }
}