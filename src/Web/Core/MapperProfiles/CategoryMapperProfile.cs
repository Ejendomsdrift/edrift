using AutoMapper;
using CategoryCore.Contract.Interfaces;
using Infrastructure.Interfaces;
using Web.Models;

namespace Web.Core.MapperProfiles
{
    public class CategoryMapperProfile : Profile, IMapProfile
    {
        public CategoryMapperProfile()
        {
            CreateMap<ICategoryModel, CategoryViewModel>()
                .ForMember(d => d.CanBeHidden, f => f.Ignore())
                .ForMember(d => d.Children, f => f.Ignore())
                .ForMember(d => d.Parent, f => f.Ignore())
                .ForMember(d => d.Tasks, f => f.Ignore());
        }
    }
}