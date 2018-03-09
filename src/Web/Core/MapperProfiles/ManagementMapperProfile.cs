using AutoMapper;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using Web.Models;

namespace Web.Core.MapperProfiles
{
    public class ManagementMapperProfile: Profile, IMapProfile
    {
        public ManagementMapperProfile()
        {
            CreateMap<ICurrentUserContextModel, CurrentUserContextViewModel>();

            CreateMap<IManagementDepartmentModel, ManagementViewModel>()
                .ForMember(d => d.Selected, f => f.Ignore());            

            CreateMap<IHousingDepartmentModel, HousingDepartmentViewModel>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id))                
                .ForMember(d => d.Name, f => f.MapFrom(x => $"{x.SyncDepartmentId} {x.Name}"))
                .ForMember(d => d.IsDisabled, f => f.Ignore());
        }
    }
}