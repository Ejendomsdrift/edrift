using FileStorage.Contract.Enums;
using Infrastructure.Helpers;
using System.Collections.Generic;
using System.Web.Http;
using static Infrastructure.Constants.Constants;

namespace Web.Controllers
{
    [RoutePrefix("api/setting")]
    public class SettingController: ApiController
    {
        private readonly IAppSettingHelper appSettingHelper;

        public SettingController(IAppSettingHelper appSettingHelper)
        {
            this.appSettingHelper = appSettingHelper;
        }

        [AllowAnonymous]
        [HttpGet, Route("IsADFSMode")]
        public bool IsADFSMode()
        {
            return appSettingHelper.GetAppSetting<bool>(AppSetting.IsADFSLogin);
        }

        [HttpGet, Route("getFileExtensions")]
        public Dictionary<UploadedContentEnum, IEnumerable<string>> GetFileExtensions()
        {
            var result = appSettingHelper.GetFromJson<Dictionary<UploadedContentEnum, IEnumerable<string>>>(AppSetting.FileExtensions);
            return result;
        }
    }
}