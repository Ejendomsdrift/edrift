using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using MemberCore.Authentication.Interfaces;
using MemberCore.Authentication.Configurations;

namespace MemberCore.Authentication.Providers
{
    public class CookieAuthenticationProvider: IAutenticationProvider
    {
        private const string LoginRedirectUrlConst = "loginRedirectUrl";
        public void ConfigureAuthenticationProvider(IAppBuilder app)
        {
            app.UseCookieAuthentication(options: new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/"),                
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new Microsoft.Owin.Security.Cookies.CookieAuthenticationProvider()
                {
                    OnApplyRedirect = context =>
                    {
                        context.Response.Headers.Add(LoginRedirectUrlConst, new[] { AppSettings.CustomSettings.CustomLoginUrl });
                    }
                }
            });
        }
    }
}