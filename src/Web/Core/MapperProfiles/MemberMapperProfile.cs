using AutoMapper;
using Infrastructure.Interfaces;
using MemberCore.Contract.Interfaces;
using Web.Models;
using Web.Models.Task;

namespace Web.Core.MapperProfiles
{
    public class MemberMapperProfile : Profile, IMapProfile
    {
        public MemberMapperProfile()
        {
            CreateMap<IMemberModel, MemberViewModel>()
                .ForMember(d => d.Avatar, f => f.MapFrom(d => d.Avatar))
                .ForMember(d => d.Name, f => f.MapFrom(d => d.Name));
            CreateMap<IMemberModel, MemberModelWithTimeView>()
                .ForMember(d => d.TimeView, f => f.Ignore());
        }
    }
}