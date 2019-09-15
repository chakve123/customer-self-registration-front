using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using EservicesAPI.Common;
using EservicesLib.OraDatabase.StoredProcedures;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Filters
{
    public class RestrictDuplicateTransaction : ActionFilterAttribute
    {
        private int _UserId { get; set; }
        private bool _IsNewTransactionID { get; set; }
        private string _TransactionID { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var actionArgs = actionContext.ActionArguments;

            /*Request ში თუ პარამეტრი არ მოვიდა ე.ი. ვერც Transaction ID ვერ იქნება*/
            if (actionArgs.Count != 0)
            {
                /*request ის პარამეტრის object, რადგანაც ყველა request Post ტიპისაა მხოლოდ 1 Object ია და ქვედა კოდი სწორია !*/
                var requestParamsObj = actionContext.ActionArguments.FirstOrDefault().Value;

                _TransactionID = GetTransactionID(requestParamsObj);


                if (string.IsNullOrWhiteSpace(_TransactionID))
                {
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var authUser = actionContext.ControllerContext.Request.Properties["AuthUser"] as AuthUser;

                if (authUser == null)
                {
                    _UserId = -1;
                }
                else
                {
                    _UserId = authUser.ID;
                }

                var transactionVal = CheckTransactionID(_UserId, _TransactionID);

                _IsNewTransactionID = false;

                /*ე.ი. არ არსებობდა მსგავსი ჩანაწერი*/
                if (transactionVal.Item1 == null)
                {
                    _IsNewTransactionID = true;
                }
                
                if (!_IsNewTransactionID)
                {
                    var statusJson = string.Empty;
                    /*in Progress ია*/
                    if (transactionVal.Item1.Value == 1)
                    {
                        statusJson = ResponseBuilder.GetStatusJson(-7);

                        actionContext.Response = ResponseBuilder.Error(statusJson);
                        return;
                    }

                    if (transactionVal.Item1.Value == -1)
                    {
                        statusJson = ResponseBuilder.GetStatusJson(transactionVal.Item1.Value);

                        actionContext.Response = ResponseBuilder.Error(statusJson);
                        return;
                    }

                    /*თუ აქამდე მოვიდა ე.ი. status == 0 ანუ უკვე არსებობს და ვუბრუნებ result ს ბაზიდან*/
                    actionContext.Response = ResponseBuilder.GetResponseMessageByContentJson(transactionVal.Item2, HttpStatusCode.OK);
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            /*If it was new Trans. ID , write it in DB together with response as Value */

            base.OnActionExecuted(actionExecutedContext);

            if (actionExecutedContext.Exception != null)
            {
                DataProviderManager<PKG_TRANSACTION_LOG>.Provider.DELETE_TRANSACTION_RESPONSE(_UserId, _TransactionID);
                return;
            }

            if (_IsNewTransactionID)
            {
                DataProviderManager<PKG_TRANSACTION_LOG>.Provider.SET_TRANSACTION_RESPONSE(_UserId, _TransactionID, GetBodyFromRequest(actionExecutedContext));
            }
        }


        private string GetTransactionID(object requestParams)
        {
            var requestParamsJObj = requestParams as JObject;

            var searchSubKeys = "TransactionId";

            var res = string.Empty;

            if (requestParamsJObj != null)
            {
                var jObjValFromRequestParams = requestParamsJObj[searchSubKeys] as JToken;

                if (!(jObjValFromRequestParams == null || jObjValFromRequestParams is JArray))
                {
                    //jObjValFromRequestParams = jObjValFromRequestParams.Value<JToken>(searchSubKeys);
                    var jTokenFromRequestParams = jObjValFromRequestParams is JValue ? jObjValFromRequestParams : null;

                    if (jTokenFromRequestParams != null)
                    {
                        // var path = jTokenFromRequestParams.Path;

                        var value = (jTokenFromRequestParams as JValue).Value.ToString();
                        res = value;
                    }
                }
            }
            return res;
        }

        /// <summary>
        ///  აბრუნებს Null - ს თუ მსგავსი transactionID თი ჩანაწერი არ არსებობს, აბრუნებს string "Value" თუ უკვე არსებობს და შესაბამის status ს
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        private Tuple<int?, string> CheckTransactionID(int userId, string transactionID)
        {

            var dbRes = DataProviderManager<PKG_TRANSACTION_LOG>.Provider.GET_CREATE_TRANSACTION(userId, transactionID);

            return dbRes;
        }


        private string GetBodyFromRequest(HttpActionExecutedContext context)
        {
            string data;
            if (context.Response == null) return string.Empty;
            using (var stream = context.Response.Content.ReadAsStreamAsync().Result)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                data = context.Response.Content.ReadAsStringAsync().Result;
            }
            return data;
        }
    }
}