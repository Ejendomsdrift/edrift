using System;
using System.Linq;
using Infrastructure.Constants;
using NUnit.Framework;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Services;

namespace YearlyPlanning.Tests
{
    [TestFixture]
    public class YearlyPlanServiceTests : YearlyPlanServiceTestsContext
    {
        [Test]
        public void GetYearPlanDepartments_ManagementDepartmentWithoutHousingDepartments_Test()
        {
            //arrange
            var categoryService = BuildCategoryService();
            var dayAssignProvider = BuildDayAssignProvider();
            var jobProvider = BuildJobProvider();
            var jobAssignProvider = BuildJobAssignProvider();
            var memberService = BuildMemberService(true);

            var yearlyPlanService = new YearlyPlanService(jobProvider.Object, categoryService.Object, dayAssignProvider.Object, memberService.Object, jobAssignProvider.Object);

            var year = 2017;
            //act
            var result = yearlyPlanService.GetYearPlanDepartments(Job.Id, year);
            //assert
            Assert.IsEmpty(result);
        }

        [Ignore("")]
        [Test]
        public void GetYearPlanDepartments_TaskIdIsNull_Test()
        {
            //arrange
            var categoryService = BuildCategoryService();
            var dayAssignProvider = BuildDayAssignProvider();
            var jobProvider = BuildJobProvider();
            var jobAssignProvider = BuildJobAssignProvider();
            var memberService = BuildMemberService();

            var yearlyPlanService = new YearlyPlanService(jobProvider.Object, categoryService.Object, dayAssignProvider.Object,
                memberService.Object, jobAssignProvider.Object);

            var year = 2017;
            //act
            var result = yearlyPlanService.GetYearPlanDepartments(null, year).ToList();
            //assert
            Assert.AreEqual(ManagementDepartmentListModel.SelectMany(x => x.HousingDepartmentList).Count(), result.Count);
            Assert.IsTrue(result.All(m => ManagementDepartmentListModel.FirstOrDefault(h => h.HousingDepartmentList.Any(x => x.Id == Guid.Parse(m.Id))) != null));
            Assert.IsTrue(result.All(m => m.Weeks.Count == Constants.DateTime.WeeksInYear));
        }

        [Test]
        public void GetYearPlanDepartments_ManagementIdIsNull_Test()
        {
            //arrange
            var jobAssignProvider = BuildJobAssignProvider();
            var categoryService = BuildCategoryService();
            var dayAssignProvider = BuildDayAssignProvider();
            var jobProvider = BuildJobProvider();
            var memberService = BuildMemberService();

            var yearlyPlanService = new YearlyPlanService(jobProvider.Object, categoryService.Object, dayAssignProvider.Object, memberService.Object, jobAssignProvider.Object);

            var year = 2017;
            //act
            var result = yearlyPlanService.GetYearPlanDepartments(Job.Id, year).ToList();
            //assert
            Assert.AreEqual(ManagementDepartmentListModel.SelectMany(x => x.HousingDepartmentList).Count(), result.Count);
            Assert.IsTrue(result.All(m => ManagementDepartmentListModel.FirstOrDefault(h => h.HousingDepartmentList.Any(x => x.Id == Guid.Parse(m.Id))) != null));
            Assert.IsTrue(result.All(m => m.Weeks.Count == Constants.DateTime.WeeksInYear));
            Assert.IsTrue(result.Any(d => d.IsAssigned));
        }

        [Ignore("")]
        [Test]
        public void GetYearPlanDepartments_Test()
        {
            //arrange
            var jobAssignProvider = BuildJobAssignProvider();
            var categoryService = BuildCategoryService();
            var dayAssignProvider = BuildDayAssignProvider();
            var jobProvider = BuildJobProvider();
            var memberService = BuildMemberService();

            var yearlyPlanService = new YearlyPlanService(jobProvider.Object, categoryService.Object, dayAssignProvider.Object, memberService.Object, jobAssignProvider.Object);

            var weekModel = Job.Assigns.FirstOrDefault()
                .WeekList.FirstOrDefault();

            var year = 2017;
            //act
            var result = yearlyPlanService.GetYearPlanDepartments(Job.Id, year)
                .ToList();
            //assert
            Assert.AreEqual(ManagementDepartmentListModel.SelectMany(x => x.HousingDepartmentList).Count(), result.Count);
            Assert.IsTrue(result.All(m => ManagementDepartmentListModel.FirstOrDefault(h => h.HousingDepartmentList.Any(x => x.Id == Guid.Parse(m.Id))) != null));
            Assert.IsTrue(result.All(m => m.Weeks.Count == Constants.DateTime.WeeksInYear));
            Assert.IsTrue(result.Any(m => m.Weeks[weekModel.Number - 1].Status == YearTaskStatus.NotDefined));
        }

        [Ignore("")]
        [Test]
        public void GetYearPlanCategories_Test()
        {
            //arrange
            var jobAssignProvider = BuildJobAssignProvider();
            var categoryService = BuildCategoryService();
            var dayAssignProvider = BuildDayAssignProvider();
            var jobProvider = BuildJobProvider();
            var memberService = BuildMemberService();

            var yearlyPlanService = new YearlyPlanService(jobProvider.Object, categoryService.Object, dayAssignProvider.Object, memberService.Object, jobAssignProvider.Object);
            //act
            var result = yearlyPlanService.GetYearPlanCategories();
            //assert
            var taskItem = result.YearPlanItems.FirstOrDefault(i => i.IsTask);
            Assert.IsTrue(result.YearPlanItems.Count(i=>!i.IsTask) == 3);
            Assert.IsNotNull(taskItem);            
            Assert.IsTrue(taskItem.Id == Job.Id);
        }
    }
}