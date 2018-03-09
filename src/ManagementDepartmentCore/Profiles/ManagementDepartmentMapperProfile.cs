using AutoMapper;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using ManagementDepartmentCore.Models;

namespace ManagementDepartmentCore.Profiles
{
    public class ManagementDepartmentMapperProfile : Profile, IMapProfile
    {
        public ManagementDepartmentMapperProfile()
        {
            CreateMap<ManagementDepartment, ManagementDepartmentModel>();
            CreateMap<ManagementDepartment, IManagementDepartmentModel>().As<ManagementDepartmentModel>();

            CreateMap<HousingDepartment, HousingDepartmentModel>()
                 .ForMember(d => d.ManagementDepartmentId, f => f.Ignore());
            CreateMap<HousingDepartment, IHousingDepartmentModel>().As<HousingDepartmentModel>();
        }
    }
}
