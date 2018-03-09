using Hangfire.Dashboard;
using MemberCore.Authentication.Configurations;
using Microsoft.AspNet.Identity;
using System.Web;

namespace Web.Core.Hangfire
{
    public class AuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            string currentUserName = HttpContext.Current.GetOwinContext().Authentication.User.Identity.GetUserId();
            return currentUserName == AppSettings.CustomSettings.DefaultUserName;
        }
    }
}