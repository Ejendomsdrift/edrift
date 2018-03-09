using AutoMapper;
using GroupsContract.Models;
using Infrastructure.Interfaces;
using MemberCore.Contract.Interfaces;
using Web.Models.Group;

namespace Web.Core.MapperProfiles
{
    public class GroupMapperProfile: Profile, IMapProfile
    {
        public GroupMapperProfile()
        {
            CreateMap<IGroupModel, GroupMemberViewModel>()
                .ForMember(d => d.Members, f => f.Ignore())
                .ForMember(d=>d.IsCanBeDeleted, f=>f.Ignore());

            CreateMap<IGroupModel, GroupViewModel>()
                .ForMember(d => d.AllowedMembers, f => f.Ignore());

            CreateMap<IMemberModel, MemberViewModel>()
                .ForMember(d => d.SpentHours, f => f.Ignore())
                .ForMember(d => d.SpentMinutes, f => f.Ignore())
                .ForMember(d => d.HasSpentTime, f => f.Ignore());
        }
    }
}