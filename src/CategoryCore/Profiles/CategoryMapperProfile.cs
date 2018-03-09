using AutoMapper;
using CategoryCore.Contract.Interfaces;
using CategoryCore.Models;
using Infrastructure.Interfaces;

namespace CategoryCore.Profiles
{
    public class CategoryMapperProfile : Profile, IMapProfile
    {
        public CategoryMapperProfile()
        {
            CreateMap<ICategoryModel, Category>();
            CreateMap<Category, CategoryModel>()
                .ForMember(c => c.Parent, f => f.Ignore())
                .ForMember(c => c.Children, f => f.Ignore());
            CreateMap<CategorySource, CategoryModel>()
                .ForMember(c => c.Parent, f => f.Ignore())
                .ForMember(c => c.Children, f => f.Ignore());
        }
    }
}
