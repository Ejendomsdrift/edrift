using System;
using System.IO;
using System.Web;
using static Infrastructure.Constants.Constants;

namespace Infrastructure.Helpers.Implementation
{
    public class PathHelper : IPathHelper
    {
        private readonly IAppSettingHelper appSettingHelper;

        public PathHelper(IAppSettingHelper appSettingHelper)
        {
            this.appSettingHelper = appSettingHelper;
        }

        public string GetAvatarPath()
        {
            return appSettingHelper.GetAppSetting<string>(AppSetting.MemberAvatarFolderPath);
        }

        public string GetAvatarPath(Guid memberId)
        {
            return appSettingHelper.GetAppSetting<string>(AppSetting.MemberAvatarPath).Replace("{Id}", memberId.ToString());
        }

        public string GetAvatarPathForSync()
        {
            return appSettingHelper.GetAppSetting<string>(AppSetting.MemberAvatarFolderPathForSync);
        }

        public string GetAvatarPathForSync(Guid memberId)
        {
            return appSettingHelper.GetAppSetting<string>(AppSetting.MemberAvatarPathForSync).Replace("{Id}", memberId.ToString());
        }

        public string GetJobAssignUploadsPath(Guid jobAssignId, Guid fileId, string extension)
        {
            var path = appSettingHelper.GetAppSetting<string>(AppSetting.JobAssignUploadsPath)
                .Replace("{jobAssignId}", jobAssignId.ToString())
                .Replace("{fileId}", fileId.ToString());
            return $"{path}{extension}";
        }

        public string GetJobAssignDirectoryPath(Guid jobAssignId)
        {
            var path = appSettingHelper.GetAppSetting<string>(AppSetting.JobAssignUploadsPath)
                .Replace("{jobAssignId}", jobAssignId.ToString())
                .Replace("{fileId}", string.Empty);
            var fullPath = HttpContext.Current.Server.MapPath("~" + path);
            var directoryName = Path.GetDirectoryName(fullPath) + "\\";
            return directoryName;
        }

        public string GetDayAssignUploadsPath(Guid dayAssignId, Guid fileId, string extension)
        {
            var path = appSettingHelper.GetAppSetting<string>(AppSetting.DayAssignUploadsPath)
                .Replace("{dayAssignId}", dayAssignId.ToString())
                .Replace("{fileId}", fileId.ToString());
            return $"{path}{extension}";
        }
    }
}
