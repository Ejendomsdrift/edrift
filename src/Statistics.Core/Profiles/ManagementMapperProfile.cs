using System.Collections.Generic;
using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using ManagementDepartmentCore.Contract.Interfaces;
using Statistics.Core.Models;

namespace Statistics.Core.Profiles
{
    public class ManagementMapperProfile : Profile, IMapProfile
    {
        public ManagementMapperProfile()
        {
            CreateMap<IManagementDepartmentModel, ManagementDepartmentStatisticModel>()
                .ForMember(d => d.HousingDepartments, f => f
                    .MapFrom(x => x.HousingDepartmentList.Map<IEnumerable<HousingDepartmentStatisticModel>>()));
            CreateMap<IHousingDepartmentModel, HousingDepartmentStatisticModel>()
                .ForMember(d => d.Name, f => f.MapFrom(d => d.DisplayName));
        }
    }
}