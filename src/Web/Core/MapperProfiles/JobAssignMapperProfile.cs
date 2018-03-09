using AutoMapper;
using Infrastructure.Interfaces;
using YearlyPlanning.Contract.Commands.JobAssignCommands;
using YearlyPlanning.Contract.Models;

namespace Web.Core.MapperProfiles
{
    public class JobAssignMapperProfile : Profile, IMapProfile
    {
        public JobAssignMapperProfile()
        {
            CreateMap<JobAssign, CreateJobAssignFromJobAssignCommand>()
                .ForMember(x => x.Id, f => f.Ignore()) // It's need for creating local job assign from global  
                .ForMember(x => x.HousingDepartmentIdList, f => f.Ignore()) // Don`t delete this!
                .ForMember(x => x.UploadList, f => f.Ignore()) // Don`t delete this!
                .ForMember(x => x.Description, f => f.Ignore()) // Don`t delete this!
                .ForMember(x => x.JobIdList, f => f.Ignore()) // Don`t delete this!
                .ForMember(x => x.RewriteChangedByWeeks, f => f.Ignore())
                .AfterMap((s, d) =>
                {
                    d.IsGlobal = false;
                });
        }
    }
}