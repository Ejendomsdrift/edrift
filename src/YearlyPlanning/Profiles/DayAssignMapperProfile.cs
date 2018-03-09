using System;
using System.Collections.Generic;
using AutoMapper;
using Infrastructure.Interfaces;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;

namespace YearlyPlanning.Profiles
{
    public class DayAssignMapperProfile: Profile, IMapProfile
    {
        public DayAssignMapperProfile()
        {
            CreateMap<INewDayAssignModel, CreateDayAssignCommand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()))
                .ForMember(d => d.StatusId, f => f.Ignore())
                .ForMember(d => d.Type, f => f.Ignore())
                .ForMember(d => d.DayPerWeekId, f => f.Ignore())                
                .ForMember(d => d.Address, f => f.Ignore());


            CreateMap<INewDayAssignModel, ChangeDayAssignDateCommand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()));

            CreateMap<INewDayAssignModel, ChangeDayAssignMembersComand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()));


            CreateMap<IWeekPlanFilterModel, ITaskDataFilterModel>()
                .ForMember(f => f.HousingDepartments, t => t.MapFrom(x => x.HousingDepartmentId.HasValue ? new List <Guid> { x.HousingDepartmentId.Value } : new List<Guid>()))
                .ForMember(f => f.StartWeek, t => t.Ignore())
                .ForMember(f => f.EndDate, t => t.Ignore())
                .As<TaskDataFilterModel>();
        }
    }
}
