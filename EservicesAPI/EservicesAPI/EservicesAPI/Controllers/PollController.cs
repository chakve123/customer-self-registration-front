using BaseLib.Common;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using EservicesLib.OraDatabase.StoredProcedures;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Controllers
{
    public class PollController : MainController
    {
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage SendFeedBack([FromBody]JObject json)
        {
            try
            {
                //Find IP Address Behind Proxy Or Client Machine In ASP.NET
                string IPAdd = string.Empty;
                IPAdd = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(IPAdd))
                {
                    IPAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                int modID = getModuleID(json.GetValue("URL").ToString());

           
                    DataProviderManager<PKG_POLL>.Provider.insert_FeedBack(modID, json.GetValue("COMMENT").ToString(), json.GetValue("NAME").ToString() + " : " + json.GetValue("CONTACT").ToString(), AuthUser.ID, AuthUser.SubUserID, IPAdd, json.GetValue("WEB_PAGE").ToString());
                //else
                //    DataProviderManager<PKG_POLL>.Provider.insert_FeedBack(modID, comment, name + " : " + contact, null, null, IPAdd, webPage);

                return Success(1);
            }
            catch (Exception ex)
            {
                CommonFunctions.CatchExceptions(ex);
                return Success(0);
            }
        }

        private int getModuleID(string url)
        {

            string pageName = url.ToLower();
            //pageName = url.Substring(url.LastIndexOf("/") + 1, url.IndexOf(".aspx") - url.LastIndexOf("/") - 1).ToLower();
            switch (pageName)
            {
                case "waybills":
                    return 5;
                case "notifications":
                    return 10;
                case "invoice":
                    return 3;
                case "payerinfo":
                    return 28;
                case "agslocks":
                    return 7;
            }
            return 0;
        }
    }
}
