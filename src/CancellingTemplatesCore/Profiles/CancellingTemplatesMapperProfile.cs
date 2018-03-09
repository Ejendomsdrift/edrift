using AutoMapper;
using CancellingTemplatesCore.Contract.Interfaces;
using CancellingTemplatesCore.Models;
using Infrastructure.Interfaces;

namespace CancellingTemplatesCore.Profiles
{
    public class CancellingTemplatesMapperProfile : Profile, IMapProfile
    {
        public CancellingTemplatesMapperProfile()
        {
            CreateMap<CancellingTemplate, CancelingTemplateModel>();

            CreateMap<ICancelingTemplateModel, CancellingTemplate>()
                .ForMember(d => d.IsDeleted, f => f.Ignore())
                .ForMember(d=>d.IsCoordinatorReason, f=>f.Ignore());

            CreateMap<CancellingTemplate, ICancelingTemplateModel>();
        }
    }
}