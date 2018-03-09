using MemberCore.Authentication.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web;
using MemberCore.Authentication.Configurations;

namespace MemberCore.Authentication.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private HttpContext httpContext => HttpContext.Current;
        private IAuthenticationManager authenticationManager => httpContext.GetOwinContext().Authentication;

        public string GetCurrentUserName()
        {
            if (HasAccessAnonymousUser())
            {
                return AppSettings.CustomSettings.DefaultUserName;
            }

            return authenticationManager.User.Identity.GetUserId();
        }

        public void Logout()
        {
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        public void Login(string userName)
        {
            ClaimsIdentity identity = GetIdentity(userName);
            authenticationManager.SignIn(identity);
        }

        private bool HasAccessAnonymousUser()
        {
            if (authenticationManager.User?.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            string anonymousAccessToken = httpContext.Request.Headers[HttpRequestHeader.Authorization.ToString()];
            bool hasAccess = anonymousAccessToken == AppSettings.CustomSettings.AnonymousAccessToken;
            return hasAccess;
        }

        private ClaimsIdentity GetIdentity(string userName)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));

            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return identity;
        }
    }
}
