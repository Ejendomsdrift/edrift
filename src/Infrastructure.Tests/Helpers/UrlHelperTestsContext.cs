using System.Collections.Generic;
using Infrastructure.Helpers;
using Moq;
using NUnit.Framework;

namespace Infrastructure.Tests
{
    public class PathHelperTestsContext
    {
        protected Dictionary<string, string> Settings;

        [SetUp]
        public void SetUp()
        {
            Settings = new Dictionary<string, string>()
            {
                {"memberAvatarPath", "/Files/MembersAvatar/{Id}.png"},
                {"jobAssignUploadsPath", "/Files/JobsAssignAttachments/{jobAssignId}/{fileId}"}
            };
        }

        protected Mock<IAppSettingHelper> BuildAppSettingHelper()
        {
            var helper = new Mock<IAppSettingHelper>();
            helper.Setup(h => h.GetAppSetting<string>(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns<string, bool>((s, b) => Settings[s]);
            return helper;
        }
    }
}