using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using StatusCore.Contract.Enums;
using Web.Models;
using Web.Models.Task;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace Web.Core.MapperProfiles
{
    public class JobMapperProfile : Profile, IMapProfile
    {
        public JobMapperProfile()
        {
            CreateMap<IJobHeaderModel, JobHeaderViewModel>();
            CreateMap<IJobCounterModel, JobCounterViewModel>();
            CreateMap<IJobDetailsModel, JobDetailsViewModel>();

            CreateMap<IFormattedJobAssign, FormattedJobAssignViewModel>()
                .ForMember(x => x.AddressList, f => f.Ignore())
                .ForMember(x => x.CurrentUser, f => f.Ignore())
                .ForMember(x => x.IsGroupedJob, f => f.Ignore())
                .ForMember(x => x.IsChildJob, f => f.Ignore());

            CreateMap<JobAssign, JobAssignViewModel>();

            CreateMap<IOperationalTaskModel, AdHocViewModel>()
                .ForMember(m => m.IsCanceled, f => f.MapFrom(x => x.StatusId == JobStatus.Canceled));

            CreateMap<IOperationalTaskModel, TenantViewModel>()
                .ForMember(m => m.IsCanceled, f => f.MapFrom(x => x.StatusId == JobStatus.Canceled));

            CreateMap<IOperationalTaskModel, OtherTaskViewModel>()
                .ForMember(m => m.IsCanceled, f => f.MapFrom(x => x.StatusId == JobStatus.Canceled));

            CreateMap<UploadFileModel, UploadFileViewModel>()
               .ForMember(d => d.Uploader, f => f.Ignore());

            CreateMap<UploadFileViewModel, UploadFileModel>();

            CreateMap<NewAdHocTaskModel, MemberAssignModel>()
                .ForMember(x => x.IsUnassignAll, f => f.Ignore())
                .ForMember(x => x.DayAssignId, f => f.Ignore());

            CreateMap<NewTenantTaskModel, MemberAssignModel>()
                .ForMember(x => x.IsUnassignAll, f => f.Ignore())
                .ForMember(x => x.DayAssignId, f => f.Ignore());

            CreateMap<NewOtherTaskModel, MemberAssignModel>()
                .ForMember(x => x.IsUnassignAll, f => f.Ignore())
                .ForMember(x => x.DayAssignId, f => f.Ignore());

            CreateMap(typeof(NewAdHocTaskModel), typeof(IOperationalTaskModel))
                .As(typeof(IOperationalTaskModel).GetFirstInheritedClass());

            CreateMap(typeof(NewTenantTaskModel), typeof(IOperationalTaskModel))
                .As(typeof(IOperationalTaskModel).GetFirstInheritedClass());

            CreateMap(typeof(NewOtherTaskModel), typeof(IOperationalTaskModel))
                .As(typeof(IOperationalTaskModel).GetFirstInheritedClass());
        }
    }
}