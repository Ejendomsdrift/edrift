using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models;
using YearlyPlanning;
using YearlyPlanning.Contract.Commands.DayAssignCommands;
using YearlyPlanning.Contract.Events.DayAssignEvents;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace Web.Core.MapperProfiles
{
    public class DayAssignMapperProfile : Profile, IMapProfile
    {
        public DayAssignMapperProfile()
        {
            CreateMap<DayAssignCreated, DayAssign>()
                .ForMember(d => d.Id, f => f.MapFrom(x => Guid.Parse(x.SourceId)))
                .ForMember(d => d.DepartmentId, f => f.MapFrom(x => x.HousingDepartmentId))
                .ForMember(d => d.WeekDay, f => f.MapFrom(x => x.WeekDay))
                .ForMember(d => d.TenantType, f => f.MapFrom(x => x.Type))
                .ForMember(d => d.UploadList, f => f.Ignore());

            CreateMap<INewDayAssignModel, ChangeDayAssignEstimatedMinutesCommand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()));

            CreateMap<INewDayAssignModel, ChangeDayAssignDateCommand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()));

            CreateMap<INewDayAssignModel, ChangeDayAssignMembersComand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()));

            CreateMap<INewDayAssignModel, CreateDayAssignCommand>()
                .ForMember(d => d.Id, f => f.MapFrom(x => x.Id.ToString()))
                .ForMember(d => d.StatusId, f => f.Ignore())
                .ForMember(d => d.DayPerWeekId, f => f.Ignore())
                .ForMember(d => d.Address, f => f.Ignore())
                .ForMember(d => d.Type, f => f.Ignore());

            CreateMap<OperationalTaskModel, CreateDayAssignCommand>()
                .ForMember(d => d.EstimatedMinutes, f => f.MapFrom(x => x.Estimate))
                .ForMember(d => d.Id, f => f.Ignore())
                .ForMember(d => d.JobAssignId, f => f.Ignore())
                .ForMember(d => d.GroupName, f => f.Ignore())
                .ForMember(d => d.DayPerWeekId, f => f.Ignore())
                .ForMember(d => d.StatusId, f => f.MapFrom(x => JobStatus.Pending))
                .ForMember(d => d.WeekNumber, f => f.MapFrom(x => x.Week))
                .ForMember(d => d.WeekDay, f => f.MapFrom(x => x.DaysPerWeek.FirstOrDefault()))
                .ForMember(d => d.Date, f => f.Ignore())
                .ForMember(d => d.Comment, f => f.Ignore())
                .ForMember(d => d.ResidentName, f => f.Ignore())
                .ForMember(d => d.ResidentPhone, f => f.Ignore())
                .ForMember(d => d.Type, f => f.Ignore())
                .ForMember(d => d.ExpiredDayAssignId, f => f.Ignore())
                .ForMember(d => d.ExpiredWeekNumber, f => f.Ignore());

            CreateMap<DayAssign, DayAssignDomain>()
                .ForMember(x => x.Id, f => f.MapFrom(x => x.Id.ToString()))
                .ForMember(x => x.Type, f => f.MapFrom(x => x.TenantType))
                .ForMember(x => x.Version, f => f.Ignore())
                .ForMember(x => x.Time, f => f.Ignore());

            CreateMap(typeof(WeekPlanTaskRequestViewModel), typeof(ITaskDataFilterModel))
                .ForMember(nameof(ITaskDataFilterModel.HousingDepartments), f => f.MapFrom(x => ((WeekPlanTaskRequestViewModel)x).HousingDepartmentId.HasValue ? new List<Guid> { ((WeekPlanTaskRequestViewModel)x).HousingDepartmentId.Value} : new List<Guid>()))
                .ForMember(nameof(ITaskDataFilterModel.EndDate), f => f.Ignore())
                .As(typeof(ITaskDataFilterModel).GetFirstInheritedClass());
        }
    }
}