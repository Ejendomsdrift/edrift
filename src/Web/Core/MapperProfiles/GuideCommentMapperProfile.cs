using AutoMapper;
using Infrastructure.Interfaces;
using Web.Models;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Core.MapperProfiles
{
    public class GuideCommentMapperProfile : Profile, IMapProfile
    {
        public GuideCommentMapperProfile()
        {
            CreateMap<GuidCommentViewModel, IGuideCommentModel>();
        }
    }
}