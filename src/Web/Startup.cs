using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Infrastructure.Constants;
using Infrastructure.Helpers.Implementation;
using MemberCore.Authentication.Configurations;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Web.Core.Hangfire;

[assembly: OwinStartup(typeof(Web.Startup))]

namespace Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthenticationConfigurator.ConfigureAuth(app);
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);

            string hangfireDBName = AppSettingHelper.GetAppSetting<string>(Constants.AppSetting.HangfireDBName);
            GlobalConfiguration.Configuration.UseMongoStorage("mongodb://localhost", hangfireDBName);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter() }
            });

            app.UseHangfireServer();
        }
    }
}
