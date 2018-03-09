using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using SecurityCore.Contract.Enums;
using SecurityCore.Contract.Interfaces;
using SecurityCore.Models;
using Web.Enums;
using Web.Models.Security;

namespace Web.Core.MapperProfiles
{
    public class SecurityPermissionMapperProfile : Profile, IMapProfile
    {
        public SecurityPermissionMapperProfile()
        {
            CreateMap<Pages, SecurityPages>();

            CreateMap<SecurityPermissionModel, SecurityPermission>()
                .ForMember(x => x.Id, f => f.Ignore());

            CreateMap(typeof(SequrityQueryViewModel), typeof(ISecurityQuery))
                .As(typeof(ISecurityQuery).GetFirstInheritedClass());

            CreateMap(typeof (SecurityPermissionViewModel), typeof (ISecurityPermissionModel))
                .As(typeof (ISecurityPermissionModel).GetFirstInheritedClass());

            CreateMap(typeof(RuleViewModel), typeof(IRuleModel))
                .As(typeof(IRuleModel).GetFirstInheritedClass());
        }
    }
}