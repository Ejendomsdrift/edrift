using MemberCore.Authentication.Providers;
using Owin;

namespace MemberCore.Authentication.Configurations
{
    public static class AuthenticationConfigurator
    {
        public static void ConfigureAuth(IAppBuilder app)
        {
            AuthenticationProvider strategy = null;
            if (AppSettings.CustomSettings.IsADFSLogin)
            {
                strategy = new AuthenticationProvider(new ADFSAuthenticationProvider());                
            }
            else
            {
                strategy = new AuthenticationProvider(new CookieAuthenticationProvider());
            }
            strategy.ConfigureAuthenticationStrategy(app);
        }
    }
}
