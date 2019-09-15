using BaseLib.Exceptions;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EservicesLib.OraDatabase.Models;
using Newtonsoft.Json.Linq;
namespace EservicesAPI.Controllers
{
    [RoutePrefix("UserParameters")]
    public class UserParametersController : MainController
    {
  
        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public IHttpActionResult GetPersonalInfo()
        //{
        //    //133366
        //    var personalInfo = AuthUser.SubUserID != 0 ?
        //        DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetSubPersonalInfo(AuthUser.ID, AuthUser.SubUserID)
        //        : DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetPersonalInfo(AuthUser.ID);
        //    return Ok(personalInfo);
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public void UpdateUserEmail([FromBody] string email)
        //{
        //    DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserEmail(AuthUser.ID,AuthUser.SubUserID,email);
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public void UpdateUserPhone([FromBody] string phone)
        //{
        //    DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserPhone(AuthUser.ID, AuthUser.SubUserID, phone);
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public int UpdateSecretWord([FromBody] string secretWord)
        //{
        //    var status = DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateSecretWord(AuthUser.ID, AuthUser.SubUserID, secretWord);
        //    return status;
        //}
        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public void UpdateUserAddress([FromBody] JObject JData)
        //{

        //    if (AuthUser.SamFormaType == SamformaType.Individual)
        //    {
        //        DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserAddress(AuthUser.UnID, JData.GetValue("region").ToString(), JData.GetValue("district").ToString(), JData.GetValue("streets").ToString()
        //        , JData.GetValue("placenumber").ToString());
        //    }
        //     else throw new UserExceptions("თქვენ არ გაქვთ ამ ოპერაციის განხორციელების უფლება");
        //}
        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public Dictionary<string, object> GetRegions([FromBody]string regionId)
        //{
        //    var data = new Dictionary<string, object>();
        //    data.Add("Regions", DataProviderManager<PKG_REGION>.Provider.GetAddressList(regionId));
        //    return data;
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public Dictionary<string, object> GetDistricts([FromBody]string regionId)
        //{
        //    var data = new Dictionary<string, object>();
        //    data.Add("Districts", DataProviderManager<PKG_REGION>.Provider.GetAddressList(regionId));
        //    return data;
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public Dictionary<string, object> GetStreets([FromBody] JObject JData)
        //{
        //    var data = new Dictionary<string, object>();
        //    if (string.IsNullOrEmpty(JData.GetValue("value").ToString()))
        //        throw new UserExceptions("აირჩიეთ რაიონი");

        //    data.Add("Streets", DataProviderManager<PKG_ORG_INFO>.Provider.GetStreets(JData.GetValue("value").ToString(), JData.GetValue("text").ToString()));
        //    data.Add("Officer", DataProviderManager<PKG_ORG_INFO>.Provider.GetOficerInfo(JData.GetValue("regionId").ToString(), JData.GetValue("value").ToString()));
        //    return data;
        //}
        //[HttpPost]
        //[Authenticate(Module = Modules.UserParameters, Permission = Permissions.Read)]
        //public Dictionary<string, object> GetAddressDetails()
        //{
        //    var result = new Dictionary<string, object>();
        //    result.Add("AddressDetails", DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetAddressDetails(AuthUser.UnID));
        //    //result.Add("region", RsDataProviderManager<PKG_REGION>.Provider.GetAddressList());
        //    return result;
        //}

       


    }
}