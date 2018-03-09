using Owin;
using MemberCore.Authentication.Interfaces;

namespace MemberCore.Authentication.Providers
{
    public class AuthenticationProvider
    {
        private IAutenticationProvider autentication;        

        public AuthenticationProvider(IAutenticationProvider autentication)
        {
            this.autentication = autentication;
        }

        public void ConfigureAuthenticationStrategy(IAppBuilder app)
        {
            autentication.ConfigureAuthenticationProvider(app);
        }
    }
}