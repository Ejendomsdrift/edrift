namespace MemberCore.Authentication.Interfaces
{
    public interface ILoginResultModel
    {
        bool IsLoginExist { get; set; }
        bool IsPasswordExist { get; set; }
        bool UserHasRole { get; set; }
        int? UserRole { get; set; }
    }
}