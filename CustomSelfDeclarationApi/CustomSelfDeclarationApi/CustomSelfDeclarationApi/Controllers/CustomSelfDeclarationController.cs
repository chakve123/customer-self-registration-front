using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using BaseLib.OraDataBase;
using CustomSelfDeclarationLib.OraDatabase.StoredProcedures;

namespace CustomSelfDeclarationApi.Controllers
{
    public class CustomSelfDeclarationController : ApiController 
    {
        EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");

        [HttpPost]
        public IHttpActionResult GetUserInfo([FromBody]Dictionary<string,object> request)
        {
            var userInfo = DataProviderManager<PKG_PERSON_INFO>.Provider.get_person_info(request["PersonID"].ToString());

            return Ok(userInfo);
        }
        [HttpPost]
        public IHttpActionResult GetGuestInfo([FromBody]Dictionary<string, object> request)
        {
            var userInfo = DataProviderManager<PKG_PERSON_INFO>.Provider.get_guest_info(request["PersonID"].ToString());

            return Ok(userInfo);
        }
    }
}
