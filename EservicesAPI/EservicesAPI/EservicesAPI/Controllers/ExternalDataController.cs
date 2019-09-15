using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.OraDatabase.Models;
using EservicesLib.OraDatabase.DataSources;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Controllers
{
    [RoutePrefix("ExternalData")]

    public class ExternalDataController : MainController
    {
        #region XLS MODULE REGION

        [HttpPost]
        //[Route("GetTemplatesList")]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        public HttpResponseMessage GetTemplatesList()
        {
            var templates = DataProviderManager<PKG_EXTERNAL_DATA>.Provider.GetTemplatesList(AuthUser.UnID);
            return Success(templates);
        }

        [HttpPost]
        //[Route("GetLastUpload")]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        public HttpResponseMessage GetLastUpload([FromUri] int id)
        {
            var file = DataProviderManager<PKG_EXTERNAL_DATA>.Provider.GetLastUpload(AuthUser.UnID, id);
            FileData.FileName = id + "_template.xlsx";
            FileData.FileBytes = file;
            return FileData.DownloadFile();
        }

        [HttpPost]
        //[Route("GetTemplate")]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        public HttpResponseMessage GetTemplate([FromUri] int id)
        {
            var file= DataProviderManager<PKG_EXTERNAL_DATA>.Provider.GetTemplate(AuthUser.UnID, id);
            FileData.FileName = id + "_template.xlsx";
            FileData.FileBytes = file;
            return FileData.DownloadFile();
        }

        [HttpPost]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        //[Route("UploadFile")]

        public HttpResponseMessage UploadFile()
        {
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["uploadFile"];
            var tempID = context.Request.Params.Get("templateID").ToString().ToNumber<int>();
            JObject json = null;
            var response = new Excel().GetJsonFromExcel(postedFile);
            var fileName = postedFile.FileName;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(postedFile.InputStream))
            {
                fileData = binaryReader.ReadBytes(postedFile.ContentLength);
            }
            json = new JObject(new JProperty("EXDATA", JArray.Parse(response)));
  
            var i = 0;
            foreach (var x in json["EXDATA"])
            {
                i++;
            };
            if (i == 0) throw new UserExceptions("აირჩიეთ სწორი ფაილი");            
            DataProviderManager<PKG_EXTERNAL_DATA>.Provider.SaveTemplate(AuthUser.UnID, AuthUser.SubUserID, fileData, fileName, tempID, json.ToString());
            return Success(json);
        }

        #endregion

        #region JSON SERVICE REGION

        //[HttpPost]
        //[Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        //[Route("SendJson")]
        //public IHttpActionResult SendData([FromBody]  int TempID) 
        //{
        //    var response = DataProviderManager<PKG_EXTERNAL_DATA>.Provider.SendJsonData(AuthUser.UnID, TempID, AuthUser.SubUserID);
        //    return Ok(response);
        //}

        //[HttpPost]
        //[Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        //[Route("GetJson")]
        //public IHttpActionResult GetJson([FromBody] ED_Package data) 
        //{
        //    JObject json = null;
        //    if (data.JSON == "{}" || data.JSON == "[]" || data.TemplateID<=0 || data.JSON==null) 
        //    {
        //        return Ok(-1);
        //    }
        //    else if(data.JSON[0] == '[')
        //    { 
        //        json = new JObject(new JProperty("EXDATA", JArray.Parse(data.JSON)));
        //        DataProviderManager<PKG_EXTERNAL_DATA>.Provider.GetJson(AuthUser.UnID, AuthUser.SubUserID, data.TemplateID, json.ToString());
        //        return Ok(1);              
        //    }  
        //    else
        //    {
        //        return Ok(0);
        //    }
        //}

        #endregion

        #region CONTACT REGION

        [HttpPost]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        //[Route("GetContact")]
        public HttpResponseMessage GetContactInfo()
        {
            var contactes = DataProviderManager<PKG_EXTERNAL_DATA>.Provider.GetContactInfo(AuthUser.UnID);
            return Success(contactes);
        }

        [HttpPost]
        [Authenticate(Module = Modules.ExternalData, Permission = Permissions.Read)]
        //[Route("SaveContact")]
        public HttpResponseMessage SaveContactInfo([FromBody] ED_Contact contact)
        {
            if (contact != null)
            {
                DataProviderManager<PKG_EXTERNAL_DATA>.Provider.SaveContactInfo(AuthUser.UnID, contact.PHONE, contact.EMAIL);
                return Success("Info Saved");
            }
            return Success("");
        }
        #endregion

    }
}



