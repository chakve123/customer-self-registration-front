using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.OraDatabase.DataSources;
using EservicesLib.OraDatabase.StoredProcedures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EservicesAPI.Controllers
{
    public class TaxPayerController : MainController
    {
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GrdSearchTaxPayers([FromBody] GridData gridData)
        {
            if ((!gridData.FilterExpression.Exists(f => f.FieldName == "SAID_KODI") || gridData.FilterExpression.Find(f => f.FieldName == "SAID_KODI").FilterValue == "") && (!gridData.FilterExpression.Exists(f => f.FieldName == "DASAXELEBA") || gridData.FilterExpression.Find(f => f.FieldName == "DASAXELEBA").FilterValue == ""))
            {
                ThrowError(-12);
            }
            gridData.GetData<dsSearchTaxPayers>();
            return Success(gridData.Data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage PrintReport([FromBody]JObject json)
        {
            var file=new byte[0];
            CardType getPayerType=CardType.Default;
            switch (json.GetValue("SAM_FORMA").ToString())
            {
                case "01":
                    getPayerType = CardType.Registry;
                    break;
                case "02":
                    getPayerType = CardType.Registry;
                    break;
                case "03":
                    getPayerType = CardType.Registry;
                    break;
                case "04":
                    getPayerType = CardType.Registry;
                    break;
                case "05":
                    getPayerType = CardType.Registry;
                    break;
                case "06":
                    getPayerType = CardType.Registry;
                    break;
                case "07":
                    getPayerType = CardType.Registry;
                    break;
                case "15":
                    getPayerType = CardType.Registry;
                    break;
                case "26":
                    getPayerType = CardType.Registry;
                    break;
                case "27":
                    getPayerType = CardType.Registry;
                    break;
                case "28":
                    getPayerType = CardType.Registry;
                    break;
                default:
                    getPayerType = CardType.Other;
                    break;
            }

            if (getPayerType == CardType.Default)
                ThrowError(-13);
            
            switch (getPayerType)
            {
                case CardType.Registry:  file= DataProviderManager<PKG_SEARCHTAXPAYERS>.Provider.reportTypeReg(json.GetValue("UN_ID").ToString().ToNumber<int>()); break;
                case CardType.Other: file = DataProviderManager<PKG_SEARCHTAXPAYERS>.Provider.reportTypeOther(json.GetValue("UN_ID").ToString().ToNumber<int>()); break;
            }

            FileData.FileBytes = file;
            FileData.FileName = "Report.pdf";
            return FileData.DownloadFile();
        }
    }
    public enum CardType
    {
        Registry,
        Other,
        Default

    }
}

