using Owin;

namespace MemberCore.Authentication.Interfaces
{
    public interface IAutenticationProvider
    {
        void ConfigureAuthenticationProvider(IAppBuilder app);
    }
}
