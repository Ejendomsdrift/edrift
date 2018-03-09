using MemberCore.Authentication.Interfaces;

namespace MemberCore.Authentication.Models
{
    public class LoginResultModel : ILoginResultModel
    {
        public bool IsLoginExist { get; set; }
        public bool IsPasswordExist { get; set; }
        public bool UserHasRole { get; set; }
        public int? UserRole { get; set; }
        public bool IsUserDisabled { get; set; }
    }
}
