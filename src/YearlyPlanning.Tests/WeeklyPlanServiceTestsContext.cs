using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using CategoryCore.Models;
using Groups.Models;
using GroupsContract.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using ManagementDepartmentCore.Models;
using MemberCore.Contract.Interfaces;
using Moq;
using NUnit.Framework;
using StatusCore.Contract.Enums;
using StatusCore.Contract.Interfaces;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Tests
{
    public class WeeklyPlanServiceTestsContext
    {

        protected List<ManagementDepartmentModel> ManagementDepartments;
        protected List<Job> Jobs;
        protected List<HousingDepartment> HousingDepartments;
        protected List<Group> Groups;
        protected List<JobAssign> JobAssigns;

        protected ManagementDepartmentModel CommonManagementDepartment;
        protected HousingDepartment CommonHousingDepartment;
        protected Group CommonGroup;
        protected Job Job1;
        protected Job Job2;
        protected JobAssign JobAssign1;
        protected JobAssign JobAssign2;
        protected CategoryModel ParentCategory;
        protected CategoryModel ChildCategory1;
        protected CategoryModel ChildCategory2;
        protected DayAssign DayAssign1;
        protected DayAssign DayAssign2;

        [SetUp]
        protected void SetUp()
        {
            SetUpManagementDepartments();
            SetUpManagementJobs();
            SetUpHousingDepartments();
            SetUpGroups();
            SetUpCategoryModels();
            SetUpJobAssigns();
            SetUpDayAssigns();
            SetUpAutoMapper();
        }

        private void SetUpAutoMapper()
        {
            var profiles = typeof(IMapProfile).GetInheritedClasses();
            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });
        }

        private void SetUpManagementDepartments()
        {
            ManagementDepartments = new EditableList<ManagementDepartmentModel>();

            CommonManagementDepartment = new ManagementDepartmentModel()
            {
                Id = Guid.NewGuid(),
                Name = "CommonManagementDepartment"
            };
        }

        private void SetUpManagementJobs()
        {
            Jobs = new List<Job>();

            Job1 = new Job()
            {
                Id = "Job1"
            };

            Job2 = new Job()
            {
                Id = "Job1"
            };
        }

        private void SetUpCategoryModels()
        {
            ParentCategory = new CategoryModel()
            {
                Id = Guid.NewGuid()
            };
            ChildCategory1 = new CategoryModel()
            {
                Id = Guid.NewGuid()
            };
            ChildCategory2 = new CategoryModel()
            {
                Id = Guid.NewGuid()
            };
        }

        private void SetUpHousingDepartments()
        {
            HousingDepartments = new List<HousingDepartment>();

            CommonHousingDepartment = new HousingDepartment()
            {
                Id = Guid.NewGuid(),
                Name = "CommonHousingDepartment"
            };

        }

        private void SetUpGroups()
        {
            CommonGroup = new Group()
            {
                Id = Guid.NewGuid()
            };
            Groups = new List<Group>() {CommonGroup};
        }

        private void SetUpJobAssigns()
        {
            JobAssigns = new List<JobAssign>();

            JobAssign1 = new JobAssign()
            {
                Id = Guid.NewGuid()
            };

            JobAssign2 = new JobAssign()
            {
                Id = Guid.NewGuid()
            };
        }

        private void SetUpDayAssigns()
        {
            DayAssign1 = new DayAssign()
            {
                Id = Guid.NewGuid()
            };
            DayAssign2 = new DayAssign()
            {
                Id = Guid.NewGuid()
            };
        }


        protected void SetUpTest_GetTasksAndWeeksStartedFromSelectedWeek_TakePart()
        {

            Jobs = new List<Job>() {Job1, Job2};

            Job1.Assigns = new List<JobAssign>() {JobAssign1};
            Job2.Assigns = new List<JobAssign>() {JobAssign2};

            Job1.DayAssigns = new List<IDayAssign>() {DayAssign1};
            DayAssign1.JobId = Job1.Id;
            DayAssign1.JobAssignId = JobAssign1.Id;
            DayAssign1.WeekNumber = 2;
            JobAssign1.HousingDepartmentIdList = new List<Guid>() {CommonHousingDepartment.Id};

            Job2.DayAssigns = new List<IDayAssign>() {DayAssign2};
            DayAssign2.JobId = Job2.Id;
            DayAssign2.JobAssignId = JobAssign2.Id;
            DayAssign2.WeekNumber = 2;
            JobAssign2.HousingDepartmentIdList = new List<Guid>() {CommonHousingDepartment.Id};

            HousingDepartments = new List<HousingDepartment>() {CommonHousingDepartment};
        }

        protected void SetUpTest_GetTasksAndWeeksStartedFromSelectedWeek_TakeAll()
        {
            Jobs = new List<Job>() {Job1, Job2};

            Job1.Assigns = new List<JobAssign>() {JobAssign1};
            Job2.Assigns = new List<JobAssign>() {JobAssign2};

            Job1.DayAssigns = new List<IDayAssign>() {DayAssign1};
            DayAssign1.JobId = Job1.Id;
            DayAssign1.JobAssignId = JobAssign1.Id;
            DayAssign1.WeekNumber = 2;
            JobAssign1.HousingDepartmentIdList = new List<Guid>() {CommonHousingDepartment.Id};

            Job2.DayAssigns = new List<IDayAssign>() {DayAssign2};
            DayAssign2.JobId = Job2.Id;
            DayAssign2.JobAssignId = JobAssign2.Id;
            DayAssign2.WeekNumber = 3;
            JobAssign2.HousingDepartmentIdList = new List<Guid>() {CommonHousingDepartment.Id};

            HousingDepartments = new List<HousingDepartment>() {CommonHousingDepartment};
        }

        protected void SetUpTest_Tabs()
        {
            Jobs = TabsTestsJobs().ToList();
            JobAssign1.HousingDepartmentIdList = new List<Guid>() {CommonHousingDepartment.Id};
            HousingDepartments = new List<HousingDepartment>() {CommonHousingDepartment};
        }

        private IEnumerable<DayAssign> TabsTestsDayAssigns()
        {
            var createAssign = new Func<JobStatus, DayAssign>((jobStatus) =>
                new DayAssign()
                {
                    Id = Guid.NewGuid(),
                    StatusId = jobStatus
                });

            yield return createAssign(JobStatus.Assigned);
            yield return createAssign(JobStatus.Canceled);
            yield return createAssign(JobStatus.Completed);
            yield return createAssign(JobStatus.InProgress);
            yield return createAssign(JobStatus.Opened);
            yield return createAssign(JobStatus.Paused);
            yield return createAssign(JobStatus.Pending);
            yield return createAssign(JobStatus.Rejected);
        }

        private IEnumerable<Job> TabsTestsJobs()
        {
            return TabsTestsDayAssigns()
                .Select(a =>
                {
                    var job = new Job()
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    a.JobId = job.Id;
                    a.JobAssignId = JobAssign1.Id;
                    a.WeekDay = 2;
                    a.WeekNumber = 2;
                    JobAssign1.TillYear = 2012;
                    job.Assigns = new List<JobAssign>() {JobAssign1};
                    job.DayAssigns = new List<IDayAssign>() {a};
                    return job;
                });
        }

        protected Mock<IJobService> BuildJobService()
        {
            var mock = new Mock<IJobService>();
            return mock;
        }

        protected Mock<IJobProvider> BuildJobProvider()
        {
            var mock = new Mock<IJobProvider>();
            mock.Setup(m => m.GetByDepartmentIdYearWeek(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns<Guid, int, int, bool, bool>((departmentId, year, weekNumber, fillDayAssigns, fillCategories) => Jobs);
            return mock;
        }

        protected Mock<IMemberService> BuildMemberService()
        {
            var mock = new Mock<IMemberService>();
            return mock;
        }

        protected Mock<IGroupService> BuildGroupService()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(m => m.GetByIds(It.IsAny<IEnumerable<Guid>>()))
                .Returns<IEnumerable<Guid>>(ids => Groups.Where(g => ids.Contains(g.Id)).Map<IEnumerable<GroupModel>>());
            return mock;
        }

        protected Mock<IAppSettingHelper> BuildAppSettingsHelper()
        {
            var mock = new Mock<IAppSettingHelper>();
            mock.Setup(m => m.GetFromJson<IEnumerable<JobStatus>>(It.IsAny<string>()))
                .Returns(new List<JobStatus> {JobStatus.Assigned, JobStatus.Opened, JobStatus.Pending});
            return mock;
        }

        protected Mock<IJobStatusService> BuildJobStatusService()
        {
            var mock = new Mock<IJobStatusService>();
            mock.Setup(m => m.Pending(It.IsAny<Guid>(), It.IsAny<JobStatus>()));
            return mock;
        }

        protected Mock<IDayAssignService> BuildDayAssignService()
        {
            var mock = new Mock<IDayAssignService>();
            return mock;
        }

        protected Mock<IJobStatusLogService> BuildJobStatusLogService()
        {
            var mock = new Mock<IJobStatusLogService>();
            return mock;
        }

        protected Mock<IPathHelper> BuildPathHelper()
        {
            var mock = new Mock<IPathHelper>();
            return mock;
        }

        protected Mock<IManagementDepartmentService> BuildManagementDepartmentService()
        {
            var mock = new Mock<IManagementDepartmentService>();
            return mock;
        }
    }
}