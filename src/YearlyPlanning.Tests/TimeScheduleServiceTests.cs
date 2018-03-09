using NUnit.Framework;

namespace YearlyPlanning.Tests
{
    [TestFixture]
    public class TimeScheduleServiceTests : TimeScheduleServiceTestsContext
    {
        //[Test]
        //public void GetMemberEstimationsForWeekTest()
        //{
        //    //arrange
        //    var dayAssignMock = BuildDayAssignProvider();
        //    var groupService = BuildGroupService();
        //    var memberService = BuildMemberService();
        //    var managementDepartmentService = BuildManagementDepartmentService();
        //    var timeScheduleService = new TimeScheduleService(dayAssignMock.Object, groupService.Object, memberService.Object, managementDepartmentService.Object);

        //    var memberId = Members.First().MemberId;
        //    DayAssigns.ForEach(d => d.UserIdList = Members.Select(m => m.MemberId).ToList());
        //    var year = 2017;
        //    var week = 1;
        //    //act
        //    var result = timeScheduleService.GetMemberEstimationsForWeek(memberId, year, week);
        //    //assert
        //    Assert.AreEqual(30, result.Sum(e => e.Value));
        //}

        //[Test]
        //public void TaskAssignedToAllUsers_Test()
        //{
        //    //arrange
        //    var dayAssignMock = BuildDayAssignProvider();
        //    var groupService = BuildGroupService();
        //    var memberService = BuildMemberService();
        //    var managementDepartmentService = BuildManagementDepartmentService();
        //    var timeScheduleService = new TimeScheduleService(dayAssignMock.Object, groupService.Object, memberService.Object, managementDepartmentService.Object);

        //    var memberId = Members.First().MemberId;
        //    DayAssigns.ForEach(d => d.IsAssignedToAllUsers = true);
        //    DayAssigns.ForEach(d => d.GroupId = null);
        //    var year = 2017;
        //    var week = 1;
        //    var day = 1;
        //    //act
        //    var result = timeScheduleService.GetMembersEstimationsForDay(new[] {memberId}, year, week, day);
        //    //assert
        //    var memberEstimations = result[memberId];
        //    Assert.AreEqual(15, memberEstimations);
        //}

        //[Test]
        //public void GetUsersCountIfTaskAssignedToSomeUsers_Test()
        //{
        //    //arrange
        //    var dayAssignMock = BuildDayAssignProvider();
        //    var groupService = BuildGroupService();
        //    var memberService = BuildMemberService();
        //    var managementDepartmentService = BuildManagementDepartmentService();
        //    var timeScheduleService = new TimeScheduleService(dayAssignMock.Object, groupService.Object, memberService.Object, managementDepartmentService.Object);

        //    DayAssigns.ForEach(d => d.UserIdList = Members.Select(m => m.MemberId).ToList());
        //    var memberId = Members.First().MemberId;
        //    var year = 2017;
        //    var week = 1;
        //    var day = 1;
        //    //act
        //    var result = timeScheduleService.GetMembersEstimationsForDay(new[] {memberId}, year, week, day);
        //    //assert
        //    var memberEstimations = result[memberId];
        //    Assert.AreEqual(15, memberEstimations);
        //}

        //[Test]
        //public void GetUsersCountIfTaskAssignedToAllUsersInGroup_Test()
        //{
        //    //arrange
        //    var dayAssignMock = BuildDayAssignProvider();
        //    var groupService = BuildGroupService();
        //    var memberService = BuildMemberService();
        //    var managementDepartmentService = BuildManagementDepartmentService();
        //    var timeScheduleService = new TimeScheduleService(dayAssignMock.Object, groupService.Object, memberService.Object, managementDepartmentService.Object);

        //    DayAssigns.ForEach(d => d.IsAssignedToAllUsers = true);

        //    var memberId = Members.First().MemberId;
        //    var year = 2017;
        //    var week = 1;
        //    var day = 1;
        //    //act
        //    var result = timeScheduleService.GetMembersEstimationsForDay(new[] { memberId }, year, week, day);
        //    //assert
        //    var memberEstimations = result[memberId];
        //    Assert.AreEqual(15, memberEstimations);
        //}

        //[Test]
        //public void GetUsersCountIfTaskAssignedToSomeUsersInGroup_Test()
        //{
        //    //arrange
        //    var dayAssignMock = BuildDayAssignProvider();
        //    var groupService = BuildGroupService();
        //    var memberService = BuildMemberService();
        //    var managementDepartmentService = BuildManagementDepartmentService();
        //    var timeScheduleService = new TimeScheduleService(dayAssignMock.Object, groupService.Object, memberService.Object, managementDepartmentService.Object);

        //    DayAssigns.ForEach(d => d.UserIdList = Members.Select(m => m.MemberId).ToList());

        //    var memberId = Members.First().MemberId;
        //    var year = 2017;
        //    var week = 1;
        //    var day = 1;
        //    //act
        //    var result = timeScheduleService.GetMembersEstimationsForDay(new[] { memberId }, year, week, day);
        //    //assert
        //    var memberEstimations = result[memberId];
        //    Assert.AreEqual(15, memberEstimations);
        //}
    }
}