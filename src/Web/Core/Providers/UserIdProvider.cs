using System;
using System.Linq;
using System.Security.Claims;
using MemberCore.Contract.Interfaces;
using Microsoft.AspNet.SignalR;

namespace Web.Core.Providers
{
    public class UserIdProvider : IUserIdProvider
    {
        private readonly IMemberService memberService;

        public UserIdProvider(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        public string GetUserId(IRequest request)
        {
            try
            {
                var identity = request.User.Identity as ClaimsIdentity;
                var member = memberService.GetByUserName(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                return member.MemberId.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}