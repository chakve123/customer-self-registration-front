using System;
using System.Net;
using System.Web.Http.Filters;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.User;
using EservicesAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string userName;
            try
            {
                var authUser = (AuthUser)context.Request.Properties["AuthUser"];
                userName = authUser.Username;
            }
            catch
            {
                userName = string.Empty;
            }

            try
            {

                if (context.Exception is UserExceptions || context.Exception.Message.StartsWith("ORA-20001:"))
                {
                    string json;

                    if (context.Exception is UserExceptions) json = context.Exception.Message;
                    else json = context.Exception.Message.Substring(11, context.Exception.Message.IndexOf("\n", StringComparison.Ordinal) - 11);

                    var httpStatusCode = HttpStatusCode.BadRequest;

                    var jsonObj = JsonConvert.DeserializeObject("{" + json + "}");
                    var status = ((JObject)jsonObj)["STATUS"];
                    var statusCode = status["ID"].ToString().ToNumber<int>();

                    if (statusCode >= -199 && statusCode <= -100) httpStatusCode = HttpStatusCode.Unauthorized;

                    context.Response = ResponseBuilder.Error(json, httpStatusCode);
                }
                else
                {
                    context.Response = ResponseBuilder.LogError(context.Exception, userName);
                }
            }
            catch (Exception ex)
            {
                context.Response = ResponseBuilder.LogError(ex, userName);
            }
        }
    }
}