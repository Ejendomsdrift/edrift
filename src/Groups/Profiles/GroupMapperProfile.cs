using AutoMapper;
using Infrastructure.Interfaces;
using GroupsContract.Models;
using Groups.Models;

namespace Groups.Profiles
{
    public class GroupMapperProfile : Profile, IMapProfile
    {
        public GroupMapperProfile()
        {
            CreateMap<IGroupModel, Group>();
            CreateMap<Group, GroupModel>();
            CreateMap<GroupSource, GroupModel>();
        }
    }
}
