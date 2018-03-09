using MemberCore.Authentication.Interfaces;

namespace MemberCore.Authentication.Models
{
    public class LoginModel : ILoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
