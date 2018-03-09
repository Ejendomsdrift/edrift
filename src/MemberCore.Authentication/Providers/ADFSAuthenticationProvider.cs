using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using Microsoft.Owin;
using System;
using MemberCore.Authentication.Interfaces;
using MemberCore.Authentication.Configurations;
using System.Net;

namespace MemberCore.Authentication.Providers
{
    public class ADFSAuthenticationProvider : IAutenticationProvider
    {
        public void ConfigureAuthenticationProvider(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
            {
                AuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType,
                MetadataAddress = AppSettings.ADFS.Metadata,
                Wtrealm = AppSettings.ADFS.Realm
            });

            app.Use((context, continuation) =>
            {
                return AuthenticationHandler(context, continuation);
            });
        }

        private Task AuthenticationHandler(IOwinContext context, Func<Task> continuation)
        {
            if (IsValidUser(context))
            {
                return continuation();
            }

            context.Authentication.Challenge(WsFederationAuthenticationDefaults.AuthenticationType);
            return Task.FromResult(0);
        }

        private bool IsValidUser(IOwinContext context)
        {
            return context.Authentication.User?.Identity?.IsAuthenticated == true ||
                   context.Request.Headers[HttpRequestHeader.Authorization.ToString()] == AppSettings.CustomSettings.AnonymousAccessToken; // need for sync functionality
        }
    }
}