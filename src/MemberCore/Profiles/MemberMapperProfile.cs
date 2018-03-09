using AutoMapper;
using Infrastructure.Interfaces;
using MemberCore.Models;

namespace MemberCore.Profiles
{
    public class MemberMapperProfile : Profile, IMapProfile
    {
        public MemberMapperProfile()
        {
            CreateMap<SyncMember, Member>();
            CreateMap<Member, SyncMember>().ForMember(sm => sm.AvatarFileContent, opt => opt.Ignore());
        }
    }
}