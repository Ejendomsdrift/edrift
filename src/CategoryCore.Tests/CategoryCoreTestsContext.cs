using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using CategoryCore.Models;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using MongoRepository.Contract.Interfaces;
using Moq;
using NUnit.Framework;

namespace CategoryCore.Tests
{
    public class CategoryCoreTestsContext
    {
        protected List<Category> Categories;

        [SetUp]
        public void SetUp()
        {
            var category = new Category()
            {
                Color = "color3",
                Name = "ctegory3",
                Visible = true
            };
            Categories = new List<Category>()
            {
                category,
                new Category()
                {
                    Color = "color1",
                    Name = "category1",
                    ParentId = category.Id
                },
                new Category()
                {
                    Color = "color2",
                    Name = "ctegory2",
                    ParentId = category.Id,
                    Visible = true
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

        protected Mock<IAppSettingHelper> BuildAppSettingsHelper()
        {
            var moqRepo = new Mock<IAppSettingHelper>();
            return moqRepo;
        }


        protected Mock<IRepository<Category>> BuildCategoryRepository()
        {
            var moqRepo = new Mock<IRepository<Category>>();

            moqRepo.Setup(r => r.Save(It.IsAny<Category>()))
                .Callback<Category>(m =>
                {
                    var entry = Categories.FirstOrDefault(e => e.Id == m.Id);
                    if (entry != null)
                    {
                        entry.Color = m.Color;
                        entry.ParentId = m.ParentId;
                        entry.Visible = m.Visible;
                        entry.Name = m.Name;
                    }
                    else Categories.Add(m);
                }).Verifiable();

            moqRepo.Setup(
                    r =>
                        r.FindOne(
                            It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns<Expression<Func<Category, bool>>>(
                    f => Categories.FirstOrDefault(f.Compile())).Verifiable();

            moqRepo.Setup(
                    r =>
                        r.Find(
                            It.IsAny<Expression<Func<Category, bool>>>(),
                            It.IsAny<IQueryOptions<Category>>()))
                .Returns
                <Expression<Func<Category, bool>>,
                    IQueryOptions<Category>>(
                    (fun, filter) => Categories.Where(fun.Compile()));
            moqRepo.Setup(r => r.GetAll()).Returns(Categories);
            return moqRepo;
        }
    }
}