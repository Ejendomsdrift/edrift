using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;
using System.Net.Http;

namespace Web.Core
{
    public class StopCamelCaseAttribute : ActionFilterAttribute
    {
        private JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();

        public StopCamelCaseAttribute()
        {
            formatter.SerializerSettings.ContractResolver = new DefaultContractResolver();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ObjectContent content = actionExecutedContext.Response.Content as ObjectContent;
            if (content != null)
            {
                if (content.Formatter is JsonMediaTypeFormatter)
                {
                    actionExecutedContext.Response.Content = new ObjectContent(content.ObjectType, content.Value, formatter);
                }
            }
        }
    }
}