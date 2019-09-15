using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BaseLib.Attributes;
using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesAPI.Filters;
//using EservicesAPI.InvoiceWoodSrv;
using EservicesLib.Models;
using EservicesLib.Models.Invoice;
using EservicesLib.OraDatabase.DataSources;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static EservicesAPI.Controllers.CommonController;
using static EservicesAPI.Controllers.Excel;
using DateTime = System.DateTime;

namespace EservicesAPI.Controllers
{
 // [RoutePrefix("Notifications")]
    public class NotificationsController : MainController
    {

        #region Grid


        [HttpPost]
        [Authenticate]
        [RequestLog(Module = Modules.Invoice)]
        public HttpResponseMessage GrdNotifications([FromBody] GridData gridData)
        {
            var rf = new FilterExpression();

            if ((gridData.View == "tab_notif" || gridData.View == "tab_deleted" || gridData.View == "tab_fav") && gridData.FilterExpression.Find(p => (FilterFunc)p.Func == FilterFunc.Between) == null && !gridData.IgnorePeriod)
            {
                rf.DataType = (int)DataType.tpDate;
                rf.FieldName = "NOTIF_DATE";
                rf.Func = (int)FilterFunc.Between;
                rf.FilterValue = new PeriodControl(60).ClassToDictionary();
                gridData.FilterExpression.Add(rf);
            }

            var unId = 0;

            if (AuthUser.IsEmployee)
            {
                var person = AuthUser.GetActivePerson;
                if (person != null)
                    unId = person.UnID;

                switch (gridData.NotifView)
                {
                    case "tab_newnotif":
                        gridData.Criteria = "UN_ID = :Param1 and ACTIVE = 'Y' ";
                        gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", unId, CustomOracleDbType.Int32));
                        break;
                    case "tab_notif":
                        gridData.Criteria = "UN_ID = :Param1 and ACTIVE <> 'D'";
                        gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", unId, CustomOracleDbType.Int32));
                        break;
                    case "tab_deleted":
                        gridData.Criteria = "UN_ID = :Param1 and ACTIVE = 'D'";
                        gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", unId, CustomOracleDbType.Int32));
                        break;
                    case "tab_fav":
                        gridData.Criteria = "UN_ID = :Param1 and STATUS = 'F'";
                        gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", unId, CustomOracleDbType.Int32));
                        break;
                }
            }
            else
            {
                switch (gridData.NotifView)
                {
                    case "tab_newnotif":
                        gridData.Criteria = string.Format(" UN_ID In ({0}) and ACTIVE = 'Y' ", GetUnionUnIdis(gridData));
                        break;
                    case "tab_notif":
                        gridData.Criteria = string.Format(" UN_ID In ({0}) and ACTIVE <> 'D' ", GetUnionUnIdis(gridData));
                        break;
                    case "tab_deleted":
                        gridData.Criteria = string.Format(" UN_ID In ({0}) and ACTIVE = 'D' ", GetUnionUnIdis(gridData));
                        break;
                    case "tab_fav":
                        gridData.Criteria = string.Format(" UN_ID In ({0}) and STATUS = 'F' ", GetUnionUnIdis(gridData));
                        break;
                }
            }



           var res = gridData.GetDataJson<dsNotifications>();
            return Success(res);
        }

        [HttpPost]
        [Authenticate]
        [RestrictDuplicateTransaction]
        public HttpResponseMessage TestTransactions([FromBody] JObject dto)
        {
            // ეს ადგილი თუ გაატარე ვაბრუნებ 1 - ს;
            // ისე RestrictDuplicateTransaction ში თუ გაიჭედა ჯერ ვნახოთ
            //ThrowError(-204);
            //DataProviderManager<PKG_TRANSACTION_LOG>.Provider.MethodWhichThrowsExceptionTest();
            return Success(new TestClass());
           //  return Success(1);
        }
        //[HttpPost]
        //[Authenticate]
        //public HttpResponseMessage GetNotificationDetails([FromBody]GetNotifDetailsParam param)
        //{

        //    var data = new Dictionary<string, object>();

        //    var unID = 0;
        //    if (AuthUser.CurrentUser.IsEmployee)
        //    {
        //        var person = AuthUser.CurrentUser.GetActivePerson;
        //        if (person != null)
        //            unID = person.UnID;
        //    }
        //    else
        //        unID = AuthUser.CurrentUser.UnID;

        //    var notificationData = DataProviderManager<PKG_NOTIFY>.Provider.get_notif_details(unID,
        //        param.NotificationID, AuthUser.CurrentUser.IsEmployee ? 1 : 0, param.NotificationIdM);

        //    data.Add("Notification", notificationData);
        //    data.Add("IsEmp", AuthUser.CurrentUser.IsEmployee);

        //    return data;
        //}

        private string GetUnionUnIdis(GridData gridData)
        {
            string unIds = string.Empty;
            //cp.Add(new DataSourceCriteriaParameters("UParam1", AuthUser.CurrentUser.UnID, CustomOracleDbType.Int32));

            int paramCount = 1;
            if (AuthUser.CurrentUser.UnionOrgs != null)
            {
                foreach (var unionOrg in (AuthUser.CurrentUser as AuthUser).UnionOrgs.FindAll(f => f.IsAvtive))
                {
                    string param = "UParam" + ++paramCount;
                    if (string.IsNullOrEmpty(unIds))
                        unIds = string.Format(":{0}", param);
                    else
                        unIds += string.Format(",:{0}", param);

                    gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters(param, unionOrg.UnID, CustomOracleDbType.Int32));
                }
            }

            return unIds;
        }



        public struct GetNotifDetailsParam
        {
            public int NotificationID { get; set; }
            public int NotificationIdM { get; set; }
        }

        #endregion
        
    }


    public class TestClass
    {
        public int Test { get; set; }

        public string Test1 { get; set; }

        public bool Test2 { get; set; }

        public DateTime Test3 { get; set; }

        public TimeSpan Test4 { get; set; }
        public object Test5 { get; set; }

        public TestClass()
        {
            Test = -1;
            Test1 = "666";
            Test2 = true;
            Test3 = DateTime.Now;
            Test4 = TimeSpan.FromSeconds(3);
        }
    }
}
