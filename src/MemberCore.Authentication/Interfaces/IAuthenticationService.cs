namespace MemberCore.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        string GetCurrentUserName();

        void Logout();

        void Login(string userName);
    }
}
