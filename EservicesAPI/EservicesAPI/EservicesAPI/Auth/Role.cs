using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BaseLib.Common;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Common;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;

namespace EservicesAPI.Auth
{
    //public class Role : AuthorizationFilterAttribute
    //{
    //    public Modules Module { get; set; }

    //    public Permissions Permission { get; set; }

    //    public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            var errorText = ConfigurationManager.AppSettings["invalid_token"];
    //            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
    //            if (principal != null && !principal.Identity.IsAuthenticated)
    //            {
    //               //actionContext.Response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.Unauthorized, -4, errorText);
    //                return Task.FromResult<object>(null);
    //            }

    //            //var errorText = ConfigurationManager.AppSettings["perm_error"];
    //            //var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
    //            //if (principal != null && !principal.Identity.IsAuthenticated)
    //            //{
    //            //    actionContext.Response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.Unauthorized, -2, errorText);
    //            //    return Task.FromResult<object>(null);
    //            //}

    //            var userID = principal.Claims.FirstOrDefault(x => x.Type == "UserID").Value.ToNumber<int>();
    //            var subUserID = principal.Claims.FirstOrDefault(x => x.Type == "SubUserID").Value.ToNumber<int>();

    //            var permission = DataProviderManager<PKG_USERS>.Provider.has_permission(userID, subUserID, (int)Module, (int)Permission);
    //            if (permission == 0)
    //            {
    //                //actionContext.Response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.Unauthorized, -2, errorText);
    //                return Task.FromResult<object>(null);
    //            }

    //            actionContext.Request.Properties.Add("Permission", permission);
    //        }
    //        catch (Exception ex)
    //        {
    //            var errorText = ConfigurationManager.AppSettings["error_text"];
    //            CommonFunctions.CatchExceptions(ex);
    //            //actionContext.Response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.InternalServerError, -3, errorText);
    //            return Task.FromResult<object>(null);
    //        }

    //        return Task.FromResult<object>(null);
    //    }
    //}
}