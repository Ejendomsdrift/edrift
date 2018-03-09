using AutoMapper;
using CancellingTemplatesCore.Contract.Interfaces;
using CancellingTemplatesCore.Models;
using Infrastructure.Interfaces;
using Web.Models;

namespace Web.Core.MapperProfiles
{
    public class CancellingTemplateMapperProfile : Profile, IMapProfile
    {
        public CancellingTemplateMapperProfile()
        {
            CreateMap<CancellingTemplateViewModel, ICancelingTemplateModel>()
                .ForMember(f => f.Id, t => t.Ignore());

            CreateMap<ICancelingTemplateModel, CancellingTemplate>()
                .ForMember(f => f.IsDeleted, t => t.Ignore());
        }
    }
}