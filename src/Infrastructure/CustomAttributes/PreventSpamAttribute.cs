using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;

namespace Infrastructure.CustomAttributes
{
    public class PreventSpamAttribute: System.Web.Http.AuthorizeAttribute
    {
        public int DelayRequestInSeconds = 1;
        public string ErrorMessage = "Excessive Request Attempts Detected.";

        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            filterContext.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var request = HttpContext.Current.Request;
            var cache = HttpContext.Current.Cache;

            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            originationInfo += request.UserAgent;

            var targetInfo = request.RawUrl + request.QueryString;

            var hashValue = string.Join(string.Empty, MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            if (cache[hashValue] != null)
            {
                return false;
            }
            else
            {
                cache.Add(hashValue, string.Empty, null, DateTime.Now.AddSeconds(DelayRequestInSeconds), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                return true;
            }            
        }
    }
}