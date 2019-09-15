using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using BaseLib.Common;
using BaseLib.Exceptions;
using EservicesLib.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Common
{
    public class ResponseBuilder
    {
        public static HttpResponseMessage Success(object data = null)
        {
            var statusText = ConfigurationManager.AppSettings["success_text"];
            var status = new { ID = 0, TEXT = statusText };
            var resObj = new { DATA = data == null ? new { } : data, STATUS = status };
            //var json = new JavaScriptSerializer().Serialize(resObj);
            var json = JsonConvert.SerializeObject(resObj);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = string.Empty,
                Content = new StringContent(json)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }

        public static HttpResponseMessage Error(string statusJson, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var json = "{\"DATA\":{}," + statusJson + "}";

            var response = new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                ReasonPhrase = string.Empty,
                Content = new StringContent(json)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }


        public static HttpResponseMessage Error(string statusJson, object data, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var dataJson = "{}";
            if (data != null && string.IsNullOrWhiteSpace(data.ToString()))
            {
                dataJson = data.ToString();
            }

            var json = "{\"DATA\": " + dataJson + "," + statusJson + "}";

            var response = new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                ReasonPhrase = string.Empty,
                Content = new StringContent(json)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }

        public static HttpResponseMessage LogError(Exception ex, string userName = "")
        {
            var statusJson = "\"STATUS\":{ \"ID\" :" + -1 + ", \"TEXT\" : \"" + ConfigurationManager.AppSettings["error_text"] + "\"}";

            CommonFunctions.CatchExceptions(ex, string.Empty, false, userName);

            return Error(statusJson, HttpStatusCode.InternalServerError);
        }

        public static HttpResponseMessage GetResponseMessageByContentJson(string contentJson, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                ReasonPhrase = string.Empty,
                Content = new StringContent(contentJson)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }



        public static void ThrowError(int statusID)
        {
            var json = GetStatusJson(statusID);

            throw new UserExceptions(json);
        }

        public static string GetStatusJson(int statusID)
        {
            var errorCodes = StaticData.ErrorCodes;
            var statusText = errorCodes.FirstOrDefault(x => x.ID == statusID).ErrorText;
            return "\"STATUS\": { \"ID\" :" + statusID + ", \"TEXT\" : \"" + statusText + "\"}";
        }

        //public static HttpResponseMessage Error(int statusCode, string statusText, HttpStatusCode httpStatusCode = HttpStatusCode.OK, object data = null)
        //{
        //    var status = new {ID = statusCode, TEXT = statusText};
        //    var resObj = new {DATA = data == null ? new { } : data, STATUS = status };
        //    //var json = new JavaScriptSerializer().Serialize(resObj);
        //    var json = JsonConvert.SerializeObject(resObj);

        //    var response = new HttpResponseMessage
        //    {
        //        StatusCode = httpStatusCode,
        //        ReasonPhrase = string.Empty,
        //        Content = new StringContent(json)
        //    };
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    return response;
        //}

        //public static HttpResponseMessage LogError(string errorText)
        //{
        //    var statusJson = "\"STATUS\":{ \"ID\" :" + -1 + ", \"TEXT\" : \"" + ConfigurationManager.AppSettings["error_text"] + "\"}";

        //    CommonFunctions.CatchExceptions(new Exception(errorText), string.Empty, false);

        //    return Error(statusJson, HttpStatusCode.InternalServerError);
        //}

        //public static void ThrowError(int statusID, string statusText)
        //{
        //    var json = "\"STATUS\":{ \"ID\" :" + statusID + ", \"TEXT\" : \"" + statusText + "\"}";

        //    throw new UserExceptions(json);
        //}

        //public static void ThrowError(int statusID)
        //{
        //    DataProviderManager<PKG_EXCEPTIONS>.Provider.Throw(statusID);
        //}
    }

    //class UppercaseContractResolver : DefaultContractResolver
    //{
    //    protected override string ResolvePropertyName(string propertyName)
    //    {
    //        return propertyName.ToUpper();
    //    }
    //}

    //class LowercaseContractResolver : DefaultContractResolver
    //{
    //    protected override string ResolvePropertyName(string propertyName)
    //    {
    //        return propertyName.ToLower();
    //    }
    //}
}