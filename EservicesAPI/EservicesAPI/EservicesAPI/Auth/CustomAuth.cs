using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using EservicesAPI.Common;

namespace EservicesAPI.Auth
{
    public class CustomAuth : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //var errorText = ConfigurationManager.AppSettings["invalid_token"];

            //var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            //if (principal != null && !principal.Identity.IsAuthenticated)
            //{
            //    actionContext.Response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.Unauthorized, -4, errorText);
            //}
        }
    }
}