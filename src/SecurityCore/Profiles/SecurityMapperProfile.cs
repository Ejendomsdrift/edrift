using AutoMapper;
using Infrastructure.Interfaces;
using SecurityCore.Contract.Interfaces;
using SecurityCore.Models;

namespace SecurityCore.Profiles
{
    public class SecurityMapperProfile : Profile, IMapProfile
    {
        public SecurityMapperProfile()
        {
            CreateMap<SecurityPermission, ISecurityPermissionModel>().As<SecurityPermissionModel>();
            CreateMap<Rule, IRuleModel>().As<RuleModel>();
            CreateMap<IRuleModel, Rule>();
        }
    }
}
