using System.Web;
using System.Web.Http;
using Web.Core;

namespace Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Bootstrap.RegisterApi);
        }
    }
}
