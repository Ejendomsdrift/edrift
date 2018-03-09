using System.Linq;
using NUnit.Framework;
using YearlyPlanning.Services;

namespace YearlyPlanning.Tests
{
    [TestFixture]
    public class WeeklyPlanServiceTests : WeeklyPlanServiceTestsContext
    {
        [Test]
        [Ignore("Recheck it")]
        public void GetTasksAndWeeksStartedFromSelectedWeek_TakePart_Test()
        {
            //arrange
            var dayAssignService = BuildDayAssignService();
            var memberService = BuildMemberService();
            var jobProvider = BuildJobProvider();
            var jobService = BuildJobService();
            var jobStatusLogService = BuildJobStatusLogService();
            var managementDepartmentService = BuildManagementDepartmentService();
            var pathHelper = BuildPathHelper();
            var groupService = BuildGroupService();
            var appSettings = BuildAppSettingsHelper();
            var jobStatusService = BuildJobStatusService();

            var weeklyPlanService = new WeekPlanService(jobProvider.Object, memberService.Object, groupService.Object, dayAssignService.Object,
                jobStatusLogService.Object, pathHelper.Object, managementDepartmentService.Object, jobService.Object, appSettings.Object, jobStatusService.Object);

            SetUpTest_GetTasksAndWeeksStartedFromSelectedWeek_TakePart();

            var weekNumber = 1;
            var year = 2017;
            var showedTaskCount = 1;
            var departmentId = HousingDepartments.FirstOrDefault().Id;
            ////act
            //var result = weeklyPlanService
            //    .GetTasksAndWeeksStartedFromSelectedWeek(departmentId, weekNumber, year, showedTaskCount, WeekPlanListViewTabEnum.Current);
            ////assert
            //Assert.AreEqual(showedTaskCount, result.TasksByWeeks.Count);
            //Assert.IsTrue(result.MoreResultsIsAvailable);
        }

        [Test]
        [Ignore("Recheck it")]
        public void GetTasksAndWeeksStartedFromSelectedWeek_TakeAll_Test()
        {
            //arrange
            var dayAssignService = BuildDayAssignService();
            var memberService = BuildMemberService();
            var jobProvider = BuildJobProvider();
            var jobService = BuildJobService();
            var jobStatusLogService = BuildJobStatusLogService();
            var managementDepartmentService = BuildManagementDepartmentService();
            var pathHelper = BuildPathHelper();
            var groupService = BuildGroupService();
            var appSettings = BuildAppSettingsHelper();
            var jobStatusService = BuildJobStatusService();

            var weeklyPlanService = new WeekPlanService(jobProvider.Object, memberService.Object, groupService.Object, dayAssignService.Object,
                jobStatusLogService.Object, pathHelper.Object, managementDepartmentService.Object, jobService.Object, appSettings.Object, jobStatusService.Object);

            SetUpTest_GetTasksAndWeeksStartedFromSelectedWeek_TakeAll();

            var weekNumber = 2;
            var year = 2017;
            var showedTaskCount = 2;
            var departmentId = HousingDepartments.FirstOrDefault().Id;
            ////act
            //var result = weeklyPlanService
            //    .GetTasksAndWeeksStartedFromSelectedWeek(departmentId, weekNumber, year, showedTaskCount, WeekPlanListViewTabEnum.Current);
            ////assert
            //Assert.AreEqual(showedTaskCount, result.TasksByWeeks.Count);
            //Assert.IsFalse(result.MoreResultsIsAvailable);
        }

