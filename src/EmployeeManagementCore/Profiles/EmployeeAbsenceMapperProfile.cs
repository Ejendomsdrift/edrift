using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Models;
using AutoMapper;
using Infrastructure.Interfaces;

namespace AbsenceTemplatesCore.Profiles
{
    public class EmployeeAbsenceMapperProfile : Profile, IMapProfile
    {
        public EmployeeAbsenceMapperProfile()
        {
            CreateMap<EmployeeAbsenceInfo, EmployeeAbsenceInfoModel>();

            CreateMap<IEmployeeAbsenceInfoModel, EmployeeAbsenceInfo>()
                .ForMember(d => d.IsDeleted, f => f.Ignore())
                .ForMember(d => d.Id, f => f.Ignore());

            CreateMap<AbsenceTemplate, AbsenceTemplateModel>();

            CreateMap<IAbsenceTemplateModel, AbsenceTemplate>()
                .ForMember(d => d.IsDeleted, f => f.Ignore());

            CreateMap<AbsenceTemplate, IAbsenceTemplateModel>();
        }
    }
}