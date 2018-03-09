using System;
using Infrastructure.Helpers.Implementation;
using NUnit.Framework;

namespace Infrastructure.Tests
{
    [TestFixture]
    public class PathHelperTests:PathHelperTestsContext
    {
        [Test]
        public void GetAvatarUrl_Test()
        {
            //arrange
            var appSettingsMock = BuildAppSettingHelper();
            var pathHelper = new PathHelper(appSettingsMock.Object);

            var memberId = Guid.NewGuid();
            //act
            var result = pathHelper.GetAvatarPath(memberId);
            //assert
            Assert.IsTrue(result.Contains(memberId.ToString()));
        }

        [Test]
        public void GetJobAssignUploadsPath_Test()
        {
            //arrange
            var appSettingsMock = BuildAppSettingHelper();
            var pathHelper = new PathHelper(appSettingsMock.Object);

            var jobAssignId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            const string extension = "ext";
            //act
            var result = pathHelper.GetJobAssignUploadsPath(jobAssignId, fileId, extension);
            //assert
            Assert.IsTrue(result.Contains(jobAssignId.ToString()));
            Assert.IsTrue(result.Contains(fileId.ToString()));
            Assert.IsTrue(result.EndsWith(extension));
        }
    }
}
