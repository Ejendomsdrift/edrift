using AutoMapper;
using Infrastructure.Interfaces;
using StatusCore.Contract.Interfaces;
using StatusCore.Models;

namespace StatusCore.Profiles
{
    public class JobStatusLogMapperProfile : Profile, IMapProfile
    {
        public JobStatusLogMapperProfile()
        {
            CreateMap<JobStatusLog, IJobStatusLogModel>()
                .ForMember(d => d.CancelingReason, f => f.Ignore())
                .As<JobStatusLogModel>();

            CreateMap<TimeLog, ITimeLogModel>();
            CreateMap<JobStatusLog, IJobStatusLog>();
        }
    }
}
