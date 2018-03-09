namespace ManagementDepartmentCore.Tests
{
    //[TestFixture]
  /*  public class ManagementDepartmentTests : ManagmentDepartmentTestsContext
    {
        [Test]
        public void SyncManagementDepartments_ParameterIsNull_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);
            //act
            departmentService.SyncManagementDepartments(null);
            //assert
            repoMock.Verify(r => r.Save(It.IsAny<ManagementDepartment>()), Times.Never);
        }

        [Test]
        public void SyncMembers_AddMember_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);

            var department = new ManagementDepartment()
            {
                Id = Guid.NewGuid()
            };
            var syncDepartment = new[] {department}.ToJson();
            //act
            departmentService.SyncManagementDepartments(syncDepartment);
            //assert
            Assert.IsNotNull(Departments.FirstOrDefault(m => m.Id == department.Id));
        }

        [Test]
        [Ignore("")]
        public void SyncMembers_AlterMember_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);

            var department = Departments.First();
            var housingDepartment = department.HousingDepartmentList.First();
            var alteredDepartment = new ManagementDepartment()
            {
                Id = housingDepartment.Id,
                ManagementDepartmentRefId = department.ManagementDepartmentRefId,
                SyncDepartmentId = department.SyncDepartmentId,
                HousingDepartmentList = new List<HousingDepartment>()
                {
                    new HousingDepartment()
                    {
                        Id = Guid.NewGuid(),
                        SyncDepartmentId = "NewDepartment"
                    }
                }
            };
            var syncDepartments = new[] {alteredDepartment}.ToJson();
            //act        
            departmentService.SyncManagementDepartments(syncDepartments);
            //assert           
            Assert.NotNull(department.HousingDepartmentList.FirstOrDefault(r => r.SyncDepartmentId == "NewDepartment"));
            Assert.True(department.HousingDepartmentList.Any(d => d.Id == housingDepartment.Id && d.IsDeleted));
            Assert.True(Departments.Any(r => r.IsDeleted));
        }

        [Test]
        [Ignore("")]
        public void GetAllManagements_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);
            //act
            var result = departmentService.GetAllManagements();
            //assert
            Assert.True(Departments.All(m => result.FirstOrDefault(d => d.Id == m.Id) != null));
        }

        [Test]
        [Ignore("")]
        public void GetParentManagementId_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);

            var department = Departments.First();
            var child = department.HousingDepartmentList.FirstOrDefault();
            //act
            var result = departmentService.GetParentManagementId(child.Id);
            //assert
            Assert.NotNull(result == department.Id);
        }

        [Test]
        [Ignore("")]
        public void GetHousingDepartments_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var departmentService = new ManagementDepartmentService(repoMock.Object);

            var department = Departments.First();
            var housingDepartment = department.HousingDepartmentList.FirstOrDefault();
            //act
            var result = departmentService.GetHousingDepartments(department.Id);
            //assert
            Assert.NotNull(result.FirstOrDefault(r => r.Id == housingDepartment.Id) != null);
        }

        [Test]
        [Ignore("")]
        public void GetHousingDepartment_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var memberService = new ManagementDepartmentService(repoMock.Object);

            var department = Departments.First();
            var housingDepartment = department.HousingDepartmentList.FirstOrDefault();
            //act
            var result = memberService.GetHousingDepartment(housingDepartment.Id);
            //assert
            Assert.NotNull(result.Id == housingDepartment.Id);
        }

        [Test]
        [Ignore("")]
        public void GetAllHousingDepartments_Test()
        {
            //arrange
            var repoMock = BuildDepartmentsRepository();
            var memberService = new ManagementDepartmentService(repoMock.Object);

            var housingDepartments = Departments.SelectMany(d => d.HousingDepartmentList);
            //act
            var result = memberService.GetAllHousingDepartments();
            //assert
            Assert.IsTrue(housingDepartments.All(h => result.FirstOrDefault(r => h.Id == r.Id) != null));
        }
    }*/
}