using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Models;
using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Web.Models;

namespace Web.Core.MapperProfiles
{
    public class AbsenceInfoMapperProfile : Profile, IMapProfile
    {
        public AbsenceInfoMapperProfile()
        {
            CreateMap<NewEmployeeAbsenceInfoModel, EmployeeAbsenceInfoModel>()
                .ForMember(d => d.Id, f => f.Ignore());
            CreateMap(typeof(NewEmployeeAbsenceInfoModel), typeof(IEmployeeAbsenceInfoModel))
                .ForMember(nameof(IEmployeeAbsenceInfoModel.Id), d => d.Ignore())
                .As(typeof(IEmployeeAbsenceInfoModel).GetFirstInheritedClass());

        }
    }
}