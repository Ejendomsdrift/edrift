using AutoMapper;
using Infrastructure.Interfaces;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.Models;

namespace Statistics.Core.Implementation
{
    public class StatisticMapperProfile : Profile, IMapProfile
    {
        public StatisticMapperProfile()
        {
            CreateMap<RejectionReasonInfo, ICancelingReasonDataModel>()
                .ForMember(x => x.JobId, f => f.MapFrom(y => y.Id))
                .ForMember(x => x.Reason, f => f.MapFrom(y => y.RejectionReason))
                .ForMember(x => x.RejectionDate, f => f.MapFrom(y => y.RejectionDate))
                .ForMember(x => x.HousingDepartmentId, f => f.Ignore())
                .ForMember(x => x.DayAssignId, f => f.Ignore())
                .ForMember(x => x.ReasonId, f => f.Ignore())
                .ForMember(x => x.ManagementDepartmentId, f => f.Ignore());

            CreateMap<ICancelingReasonDataModel, RejectionReasonInfo>()
                .ForMember(x => x.RejectionReason, f => f.MapFrom(y => y.Reason))
                .ForMember(x => x.RejectionDate, f => f.MapFrom(y => y.RejectionDate))
                .ForMember(x => x.Id, f => f.MapFrom(y => y.JobId));
        }
    }
}
