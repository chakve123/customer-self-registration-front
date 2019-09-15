using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EservicesAPI.Filters
{

    public class RequestLog : ActionFilterAttribute
    {
        public string LogParams { get; set; }

        public Modules Module { get; set; }

        private Stopwatch _RequestStopWatch = new Stopwatch();

        private int _ModuleId { get; set; }
        private string _RequestUri { get; set; }
        private int _UserId { get; set; }
        private int _UnId { get; set; }
        private int _SubUserId { get; set; }
        private string _Ip { get; set; }
        private string _RequestParams { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            _RequestStopWatch.Start();

            _ModuleId = (int)Module;
            _RequestUri = actionContext.Request.RequestUri.AbsolutePath;
            _UserId = actionContext.ControllerContext.Request.Properties["UserID"].ToString().ToNumber<int>();
            _UnId = actionContext.ControllerContext.Request.Properties["UnID"].ToString().ToNumber<int>();
            _SubUserId = actionContext.ControllerContext.Request.Properties["SubUserID"].ToString().ToNumber<int>();
            //_Ip = actionContext.ControllerContext.Request.Headers.Referrer.Host;

            _Ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //_Ip = (actionContext.Request.Properties["MS_OwinContext"] as OwinContext).Request.RemoteIpAddress; //.Request.RemoteIpAddress;
            var actionArgs = actionContext.ActionArguments;


            /*Request ის პარამეტრების ლოგირება*/
            if (actionArgs.Count != 0 && !string.IsNullOrWhiteSpace(LogParams))
            {
                var paramsArray = new List<string>();


                var searchKeys = LogParams.Split(',');

                foreach (var k in searchKeys)
                {
                    /*request ის პარამეტრის object*/
                    var requestParamsObj = actionContext.ActionArguments.FirstOrDefault().Value;

                    var requestParamsJObj = requestParamsObj as JObject;

                    var searchSubKeys = k.Split('.');

                    if (requestParamsJObj != null)
                    {
                        var jObjValFromRequestParams = requestParamsJObj[searchSubKeys[0]] as JToken;
                        for (int i = 1; i < searchSubKeys.Length; i++)
                        {

                            if (jObjValFromRequestParams == null || jObjValFromRequestParams is JArray) break;

                            jObjValFromRequestParams = jObjValFromRequestParams.Value<JToken>(searchSubKeys[i]);
                        }

                        var jTokenFromRequestParams = jObjValFromRequestParams is JValue ? jObjValFromRequestParams : null;


                        if (jTokenFromRequestParams != null)
                        {
                            var path = jTokenFromRequestParams.Path;

                            var value = (jTokenFromRequestParams as JValue).Value.ToString();
                            paramsArray.Add($"{path}:{value}");
                        }
                    }
                }
                if (paramsArray.Any())
                {
                    _RequestParams = paramsArray.Aggregate((current, next) => current + ", " + next);
                }
                else
                {
                    _RequestParams = "ERROR";
                }
            }

        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _RequestStopWatch.Stop();

            var requestTimeInterval = _RequestStopWatch.Elapsed;

            DataProviderManager<PKG_ESERVICES_LOGS>.Provider.SaveEservicesLogs(_ModuleId, _RequestUri, _UserId, _UnId, _SubUserId, _Ip, requestTimeInterval, _RequestParams);
            
        }
    }

}