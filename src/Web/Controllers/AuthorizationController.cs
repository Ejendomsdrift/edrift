using System.Web.Http;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using MemberCore.Contract.Interfaces;
using MemberCore.Authentication.Interfaces;
using MemberCore.Contract.Enums;
using MemberCore.Authentication.Models;
using Microsoft.AspNet.SignalR;
using Web.Core.Hubs;

namespace Web.Controllers
{
    [RoutePrefix("api/authorization")]
    public class AuthorizationController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IMemberService memberService;
        private readonly IAppSettingHelper appSettingHelper;
        private IHubContext janitorHubs;
        private IHubContext managementHubs;

        public AuthorizationController(
            IAuthenticationService authenticationService,
            IMemberService memberService,
            IAppSettingHelper appSettingHelper)
        {
            this.authenticationService = authenticationService;
            this.memberService = memberService;
            this.appSettingHelper = appSettingHelper;
            this.janitorHubs = GlobalHost.ConnectionManager.GetHubContext<JanitorHub>();
            this.managementHubs = GlobalHost.ConnectionManager.GetHubContext<ManagementHub>();
        }

        [AllowAnonymous]
        [HttpPost, Route("login")]
        public LoginResultModel LogIn(LoginModel model)
        {
            string defaultPassword = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.DefaultPassword);
            var user = memberService.GetByEmail(model.Login);

            var result = new LoginResultModel
            {
                IsPasswordExist = true,
                UserHasRole = true,
                IsLoginExist = user != null && user.Roles.HasValue() && model.Password == defaultPassword,
                UserRole = user != null ? (int) user.CurrentRole : default(int?),
                IsUserDisabled = user?.IsDeleted ?? false
            };

            if (!result.IsLoginExist)
            {
                return result;
            }

            authenticationService.Login(user.UserName);

            switch (user.CurrentRole)
            {
                case RoleType.Coordinator:
                    janitorHubs.Clients.User(user.MemberId.ToString()).refreshPage();
                    break;

                case RoleType.Janitor:
                    managementHubs.Clients.User(user.MemberId.ToString()).refreshPage();
                    break;
            }

            return result;
        }

        [AllowAnonymous]
        [HttpGet, Route("logout")]
        public void LogOut()
        {
            var currentUser = memberService.GetCurrentUser();
            memberService.ClearCurrentRole(currentUser.MemberId);
            authenticationService.Logout();
        }
    }
}