using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Statistics.Contract.Interfaces;
using Web.Models;

namespace Web.Core.MapperProfiles
{
    public class TimePeriodMapperProfile : Profile, IMapProfile
    {
        public TimePeriodMapperProfile()
        {
            CreateMap(typeof(TimePeriodViewModel), typeof(ITimePeriod))
                .As(typeof(ITimePeriod).GetFirstInheritedClass());
        }
    }
}