        [Test]
        [Ignore("Recheck it")]
        public void Tab_Current_Test()
        {
            //arrange
            var dayAssignService = BuildDayAssignService();
            var memberService = BuildMemberService();
            var jobProvider = BuildJobProvider();
            var jobService = BuildJobService();
            var jobStatusLogService = BuildJobStatusLogService();
            var managementDepartmentService = BuildManagementDepartmentService();
            var pathHelper = BuildPathHelper();
            var groupService = BuildGroupService();
            var appSettings = BuildAppSettingsHelper();
            var jobStatusService = BuildJobStatusService();

            var weeklyPlanService = new WeekPlanService(jobProvider.Object, memberService.Object, groupService.Object, dayAssignService.Object,
                jobStatusLogService.Object, pathHelper.Object, managementDepartmentService.Object, jobService.Object, appSettings.Object, jobStatusService.Object);

            SetUpTest_Tabs();

            var weekNumber = 2;
            var year = 2017;
            var taskCount = 5;
            var departmentId = HousingDepartments.FirstOrDefault().Id;
            ////act
            //var result = weeklyPlanService
            //    .GetTasksAndWeeksStartedFromSelectedWeek(departmentId, weekNumber, year, taskCount, WeekPlanListViewTabEnum.Current);
            ////assert
            //Assert.AreEqual(taskCount, result.TasksByWeeks[2].Count());
            //Assert.IsFalse(result.MoreResultsIsAvailable);
        }

        [Test]
        [Ignore("Recheck it")]
        public void Tab_Completed_Test()
        {
            //arrange
            var dayAssignService = BuildDayAssignService();
            var memberService = BuildMemberService();
            var jobProvider = BuildJobProvider();
            var jobService = BuildJobService();
            var jobStatusLogService = BuildJobStatusLogService();
            var managementDepartmentService = BuildManagementDepartmentService();
            var pathHelper = BuildPathHelper();
            var groupService = BuildGroupService();
            var appSettings = BuildAppSettingsHelper();
            var jobStatusService = BuildJobStatusService();

            var weeklyPlanService = new WeekPlanService(jobProvider.Object, memberService.Object, groupService.Object, dayAssignService.Object,
                jobStatusLogService.Object, pathHelper.Object, managementDepartmentService.Object, jobService.Object, appSettings.Object, jobStatusService.Object);

            SetUpTest_Tabs();

            var weekNumber = 2;
            var year = 2017;
            var taskCount = 1;
            var departmentId = HousingDepartments.FirstOrDefault().Id;
            ////act
            //var result = weeklyPlanService
            //    .GetTasksAndWeeksStartedFromSelectedWeek(departmentId, weekNumber, year, taskCount, WeekPlanListViewTabEnum.Completed);
            ////assert
            //Assert.AreEqual(taskCount, result.TasksByWeeks[2].Count());
            //Assert.IsFalse(result.MoreResultsIsAvailable);
        }

        [Test]
        [Ignore("Recheck it")]
        public void Tab_NotCompleted_Test()
        {
            //arrange
            var dayAssignService = BuildDayAssignService();
            var memberService = BuildMemberService();
            var jobProvider = BuildJobProvider();
            var jobService = BuildJobService();
            var jobStatusLogService = BuildJobStatusLogService();
            var managementDepartmentService = BuildManagementDepartmentService();
            var pathHelper = BuildPathHelper();
            var groupService = BuildGroupService();
            var appSettings = BuildAppSettingsHelper();
            var jobStatusService = BuildJobStatusService();

            var weeklyPlanService = new WeekPlanService(jobProvider.Object, memberService.Object, groupService.Object, dayAssignService.Object,
                jobStatusLogService.Object, pathHelper.Object, managementDepartmentService.Object, jobService.Object, appSettings.Object, jobStatusService.Object);

            SetUpTest_Tabs();

            var weekNumber = 2;
            var year = 2017;
            var taskCount = 7;
            var departmentId = HousingDepartments.FirstOrDefault().Id;
            ////act
            //var result = weeklyPlanService
            //    .GetTasksAndWeeksStartedFromSelectedWeek(departmentId, weekNumber, year, taskCount, WeekPlanListViewTabEnum.NotCompleted);
            ////assert
            //Assert.AreEqual(taskCount, result.TasksByWeeks[2].Count());
            //Assert.IsFalse(result.MoreResultsIsAvailable);
        }
    }
}