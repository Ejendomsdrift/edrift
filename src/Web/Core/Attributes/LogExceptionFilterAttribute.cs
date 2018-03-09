using Elmah;
using System.Web;
using System.Web.Http.Filters;

namespace Web.Core.Attributes
{
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ErrorLog.GetDefault(HttpContext.Current).Log(new Error(actionExecutedContext.Exception));
        }
    }
}