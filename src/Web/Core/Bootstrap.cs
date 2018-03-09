using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Implementation;
using AutoMapper;
using CancellingTemplatesCore.Contract.Interfaces;
using CancellingTemplatesCore.Implementation;
using CategoryCore.Contract.Interfaces;
using CategoryCore.Implementation;
using CategoryCore.Models;
using FileStorage;
using FileStorage.Configuration;
using Groups;
using Groups.Implementation;
using GroupsContract.Interfaces;
using Hangfire;
using Hangfire.Mongo;
using HistoryCore.Contract.Interfaces;
using HistoryCore.Implementation;
using Infrastructure.Constants;
using Infrastructure.EventSourcing;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Helpers.Implementation;
using Infrastructure.Interfaces;
using Infrastructure.Logging;
using Infrastructure.Logging.Implementation;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Implementation;
using ManagementDepartmentCore.Contract.Interfaces;
using ManagementDepartmentCore.Implementation;
using MemberCore.Authentication.Implementation;
using MemberCore.Authentication.Interfaces;
using MemberCore.Contract.Interfaces;
using MemberCore.Implementation;
using Microsoft.AspNet.SignalR;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MongoEventStore.Configurations;
using MongoEventStore.Implementation;
using MongoRepository.Contract.Interfaces;
using MongoRepository.Implementation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Syntax;
using Ninject.Web.Common;
using SecurityCore.Contract.Interfaces;
using SecurityCore.Implementation;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.ChartsDataBuilders;
using Statistics.Core.Implementation;
using Statistics.Core.Implementation.ChartsDataBuilders;
using StatusCore.Contract.Interfaces;
using StatusCore.Services;
using SyncDataService.Configuration;
using SyncDataService.Implementation;
using SyncDataService.Interfaces;
using System;
using System.Web;
using System.Web.Http;
using Translations.Configurations;
using Translations.Implementation;
using Translations.Interfaces;
using Web.Core;
using Web.Core.Configurations;
using Web.Core.Providers;
using YearlyPlanning;
using YearlyPlanning.Configuration;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;
using YearlyPlanning.Services;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Bootstrap), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Bootstrap), "Stop")]
namespace Web.Core
{
    public static class Bootstrap
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);

            var idProvider = new Bootstrapper().Kernel.Get<IUserIdProvider>();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        public static void RegisterApi(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            // Use only json formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Use camel case for JSON data.
            var jsonSerializerSettings = config.Formatters.JsonFormatter.SerializerSettings;

            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //Gloabl authorization
            config.Filters.Add(new AuthorizeAttribute());

            config.Filters.Add(new Attributes.LogExceptionFilterAttribute());
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                RegisterMessageBus(kernel);
                RegisterMappings();
                RegisterEventStore();
                RegisterHangfireTasks(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ILog>().To<NullLogger>().InSingletonScope();

            // Providers
            kernel.Bind<ITaskIdGenerator>().To<TaskIdGenerator>().InSingletonScope();
            kernel.Bind<IJobProvider>().To<JobProvider>().InSingletonScope();
            kernel.Bind<IJobAssignProvider>().To<JobAssignProvider>().InSingletonScope();
            kernel.Bind<IOperationalTaskProvider>().To<OperationalTaskProvider>().InSingletonScope();
            kernel.Bind<IDayAssignProvider>().To<DayAssignProvider>().InSingletonScope();
            kernel.Bind<IUserIdProvider>().To<UserIdProvider>().InSingletonScope();

            // Services
            kernel.Bind<ITranslationService>().To<TranslationService>().InSingletonScope();
            kernel.Bind<IWeekPlanService>().To<WeekPlanService>().InSingletonScope();
            kernel.Bind<IYearlyPlanService>().To<YearlyPlanService>().InSingletonScope();
            kernel.Bind<IManagementDepartmentService>().To<ManagementDepartmentService>().InSingletonScope();
            kernel.Bind<IJobService>().To<JobService>().InSingletonScope();
            kernel.Bind<IOperationalTaskService>().To<OperationalTaskService>().InSingletonScope();
            kernel.Bind<IMemberService>().To<MemberService>().InSingletonScope();
            kernel.Bind<IAuthenticationService>().To<AuthenticationService>().InRequestScope();
            kernel.Bind<ICategoryService>().To<CategoryService>().InRequestScope();
            kernel.Bind<IDayAssignService>().To<DayAssignService>().InRequestScope();
            kernel.Bind<IGroupService>().To<GroupService>().InSingletonScope();
            kernel.Bind<ITimeScheduleService>().To<TimeScheduleService>().InSingletonScope();
            kernel.Bind<IJobStatusService>().To<JobStatusService>().InSingletonScope();
            kernel.Bind<ISecurityService>().To<SecurityService>().InSingletonScope();
            kernel.Bind<IEmployeeAbsenceInfoService>().To<EmployeeAbsenceInfoService>().InSingletonScope();
            kernel.Bind<IAbsenceTemplatesService>().To<AbsenceTemplatesService>().InSingletonScope();
            kernel.Bind<ICancelingTemplatesService>().To<CancelingTemplatesService>().InSingletonScope();
            kernel.Bind<IJobStatusLogService>().To<JobStatusLogService>().InSingletonScope();
            kernel.Bind<ITaskStatisticService>().To<TaskStatisticService>().InSingletonScope();
            kernel.Bind<IHistoryService>().To<HistoryService>().InSingletonScope();
            kernel.Bind<IGuideCommentService>().To<GuideCommentService>().InSingletonScope();

            // Repositories
            kernel.Bind<IDbConfiguration>().ToMethod(c => MongoDBConfiguration.Configuration).InSingletonScope();
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>)).InSingletonScope();
            kernel.Bind<IMongoDatabaseRepository>().To<MongoDatabaseRepository>().InSingletonScope();
            kernel.Bind<ITranslationRepositoryConfiguration>().ToMethod(c => MongoDBConfiguration.Configuration).InSingletonScope();
            kernel.Bind<IYearlyPlanningConfiguration>().ToMethod(c => MongoDBConfiguration.Configuration).InSingletonScope();
            kernel.Bind<IMongoEventStoreConfiguration>().ToMethod(c => MongoDBConfiguration.Configuration).InSingletonScope();
            kernel.Bind<IFileStorageConfiguration>().ToMethod(c => MongoDBConfiguration.Configuration).InSingletonScope();
            kernel.Bind<ITranslationRepository>().To<TranslationRepository>().InSingletonScope();
            kernel.Bind<ITranslationLogRepository>().To<TranslationLogRepository>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<JobDomain>>().To<AggregateRootRepository<JobDomain>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<JobAssignDomain>>().To<AggregateRootRepository<JobAssignDomain>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<UploadData>>().To<AggregateRootRepository<UploadData>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<OperationalTask>>().To<AggregateRootRepository<OperationalTask>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<DayAssignDomain>>().To<AggregateRootRepository<DayAssignDomain>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<GroupSource>>().To<AggregateRootRepository<GroupSource>>().InSingletonScope();
            kernel.Bind<IAggregateRootRepository<CategorySource>>().To<AggregateRootRepository<CategorySource>>().InSingletonScope();

            // Helpers
            kernel.Bind<IFileHelper>().To<FileHelper>().InSingletonScope();
            kernel.Bind<IAppSettingHelper>().To<AppSettingHelper>().InSingletonScope();
            kernel.Bind<IPathHelper>().To<PathHelper>().InSingletonScope();
            kernel.Bind<IDayAssignsTimeSpanSelector>().To<DayAssignsTimeSpanSelector>().InSingletonScope();
            kernel.Bind<ITaskChartModelBuilder>().To<TaskChartModelBuilder>().InSingletonScope();
            kernel.Bind<ITaskRatioChartModelBuilder>().To<TaskRatioChartModelBuilder>().InSingletonScope();
            kernel.Bind<ISpentTimeChartDataBuilder>().To<SpentTimeChartDataBuilder>().InSingletonScope();
            kernel.Bind<IStatisticFiltersModelBuilder>().To<StatisticFiltersModelBuilder>().InSingletonScope();
            kernel.Bind<IAddressVisitsChartModelBuilder>().To<AddressVisitsChartModelBuilder>().InSingletonScope();
            kernel.Bind<ICsvHelper>().To<CsvHelper>().InSingletonScope();
            kernel.Bind<ITasksInfoBuilder>().To<TasksInfoBuilder>().InSingletonScope();

            // Sync
            kernel.Bind<IImportService>().To<ImportService>().InRequestScope();
            kernel.Bind<IApplicationConfiguration>().ToMethod(c => ApplicationConfiguration.Configuration).InSingletonScope();
            kernel.Bind<IRestClientConfiguration>().ToMethod(c => RestClientConfiguration.Configuration).InSingletonScope();
            kernel.Bind<IImporter>().To<Importer>();
        }

        private static void RegisterMessageBus(IBindingRoot kernel)
        {
            kernel.Bind<IHandlersProvider>().ToMethod(c => new HandlersProvider(c.Kernel)).InSingletonScope();
            kernel.Bind<IMessageBus>().To<SynchronousMessageBus>().InSingletonScope();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            kernel.Bind(x => x
                .From(assemblies)
                .SelectAllClasses()
                .InheritedFrom(typeof(IHandler<>))
                .BindAllInterfaces()
                .Configure(s => s.InRequestScope())
                );
        }

        private static void RegisterMappings()
        {
            var profiles = typeof(IMapProfile).GetInheritedClasses();

            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });

            Mapper.AssertConfigurationIsValid();
        }

        private static void RegisterEventStore()
        {
            MongoEventStore.Startup.RegisterBsonClassMaps();
        }

        private static void RegisterHangfireTasks(StandardKernel kernel)
        {
            string hangfireDBName = AppSettingHelper.GetAppSetting<string>(Constants.AppSetting.HangfireDBName);
            JobStorage.Current = new MongoStorage("mongodb://localhost", hangfireDBName);
            HangfireGlobalConfiguration.Configuration.UseNinjectActivator(kernel);

            MoveExpiredJobs(kernel);
            SyncClientData(kernel);
            SyncClientAvatars(kernel);
        }

        private static void MoveExpiredJobs(StandardKernel kernel)
        {
            var weekPlanService = kernel.Get<IWeekPlanService>();
            RecurringJob.AddOrUpdate(() => weekPlanService.MoveExpiredJobs(true), Cron.Weekly(DayOfWeek.Monday, 1));
        }

        private static void SyncClientData(StandardKernel kernel)
        {
            var importService = kernel.Get<IImportService>();
            RecurringJob.AddOrUpdate(() => importService.SyncData(), Cron.MinuteInterval(5));
        }

        private static void SyncClientAvatars(StandardKernel kernel)
        {
            var importService = kernel.Get<IImportService>();
            RecurringJob.AddOrUpdate(() => importService.SyncMembersAvatars(), Cron.DayInterval(1));
        }
    }
}
