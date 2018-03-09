using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using System;
using Web.Models;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Core.MapperProfiles
{
    public class WeekPlanMapperProfile : Profile, IMapProfile
    {
        public WeekPlanMapperProfile()
        {
            CreateMap(typeof(WeekPlanTaskRequestViewModel), typeof(IWeekPlanFilterModel))
                .ForMember(nameof(IWeekPlanFilterModel.JobState), f => f.MapFrom(i => ((WeekPlanTaskRequestViewModel)i).ListViewCurrentTab.Map<JobStateType>()))
                .As(typeof(IWeekPlanFilterModel).GetFirstInheritedClass());

            CreateMap<WeekPlanListViewTabEnum, JobStateType>().ConvertUsing(value =>
            {
                switch (value)
                {
                    case WeekPlanListViewTabEnum.Current:
                        return JobStateType.InProgress;
                    case WeekPlanListViewTabEnum.Completed:
                        return JobStateType.Completed;
                    case WeekPlanListViewTabEnum.NotCompleted:
                        return JobStateType.NotCompleted;
                    default:
                        throw new NotImplementedException("JobStateType doesn't match WeekPlanListViewTabEnum: " + value);
                }
            });
        }
    }
}