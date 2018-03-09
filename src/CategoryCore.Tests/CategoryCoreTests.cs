using System.Linq;
using CategoryCore.Contract.Interfaces;
using CategoryCore.Implementation;
using CategoryCore.Models;
using Infrastructure.Helpers;
using Moq;
using NUnit.Framework;

namespace CategoryCore.Tests
{
    [TestFixture]
    [Ignore("")]
    public class CategoryCoreTests : CategoryCoreTestsContext
    {
        [Test]
        public void Save_Test()
        {
            //arrange
            var repoMock = BuildCategoryRepository();
            var settingsMock = BuildAppSettingsHelper();
            var categoryService = new CategoryService(repoMock.Object, settingsMock.Object);

            var newCategory = (ICategoryModel) new CategoryModel();
            //act
            categoryService.Save(newCategory);
            //assert
            Assert.NotNull(Categories.FirstOrDefault(c => c.Id == newCategory.Id));
        }
        
        [Test]
        public void GetByIds_Test()
        {
            //arrange
            var repoMock = BuildCategoryRepository();
            var settingsMock = BuildAppSettingsHelper();
            var categoryService = new CategoryService(repoMock.Object, settingsMock.Object);

            var categoriesIds = Categories.Select(c => c.Id);
            //act
            var result = categoryService.GetByIds(categoriesIds);
            //assert
            Assert.IsTrue(Categories.All(c => result.FirstOrDefault(r => r.Id == c.Id) != null));
        }

        [Test]
        public void Get_Test()
        {
            //arrange
            var repoMock = BuildCategoryRepository();
            var settingsMock = BuildAppSettingsHelper();
            var categoryService = new CategoryService(repoMock.Object, settingsMock.Object);

            var category = Categories.FirstOrDefault();
            //act
            var result = categoryService.Get(category.Id);
            //assert
            Assert.AreEqual(category.Id, result.Id);
        }

       [Test]
        public void Get_Tree_ShowHiddenTrue_Test()
        {
            //arrange
            var repoMock = BuildCategoryRepository();
            var settingsMock = BuildAppSettingsHelper();
            var categoryService = new CategoryService(repoMock.Object, settingsMock.Object);

            var category = Categories.FirstOrDefault(c => !c.ParentId.HasValue);
            //act
            var result = categoryService.GetTree();
            //assert
            Assert.AreEqual(1, result.Count());
           Assert.AreEqual(2, result.FirstOrDefault().Children.Count);
        }

        [Test]
        public void Get_Tree_ShowHiddenFalse_Test()
        {
            //arrange
            var repoMock = BuildCategoryRepository();
            var settingsMock = BuildAppSettingsHelper();
            var categoryService = new CategoryService(repoMock.Object, settingsMock.Object);

            var category = Categories.FirstOrDefault(c => !c.ParentId.HasValue);
            //act
            var result = categoryService.GetTree();
            //assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.FirstOrDefault().Children.Count);
        }
    }
}