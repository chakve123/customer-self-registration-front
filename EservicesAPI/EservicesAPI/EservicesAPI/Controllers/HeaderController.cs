using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.Common;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace EservicesAPI.Controllers
{
    public class HeaderController : MainController
    {

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetHeaderInfo()
        {
            var result = new Dictionary<string, object>();
            result.Add("Tin", AuthUser.Tin);
            result.Add("Name", AuthUser.FullName);
            result.Add("notificationIconVisible", !AuthUser.IsRemoteRegisterer);
            result.Add("personalInfoVisible", !(AuthUser.IsRemoteRegisterer) && !(AuthUser.SubUserID != 0 && AuthUser.Modules.FindAll(s => s.ID == 17).Count <= 0));
            return Success(result);
        }


        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ProccessPermissions()
        {
            var result = new Dictionary<string, object>();
            var trustingUsers = new Dictionary<string, object>();
            var list = new List<object>();
            var data = new Dictionary<string, object>
            {
                { "PsCommonVisible", !(AuthUser.IsRemoteRegisterer) },
                { "divInterflowVisible",!((AuthUser.SubUserID > 0 && AuthUser.TrustingUsers.Count(x => x.UserType == 0) == 0) || AuthUser.IsRemoteRegisterer)},
                { "PsCommunicationVisible",!((AuthUser.SubUserID > 0 && AuthUser.TrustingUsers.Count(x => x.UserType == 0) == 0) || AuthUser.IsRemoteRegisterer)} ,
                { "divOrgInfoVisible",!(AuthUser.IsRemoteRegisterer) && AuthUser.SubUserID == 0 && (AuthUser.SamFormaType == SamformaType.Company || AuthUser.SamFormaType == SamformaType.Industrialist)},
                { "divRepresentativesVisible", AuthUser.SubUserID == 0 },
                { "divSubUserVisible", AuthUser.SubUserID == 0 && !AuthUser.IsRemoteRegisterer },
                { "divSubscribeVisible", AuthUser.HasPermission(Modules.UserParameters) && AuthUser.SubUserID == 0 && AuthUser.UserType != 3 },
                { "divUserParametersVisible",!(AuthUser.IsRemoteRegisterer) },
                { "switchIconVisible", AuthUser.TrustingUsers != null && ((AuthUser.SubUserID > 0 && AuthUser.TrustingUsers.Count > 0) || AuthUser.TrustingUsers.Count > 1) },
                { "userCount",StaticData.UsersCount },
                {"PsUserInfoVisible" ,!(AuthUser.SubUserID != 0 && AuthUser.Modules.FindAll(s => s.ID == 17).Count <= 0)},
                {"switcherUserVisible", AuthUser.TrustingUsers!=null},
                {"isRemoteRegisterer",AuthUser.IsRemoteRegisterer },
                {"headerSamForma"  , !(AuthUser.IsRemoteRegisterer) ?(AuthUser.SamFormaID == 14 ? AuthUser.SamFormaName : AuthUser.SamForma) : "თანამშრომელი" },
                {"headerSamFormaStyle" , !(AuthUser.IsRemoteRegisterer) ? (AuthUser.SubUserID > 0 ? "#4EBD49" : "#2A75FF") : "#616161"}
            };

            if (AuthUser.TrustingUsers != null)
            {
                foreach (var user in AuthUser.TrustingUsers.Where(s => s.UserID != AuthUser.ID))
                {
                    dynamic obj = new ExpandoObject();

                    string name;
                    var title = "";
                   
                    if (user.FullName.Length > 23)
                    {
                        name = user.FullName.Remove(23) + "...";
                        title = "title = '" + user.FullName + "'";
                    }
                    else
                    {
                        name = user.FullName;
                    }

                    ((IDictionary<String, Object>)obj)["name"] = name;
                    ((IDictionary<String, Object>)obj)["title"] = title;
                    ((IDictionary<String, Object>)obj)["UserTin"] = user.Tin;
                    ((IDictionary<String, Object>)obj)["SamFormaName"] = user.SamFormaName;
                    ((IDictionary<String, Object>)obj)["UserID"] = user.UserID;
                    ((IDictionary<String, Object>)obj)["UserSamformaID"] = user.SamFormaID;
                    ((IDictionary<String, Object>)obj)["UserType"] = user.UserType;
                    ((IDictionary<String, Object>)obj)["SamForma"] = user.SamForma;
                    list.Add(obj);
                }
            }

            result.Add("DATA",data);
            result.Add("TrustingUsers", list);
            return Success(result);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetImage([FromBody]string Tin)
        {
            var res = DataProviderManager<PKG_USERS>.Provider.get_image_blob(Tin ?? AuthUser.Tin);
            var data = new Dictionary<string, bool>();

            if (res == null)
            {
                data.Add("IS_NULL", true);
                return Success(data);
            }
            var encodedImgBytes = Convert.ToBase64String(res);

            return Success(encodedImgBytes);
        }


    }
}
