namespace MemberCore.Authentication.Interfaces
{
    public interface ILoginModel
    {
        string Login { get; set; }
        string Password { get; set; }
    }
}