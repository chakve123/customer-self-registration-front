using System;
using System.Collections;
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
using System.Reflection;
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

namespace EservicesAPI.Controllers
{
    //Invoice Controller
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    // [RoutePrefix("Invoice")]
    public class InvoiceController : MainController
    {
        #region Actions
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Activate)]
        public HttpResponseMessage ActivateInvoices(JObject json)
        {
            DataProviderManager<PKG_INVOICE>.Provider.save_invoices(json.ToString(), AuthUser.ID, AuthUser.SubUserID, 1);
            return Success();
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Confirm)]
        public HttpResponseMessage ConfirmInvoices(JObject json)
        {
            DataProviderManager<PKG_INVOICE>.Provider.save_invoices(json.ToString(), AuthUser.ID, AuthUser.SubUserID, 2);
            return Success();
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Confirm)]
        public HttpResponseMessage RefuseInvoices(JObject json)
        {
            DataProviderManager<PKG_INVOICE>.Provider.save_invoices(json.ToString(), AuthUser.ID, AuthUser.SubUserID, 3);
            return Success();
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.CreateEdit)]
        public HttpResponseMessage GetSeqNum(JObject json)
        {
            var response = new Dictionary<string, object>();

            var operationPeriod = json["OperationPeriod"].ToString();
            var seqNum = DataProviderManager<PKG_INVOICE>.Provider.get_seq_num(operationPeriod, AuthUser.UnID, AuthUser.ID);
            response.Add("SeqNum", seqNum);
            return Success(response);
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.CreateEdit)]
        public HttpResponseMessage CreateDecl(JObject json)
        {
            DataProviderManager<PKG_INVOICE>.Provider.create_decl(AuthUser.UnID, AuthUser.ID, AuthUser.SubUserID, json.ToString());
            return Success();
        }
        [HttpPost]
        //[Route("GetBarCode")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetBarCode([FromBody] Dictionary<string, object> json)
        {
            var barCode = json["barCode"].ToString();
            var response = DataProviderManager<PKG_INVOICE>.Provider.GetBarcode(AuthUser.UnID, barCode);
            return Success(JObject.Parse(response));
        }

        [HttpPost]
        //[Route("ClearBarCodes")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage ClearBarCodes()
        {
            try
            {
                DataProviderManager<PKG_INVOICE>.Provider.ClearBarCodes(AuthUser.UnID);
            }
            catch (Exception ex)
            {
                //throw new UserExceptions("ბარკოდების ბაზა ვერ გასუფთავდა");
                ThrowError(-5);
            }

            return Success();
        }

        [HttpPost]
        //[Route("GetNotifications")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetNotifications()
        {
            string response = DataProviderManager<PKG_INVOICE>.Provider.GetNotificationsJson(AuthUser.UnID, AuthUser.SubUserID);
            return Success(JObject.Parse(response));
        }

        [HttpPost]
        //[Route("GetOrgObjAddress")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetOrgObjAddress([FromBody] Dictionary<string, object> json)
        {
            var obIdentNo = string.Empty;

            if (json != null && json.ContainsKey("obIdentNo")) obIdentNo = json["obIdentNo"].ToString();

            var objAddress = DataProviderManager<PKG_INVOICE>.Provider.get_org_obj_address(obIdentNo);
            return Success(objAddress);
        }

        [HttpPost]
        //[Route("GetInvoice")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetInvoice([FromBody] JObject JInvoice)
        {
            var userID = AuthUser.ID;
            int InvoiceID = -1;
            int InvoiceNumber = -1;
            if (JInvoice.GetValue("InvoiceID") != null)
                InvoiceID = JInvoice.GetValue("InvoiceID").ToString().ToNumber<int>();
            if (JInvoice.GetValue("InvoiceNumber") != null)
                InvoiceNumber = JInvoice.GetValue("InvoiceNumber").ToString().ToNumber<int>();

            if (InvoiceID == -1 && InvoiceNumber == -1) ThrowError(-1);

            var invoice = DataProviderManager<PKG_INVOICE>.Provider.GetInvoice(InvoiceID, InvoiceNumber, userID, AuthUser.SubUserID, JInvoice.GetValue("parentInvoiceID").ToString().ToNumber<int>());
            //invoice = invoice.Replace('"','a');
            //invoice = invoice.Replace(@"\",@"");
            var json = JObject.Parse(invoice);

            return Success(json);
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.CreateEdit)]
        [RestrictDuplicateTransaction]
        public HttpResponseMessage SaveInvoice([FromBody] JObject InvoiceJson)
        {
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.SaveInvoiceStatus(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
            return InvoiceAction(InvoiceJson);
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Activate)]
        [RestrictDuplicateTransaction]
        public HttpResponseMessage ActivateInvoice([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.ActivateInvoice(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Cancel)]
        public HttpResponseMessage CancelInvoice([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.CancelInvoice(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Delete)]
        public HttpResponseMessage DeleteInvoice([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            var userID = AuthUser.ID;
            var subUserID = AuthUser.SubUserID;
            var invoiceJsonStr = InvoiceJson.ToString();
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.DeleteInvoice(userID, subUserID, invoiceJsonStr)));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Confirm)]
        public HttpResponseMessage ConfirmInvoice([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            var userID = AuthUser.ID;
            var subUserID = AuthUser.SubUserID;
            var invoiceJsonStr = InvoiceJson.ToString();
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.ConfirmInvoice(userID, subUserID, invoiceJsonStr)));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Confirm)]
        public HttpResponseMessage RefuseInvoice([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.RefuseInvoice(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.CreateEdit)]
        [RestrictDuplicateTransaction]
        public HttpResponseMessage SaveInvoiceTemplate([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            if (InvoiceJson["INVOICE"]["TEMPLATE_NAME"] == null || InvoiceJson["INVOICE"]["TEMPLATE_NAME"].ToString().Trim().Length == 0)
                ThrowError(-210);
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.SaveInvoiceTemplate(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Delete)]
        public HttpResponseMessage DeleteInvoiceTemplate([FromBody] JObject InvoiceJson)
        {
            InvoiceAction(InvoiceJson);
            return Success(JObject.Parse(DataProviderManager<PKG_INVOICE>.Provider.DeleteInvoiceTemplate(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString())));
        }
        private HttpResponseMessage InvoiceAction([FromBody] JObject InvoiceJson)
        {

            // Validations
            var InvoiceDetails = new InvoiceDetails();
            try
            {
                JsonToFieldObject((JObject)InvoiceJson["INVOICE"], InvoiceDetails);
            }
            catch (Exception ex)
            {
                DataProviderManager<PKG_INVOICE>.Provider.log_api_debug(InvoiceJson.ToString(), "Parser Error: " + ex.Message.ToString());
            }
            InvoiceDetails.TIN_BUYER.isValid();
            if (InvoiceDetails.SELLER_ACTION.value != -2) // შაბლონს ვალიდაცია არ აქვს
            {
                try
                {
                    Field<string>.validateAll(InvoiceDetails, null, InvoiceJson.ToString());
                }
                catch (Exception ex)
                {
                    DataProviderManager<PKG_INVOICE>.Provider.log_api_debug(InvoiceJson.ToString(), "Function Error: " + ex.Message.ToString());
                }
            }
            // Validations

            //return new HttpResponseMessage();
            var invoice = InvoiceJson["INVOICE"];
            if (invoice["TIN_SELLER"] != null && invoice["TIN_BUYER"] != null)
                if (!string.IsNullOrEmpty(invoice["TIN_SELLER"].ToString()) && invoice["TIN_SELLER"].ToString() == invoice["TIN_BUYER"].ToString())
                    //throw new UserExceptions("გამყიდველის და მყიდველის საიდენტიფიკაციო ნომერი არ უნდა ემთხვეოდეს.");
                    ThrowError(-200);

            if (invoice["TRANS_COMPANY_TIN"] != null && invoice["TIN_BUYER"] != null)
                if (!string.IsNullOrEmpty(invoice["TIN_BUYER"].ToString()) && invoice["TIN_BUYER"].ToString() == invoice["TRANS_COMPANY_TIN"].ToString())
                    //throw new UserExceptions("მყიდველის და გადამზიდავი კომპანიის საიდენტიფიკაციო ნომერი არ უნდა ემთხვეოდეს");
                    ThrowError(-202);

            //var response = DataProviderManager<PKG_INVOICE>.Provider.SaveInvoice(AuthUser.ID, AuthUser.SubUserID, InvoiceJson.ToString());

            return Success();
        }

        [HttpPost]
        //[Route("StartOilTransport")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.CreateEdit)]
        public HttpResponseMessage StartOilTransport([FromBody] Dictionary<string, object> json)
        {
            var invoiceID = json["invoiceID"].ToString().ToNumber<decimal>();
            DataProviderManager<PKG_INVOICE>.Provider.StartOilTransport(invoiceID, AuthUser.ID);
            return Success();
        }

        #endregion

        #region Upload , Download & Export
        [HttpPost]
        //[Route("ExportGrdInvoice")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public Dictionary<string, object> ExportGrdInvoice([FromBody] GridData gridData)
        {
            return null;
            gridData.DataExportType = ExportType.ExportXls;
            var fields = gridData.ExportFieldsForm;
            gridData.ExportFields = new Dictionary<string, string>();
            for (int i = 0; i < fields.Count; i++)
            {
                gridData.ExportFields.Add(fields[i].field, fields[i].header);
            }

            //gridData.ExportFields.Add("INV_TYPE_NAME", "ოპერაციის ტიპი
            gridData.MaximumRows = 20000;
            var x = GrdInvoices(gridData);
            return null;

            //var myBytes = new Excel().CreateExcel(testDt, null);
            //FileData.FileName = "new_excel.xlsx";
            //FileData.FileBytes = myBytes;
            //FileData.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //return FileData.DownloadFile();
        }


        [HttpPost]
        //[Route("DownloadInvoiceReport")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage DownloadInvoiceReport([FromUri] string id)
        {

            var file = DataProviderManager<PKG_INVOICE>.Provider.GetInvoiceReport(id.Trim(','), AuthUser.UnID);

            //FileData.FileName = id + ".pdf";
            FileData.FileName = "report.pdf";
            FileData.FileBytes = file;
            return FileData.DownloadFile();
        }
        [HttpPost]
        //[Route("ImportBarCodesExcel")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public void ImportBarCodesExcel()
        {
            //var x = PostedFiles.GetPostedFile(Request);
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["uploadFile"];

            var ExcelJson = new Excel().GetJsonFromExcel(postedFile);

            Dictionary<string, JArray> barcodes = new Dictionary<string, JArray>();

            var json = new JObject(new JProperty("BARCODES", JArray.Parse(ExcelJson)));

            var i = 0;
            foreach (var x in json["BARCODES"])
            {
                i++;
                //if (i == 1)
                //{
                //    if (String.IsNullOrEmpty(x["col2"].ToString()))
                //        throw new UserExceptions("გთხოვთ განაახლოთ 'შტრიხკოდების რეესტრის ნიმუში'");
                //}

            }
            if (i == 0) //throw new UserExceptions("აირჩიეთ სწორი ფაილი");
                ThrowError(-203);
            DataProviderManager<PKG_INVOICE>.Provider.SaveBarcodes(AuthUser.UnID, json.ToString());

        }


        [HttpPost]
        //[Route("DownloadBarCodes")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage DownloadBarCodes()
        {
            var data = DataProviderManager<PKG_INVOICE>.Provider.download_barcode(AuthUser.UnID);
            var templatePath = HttpContext.Current.Request.PhysicalApplicationPath + "Docs\\invoice_barcodes.xls";

            var exTable = new ExcelDataTable();

            exTable.Sheet = "Sheet1";
            exTable.ColumnsWithFieldNames = new Dictionary<ExcelColumns, string>();

            exTable.ColumnsWithFieldNames.Add(ExcelColumns.A, "barcode");
            exTable.ColumnsWithFieldNames.Add(ExcelColumns.B, "goods_name");
            exTable.ColumnsWithFieldNames.Add(ExcelColumns.E, "unit_txt");
            exTable.ColumnsWithFieldNames.Add(ExcelColumns.F, "other_unit_txt");
            var excel = new Excel();
            var fileBytes = excel.CreateExcel(data, templatePath, exTable);
            FileData.FileName = "invoice_barcodes.xls";
            FileData.FileBytes = fileBytes;

            return FileData.DownloadFile();

        }

        [HttpPost]
        //[Route("DownloadBarCodesTemplate")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage DownloadBarCodesTemplate()
        {
            var template = CommonFunctions.ReadFile(HttpContext.Current.Request.PhysicalApplicationPath + "Docs\\invoice_barcodes.xls");
            FileData.FileBytes = template;
            FileData.FileName = "invoice_barcodes.xls";
            return FileData.DownloadFile();
        }


        [HttpPost]
        //[Route("DownloadGoodsTemplate")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage DownloadGoodsTemplate()
        {
            var template = CommonFunctions.ReadFile(HttpContext.Current.Request.PhysicalApplicationPath + "Docs\\invoice_goods.xls");
            FileData.FileBytes = template;
            FileData.FileName = "invoice_goods.xls";
            return FileData.DownloadFile();
        }



        [HttpPost]
        //[Route("ImportGoodsExcel")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public JObject ImportGoodsExcel()
        {
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["uploadFile"];

            var ExcelJson = new Excel().GetJsonFromExcel(postedFile);

            Dictionary<string, JArray> barcodes = new Dictionary<string, JArray>();

            var json = new JObject(new JProperty("INVOICE_GOODS", JArray.Parse(ExcelJson)));

            var invoiceGoods = new List<xmlInvoiceGoods>();

            var i = 0;
            foreach (var x in json["INVOICE_GOODS"])
            {
                var unit_id = string.IsNullOrEmpty(x["col2"].ToString()) ? 99 : x["col2"].ToString().ToNumber<int>();

                invoiceGoods.Add(new xmlInvoiceGoods()
                {
                    GOODS_NAME = x["col0"].ToString(),
                    BARCODE = x["col1"].ToString(),
                    UNIT_ID = unit_id,
                    UNIT_TXT = unit_id == 99 ? x["col4"].ToString() : x["col3"].ToString(),
                    QUANTITY = x["col5"].ToString().ToNumber<double>(),
                    UNIT_PRICE = x["col6"].ToString().ToNumber<double>(),
                    AMOUNT = x["col7"].ToString().ToNumber<double>(),
                    VAT_TYPE = x["col8"].ToString().ToNumber<decimal>()
                }
                );
                i++;
            }
            if (i == 0)
                //throw new UserExceptions("აირჩიეთ სწორი ფაილი");
                ThrowError(-205);
            var newJson = new JObject(new JProperty("INVOICE_GOODS", JArray.Parse(JsonConvert.SerializeObject(invoiceGoods))));
            return newJson;
        }
        #endregion;

        #region Grid

        [HttpPost]
        //[Route("GrdBarCodes")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdBarCodes([FromBody] GridData gridData)
        {
            gridData.Criteria = string.Format("UN_ID IN ({0}) ", AuthUser.UnID);
            gridData.GetData<dsInvoiceBarCodes>();
            return Success(gridData.Data);
        }

        [HttpPost]
        //[Route("GrdInvoiceOilDocs")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdInvoiceOilDocs([FromBody] GridData gridData)
        {
            gridData.GetData<dsInvoiceOilDoc>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdInvoiceGoods")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdInvoiceGoods([FromBody] GridData gridData)
        {
            gridData.GetData<dsInvoiceGoods>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdOilProducts")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOilProducts([FromBody] GridData gridData)
        {
            gridData.GetData<dsOilProducts>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdExcise")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdExcise([FromBody] GridData gridData)
        {
            if (gridData.View == "v_invoice_service")
                gridData.Criteria = "ID in (309,310)";
            gridData.GetData<dsExcise>();
            return Success(gridData.Data);
        }

        [HttpPost]
        //[Route("GrdOrgObjStRetail")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjStRetail([FromBody] GridData gridData)
        {
            //საცალო INSTR(ob_ident_no,15) <> 1 AND INSTR(ob_ident_no,19) <> 1)
            gridData.GetData<dsOrgObjStRetail>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdOrgObjTrInt")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjTrInt([FromBody] GridData gridData)
        {
            //შიდა გადაზიდვა INSTR(ob_ident_no,16) <> 1"
            gridData.GetData<dsOrgObjTrInt>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdOrgObjTrStWhole")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjTrStWhole([FromBody] GridData gridData)
        {
            //საბითუმო  INSTR(ob_ident_no,16) <> 1 AND INSTR(ob_ident_no,19) <> 1
            gridData.GetData<dsOrgObjTrStWhole>();
            return Success(gridData.Data);
        }

        [HttpPost]
        //[Route("GrdOrgObjectsImport")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjectsImport([FromBody] GridData gridData)
        {
            gridData.GetData<dsOrgObjectsImport>();
            return Success(gridData.Data);
        }

        [HttpPost]
        //[Route("GrdOrgObjTrEndRetail")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjTrEndRetail([FromBody] GridData gridData)
        {
            // საცალო INSTR(ob_ident_no,16) <> 1 AND INSTR(ob_ident_no,19) <> 1
            gridData.GetData<dsOrgObjTrEndRetail>();
            return Success(gridData.Data);
        }
        [HttpPost]
        //[Route("GrdOrgObjTrEndWhole")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdOrgObjTrEndWhole([FromBody] GridData gridData)
        {
            // საბითუმო  INSTR(ob_ident_no,16) <> 1 AND INSTR(ob_ident_no,19) <> 1
            gridData.GetData<dsOrgObjTrEndWhole>();
            return Success(gridData.Data);
        }
        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdHistory([FromBody] GridData gridData)
        {
            gridData.Criteria = "USER_ID_SELLER = :Param1";
            //gridData.Criteria = "TIN_SELLER = :Param2";
            gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", AuthUser.ID, CustomOracleDbType.Int32));
            //gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param2", AuthUser.Tin, CustomOracleDbType.Int32));

            switch (gridData.View)
            {
                case "BuyerHistory":
                    gridData.GetData<dsBuyerHistory>();
                    break;
                case "DriverHistory":
                    gridData.GetData<dsDriverHistory>();
                    break;
                case "StartAddressHistory":
                    gridData.GetData<dsStartAddressHistory>();
                    break;
                case "EndAddressHistory":
                    gridData.GetData<dsEndAddressHistory>();
                    break;
            }
            return Success(gridData.Data);
        }

        [HttpPost]
        //[Route("GrdInvoices")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GrdInvoices([FromBody] GridData gridData)
        {
            var rf = new FilterExpression();

            if (!gridData.IgnorePeriod && gridData.FilterExpression.Find(p => (FilterFunc)p.Func == FilterFunc.Between) == null)
            {
                rf.DataType = (int)DataType.tpDate;
                rf.FieldName = "CREATE_DATE";
                rf.Func = (int)FilterFunc.Between;
                rf.FilterValue = new PeriodControl(320).ClassToDictionary();
                gridData.FilterExpression.Add(rf);
            }

            if (string.IsNullOrEmpty(gridData.SortExpression)) gridData.SortExpression = "CREATE_DATE DESC";

            var seller_action_filter = gridData.FilterExpression.Find(x => x.FieldName == "SELLER_ACTION_TXT") != null ? gridData.FilterExpression.Find(x => x.FieldName == "SELLER_ACTION_TXT").FilterValue.ToString() : "";

            if (gridData.View == "v_invoice_seller" || gridData.View == "v_invoice_return") // გამყიდველი
            {
                gridData.Criteria = string.Format("UNID_SELLER IN ({0}) ", AuthUser.UnID);

                if (AuthUser.PermitForAnyRecord)
                {
                    gridData.Criteria = string.Format("UNID_SELLER IN ({0}) ", AuthUser.UnID);
                }
                else
                {
                    gridData.Criteria = string.Format("UNID_SELLER IN ({0}) AND SUBUSER_ID_SELLER = :Param1", AuthUser.UnID);
                    gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", AuthUser.SubUserID, CustomOracleDbType.Int32));
                }

                if (gridData.NotifView == "v_unsent_seller") // გადასაგზავნი
                {
                    gridData.Criteria += " AND (SELLER_ACTION IS NULL OR SELLER_ACTION = 0) AND (BUYER_ACTION IS NULL OR BUYER_ACTION = 0)";
                }
                else if (gridData.NotifView == "v_sent_seller") // გადაგზავნილი-საჭიროებს მყიდველის დადასტურებას
                {
                    gridData.Criteria += " AND SELLER_ACTION = 1 AND BUYER_ACTION = 0";
                }
                else if (gridData.NotifView == "v_rejecteded_seller") // უარყოფილი მყიდველის მიერ
                {
                    gridData.Criteria += " AND BUYER_ACTION = 2";
                }
                else if (gridData.NotifView == "v_to_decl_seller") // დეკლარაციაზე მისაბმელი
                {
                    gridData.Criteria += @" AND SELLER_ACTION IN (1,3) AND BUYER_ACTION IN (0, 1, 2) AND 
                                                (CORRECT_REASON_ID <> 5 OR CORRECT_REASON_ID IS NULL)                                               
                                                AND SEQNUM_SELLER IS NULL 
                                                AND (INV_CATEGORY IN (1,4) AND INV_TYPE IN (2,3,6,11) 
                                                OR (INV_CATEGORY = 3 AND TRANS_START_DATE IS NOT NULL OR TRANS_TYPE IN (6,7)))
                                                AND CMN.FUNC_GET_IS_VAT(UNID_SELLER,OPERATION_DATE) > 0 ";

                }
                else if (gridData.NotifView == "v_single_seller") // ჩათვლაზე წარდგენილი დაუწყვილებელი
                {
                    gridData.Criteria += @" AND (DOCMOSNOM_SELLER IS NULL AND DOCMOSNOM_BUYER IS NOT NULL AND SEQNUM_BUYER IS NOT NULL) 
                                            AND SELLER_ACTION = 1 AND BUYER_ACTION = 1";
                }

                // უკან დაბრუნების ინვოისი
                if (gridData.View == "v_invoice_return")
                {
                    gridData.Criteria += @" AND INV_TYPE = 5";
                }

                if (string.IsNullOrEmpty(seller_action_filter) || seller_action_filter != "წაშლილი")
                    gridData.Criteria += " AND SELLER_ACTION > -1";

                gridData.GetData<dsInvoiceSeller>();
            }
            else if (gridData.View == "v_invoice_buyer")   //მყიდველი
            {
                gridData.Criteria = string.Format("UNID_BUYER IN ({0}) ", AuthUser.UnID);

                if (AuthUser.PermitForAnyRecord)
                {
                    gridData.Criteria = string.Format("(UNID_BUYER IN ({0}))", AuthUser.UnID);
                }
                else
                {
                    gridData.Criteria = string.Format("UNID_BUYER IN ({0}) AND SUBUSER_ID_BUYER = :Param1", AuthUser.UnID);
                    gridData.CriteriaParameters.Add(new DataSourceCriteriaParameters("Param1", AuthUser.CurrentUser.SubUserID, CustomOracleDbType.Int32));
                }

                if (gridData.NotifView == "v_to_confirm_buyer") //დასადასტურებელი
                {
                    gridData.Criteria += " AND SELLER_ACTION = 1 AND BUYER_ACTION = 0";
                }
                else if (gridData.NotifView == "v_to_decl_buyer") // დეკლარაციაზე მისაბმელი
                {
                    gridData.Criteria += @" AND ((SELLER_ACTION = 1 AND BUYER_ACTION = 1) 
                                            OR (SELLER_ACTION = 3 AND BUYER_ACTION = 1))
                                            AND SEQNUM_BUYER IS NULL 
                                            AND (INV_CATEGORY IN (1,4) AND INV_TYPE IN (2,3,6,11) 
                                            OR (INV_CATEGORY = 3 AND TRANS_START_DATE IS NOT NULL OR TRANS_TYPE IN (6,7)))
                                            AND OPERATION_DATE >= add_months(trunc(sysdate, 'Year'), - (12 * 3))";
                }
                else if (gridData.NotifView == "v_single_buyer") // ჩათვლაზე წარდგენილი დაუწყვილებელი
                {
                    gridData.Criteria += " AND (DOCMOSNOM_SELLER IS NULL AND DOCMOSNOM_BUYER IS NOT NULL AND SEQNUM_BUYER IS NOT NULL) " +
                                         "AND SELLER_ACTION = 1 AND BUYER_ACTION = 1";
                }
                else // ყველა ინვოისი
                {
                    gridData.Criteria += " AND SELLER_ACTION > 0"; //AND BUYER_ACTION <> 2";
                }

                if (string.IsNullOrEmpty(seller_action_filter) || seller_action_filter != "წაშლილი")
                    gridData.Criteria += " AND SELLER_ACTION > -1";

                gridData.GetData<dsInvoiceBuyer>();
            }
            else if (gridData.View == "v_invoice_transporter")
            {
                gridData.Criteria = string.Format("TRANS_COMPANY_UNID IN ({0}) AND SELLER_ACTION > 0", AuthUser.UnID);
                gridData.GetData<dsInvoiceTransporter>();
            }
            else if (gridData.View == "v_invoice_advance") // ავანსის ფაქტურები 
            {
                if (string.IsNullOrEmpty(gridData.FilterExpression.Find(x => x.FieldName == "TIN_BUYER").FilterValue.ToString()))
                    ThrowError(-2);
                gridData.Criteria = string.Format("UNID_SELLER IN ({0}) ", AuthUser.UnID);
                //gridData.Criteria += @"AND SELLER_ACTION > 0 or SELLER_ACTION is NULL";
                gridData.GetData<dsInvoiceAdvance>();
            }
            else if (gridData.View == "v_invoice_template")
            {
                gridData.Criteria = string.Format("UNID_SELLER IN ({0}) ", AuthUser.UnID);

                gridData.Criteria += " AND SELLER_ACTION = -2";
                gridData.GetData<dsInvoiceSeller>();
            }
            return Success(gridData.Data);
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]

        public HttpResponseMessage ListInvoices(Dictionary<string, string> FilterExpression)
        {
            List<FilterTypeParameters> FilterParameterTypes = new List<FilterTypeParameters>();
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "ID", DataType = (int)DataType.tpNumber });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "INV_NUMBER", DataType = (int)DataType.tpNumber });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "PARENT_INV_NUMBER", DataType = (int)DataType.tpNumber });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "INV_CATEGORY", DataType = (int)DataType.tpString, FilterFunc = (int)FilterFunc.InList });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "INV_TYPE", DataType = (int)DataType.tpString, FilterFunc = (int)FilterFunc.InList });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "ACTION", DataType = (int)DataType.tpNumber, FilterFunc = (int)FilterFunc.InList });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_NAME", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_DRIVER", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_COMPANY", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_CAR_MODEL", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_CAR_NO", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_TRAILER_NO", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_COST", DataType = (int)DataType.tpNumber });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_COST_PAYER", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "SELLER_ACTION_TXT", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "BUYER_ACTION_TXT", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "SELLER", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "BUYER", DataType = (int)DataType.tpString });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TIN_BUYER", DataType = (int)DataType.tpString, FilterFunc = (int)FilterFunc.Equal });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "CREATE_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "CHANGE_DATE", DataType = (int)DataType.tpDate });

            if (FilterExpression["TYPE"] != null && FilterExpression["TYPE"].ToString() == "5" || FilterExpression["TYPE"].ToString() == "50")
                FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "OPERATION_DATE", DataType = (int)DataType.tpDate, FilterFunc = (int)FilterFunc.Less });
            else
                FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "OPERATION_DATE", DataType = (int)DataType.tpDate });

            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "ACTIVATE_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "CONFIRM_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "REFUSE_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "REQUEST_CANCEL_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "AGREE_CANCEL_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "CORRECT_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TRANS_START_DATE", DataType = (int)DataType.tpDate });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "MAXIMUM_ROWS", DataType = (int)DataType.tpNumber });
            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "TYPE", DataType = (int)DataType.tpNumber });

            FilterParameterTypes.Add(new FilterTypeParameters() { FieldName = "DECL_OPERATION_PERIOD", DataType = (int)DataType.tpString });

            var grid = new GridData();
            grid.StartRowIndex = 0;
            grid.IgnorePeriod = true;
            grid.MaximumRows = 10;
            grid.NotifView = null;
            grid.SortExpression = null;
            grid.DataExportType = ExportType.GridBind;
            grid.View = "v_invoice_seller";

            if (FilterExpression == null)
                return GrdInvoices(grid);

            foreach (var x in FilterExpression)
            {
                var thisFilterParam = FilterParameterTypes.Find(t => t.FieldName == x.Key);
                var filterExpression = new FilterExpression();
                filterExpression.FieldName = x.Key;
                filterExpression.DataType = thisFilterParam.DataType;
                if (thisFilterParam.DataType == 2 && (thisFilterParam.FilterFunc == -1 || thisFilterParam.FilterFunc == 5))
                {
                    try
                    {
                        filterExpression.Func = (int)FilterFunc.Between;
                        var startDate = x.Value.Split(':')[0];
                        var endDate = x.Value.Split(':')[1];
                        var dict = new Dictionary<string, object>();
                        dict.Add("StartDate", startDate);
                        dict.Add("EndDate", endDate);
                        filterExpression.FilterValue = dict;
                        grid.FilterExpression.Add(filterExpression);
                    }
                    catch (Exception ex)
                    { }
                }
                //else if (thisFilterParam.DataType == 2 && thisFilterParam.FilterFunc > -1)
                //{
                //    filterExpression.Func = (int)thisFilterParam.FilterFunc;

                //}
                else if (filterExpression.FieldName == "MAXIMUM_ROWS")
                {
                    var max = x.Value.ToNumber<int>();
                    if (!string.IsNullOrEmpty(x.Value) && max == 0)
                        max = 99999999;
                    else if (string.IsNullOrEmpty(x.Value))
                        max = 10;
                    grid.MaximumRows = max;
                }
                else if (filterExpression.FieldName == "TYPE")
                {
                    switch (x.Value.ToNumber<int>())
                    {
                        case 1:
                            grid.View = "v_invoice_seller";
                            break;
                        case 10:
                            grid.View = "v_invoice_seller";
                            break;
                        case 11:
                            grid.View = "v_invoice_seller";
                            grid.NotifView = "v_to_decl_seller";
                            break;
                        case 2:
                            grid.View = "v_invoice_buyer";
                            break;
                        case 20:
                            grid.View = "v_invoice_buyer";
                            break;
                        case 21:
                            grid.View = "v_invoice_buyer";
                            grid.NotifView = "v_to_decl_buyer";
                            break;
                        case 3:
                            grid.View = "v_invoice_transporter";
                            break;
                        case 30:
                            grid.View = "v_invoice_transporter";
                            break;
                        case 4:
                            grid.View = "v_invoice_template";
                            break;
                        case 40:
                            grid.View = "v_invoice_template";
                            break;
                        case 5:
                            grid.View = "v_invoice_advance";
                            grid.IgnorePeriod = false;
                            break;
                        case 50:
                            grid.View = "v_invoice_advance";
                            grid.IgnorePeriod = false;
                            break;
                    }
                }
                else if (filterExpression.FieldName == "DECL_OPERATION_PERIOD")
                {
                    grid.FilterExpression.Add(new FilterExpression
                    {
                        FieldName = "OPERATION_DATE",
                        FilterValue = DateTime.Parse(x.Value.ToString()),
                        DataType = (int)DataType.tpDate,
                        Func = (int)FilterFunc.LessEqualMonth
                    });
                }
                else
                {
                    //if (new List<string>() { "INV_TYPE", "INV_CATEGORY" }.Contains(filterExpression.FieldName))
                    //else
                    filterExpression.Func = thisFilterParam.FilterFunc == -1 ? (int)FilterFunc.Contains : thisFilterParam.FilterFunc;
                    filterExpression.FilterValue = x.Value;
                    grid.FilterExpression.Add(filterExpression);
                }
            }
            return GrdInvoices(grid);
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]

        public HttpResponseMessage ListGoods(JObject FilterExpression)
        {
            Dictionary<string, int> FilterParameterTypes = new Dictionary<string, int>();
            FilterParameterTypes.Add("TIN_SELLER", (int)DataType.tpString);
            FilterParameterTypes.Add("TIN_BUYER", (int)DataType.tpString);
            FilterParameterTypes.Add("ACTIVATE_DATE", (int)DataType.tpDate);
            FilterParameterTypes.Add("START_ROW_INDEX", (int)DataType.tpNumber);
            FilterParameterTypes.Add("COUNT", (int)DataType.tpNumber);
            string TIN_SELLER = FilterExpression["TIN_SELLER"] != null ? FilterExpression["TIN_SELLER"].ToString() : "";
            string TIN_BUYER = FilterExpression["TIN_BUYER"] != null ? FilterExpression["TIN_BUYER"].ToString() : "";
            string ACTIVATE_DATE = FilterExpression["ACTIVATE_DATE"] != null ? FilterExpression["ACTIVATE_DATE"].ToString() : "";
            int START_ROW_INDEX = FilterExpression["START_ROW_INDEX"] != null ? FilterExpression["START_ROW_INDEX"].ToString().ToNumber<int>() : 0;
            int COUNT = FilterExpression["COUNT"] != null ?  FilterExpression["COUNT"].ToString().ToNumber<int>() : 1000;

            var x = DataProviderManager<PKG_INVOICE>.Provider.GetInvoicesWithGoods(AuthUser.ID, AuthUser.SubUserID, TIN_SELLER, TIN_BUYER, ACTIVATE_DATE, START_ROW_INDEX, COUNT);
            return Success(JObject.Parse(x));
        }


        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage ListBarcodes(Dictionary<string, string> FilterExpression)
        {
            Dictionary<string, int> FilterParameterTypes = new Dictionary<string, int>();
            FilterParameterTypes.Add("BARCODE", (int)DataType.tpString);
            FilterParameterTypes.Add("GOODS_NAME", (int)DataType.tpString);
            FilterParameterTypes.Add("UNIT_TXT", (int)DataType.tpString);
            FilterParameterTypes.Add("VAT_TYPE_TXT", (int)DataType.tpString);
            FilterParameterTypes.Add("UNIT_PRICE", (int)DataType.tpNumber);
            FilterParameterTypes.Add("MAXIMUM_ROWS", (int)DataType.tpNumber);

            var grid = new GridData();
            grid.StartRowIndex = 0;
            grid.IgnorePeriod = true;
            grid.MaximumRows = 10;
            grid.NotifView = null;
            grid.SortExpression = null;
            grid.DataExportType = ExportType.GridBind;

            if (FilterExpression == null)
                return GrdBarCodes(grid);

            foreach (var x in FilterExpression)
            {
                var filterExpression = new FilterExpression();
                filterExpression.FieldName = x.Key;
                filterExpression.DataType = FilterParameterTypes[x.Key];
                if (filterExpression.FieldName == "MAXIMUM_ROWS")
                {
                    var max = x.Value.ToNumber<int>();
                    if (!string.IsNullOrEmpty(x.Value) && max == 0)
                        max = 99999999;
                    else if (string.IsNullOrEmpty(x.Value))
                        max = 10;
                    grid.MaximumRows = max;
                }
                else
                {
                    filterExpression.Func = (int)FilterFunc.Contains;
                    filterExpression.FilterValue = x.Value;
                    grid.FilterExpression.Add(filterExpression);
                }
            }
            return GrdBarCodes(grid);
        }

        [HttpPost]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage ListExcise(Dictionary<string, string> FilterExpression)
        {
            Dictionary<string, int> FilterParameterTypes = new Dictionary<string, int>();
            FilterParameterTypes.Add("PRODUCT_NAME", (int)DataType.tpString);
            FilterParameterTypes.Add("EFFECT_DATE", (int)DataType.tpDate);
            FilterParameterTypes.Add("END_DATE", (int)DataType.tpDate);
            FilterParameterTypes.Add("MAXIMUM_ROWS", (int)DataType.tpNumber);

            var grid = new GridData();
            grid.StartRowIndex = 0;
            grid.IgnorePeriod = true;
            grid.MaximumRows = 10;
            grid.NotifView = null;
            grid.SortExpression = null;
            grid.DataExportType = ExportType.GridBind;

            if (FilterExpression == null)
                return GrdExcise(grid);

            foreach (var x in FilterExpression)
            {
                var filterExpression = new FilterExpression();
                filterExpression.FieldName = x.Key;
                filterExpression.DataType = FilterParameterTypes[x.Key];
                if (FilterParameterTypes[x.Key] == 2)
                {
                    try
                    {
                        filterExpression.Func = (int)FilterFunc.Between;
                        var startDate = x.Value.Split(':')[0];
                        var endDate = x.Value.Split(':')[1];
                        var dict = new Dictionary<string, object>();
                        dict.Add("StartDate", startDate);
                        dict.Add("EndDate", endDate);
                        filterExpression.FilterValue = dict;
                        grid.FilterExpression.Add(filterExpression);
                    }
                    catch
                    { }
                }
                else if (filterExpression.FieldName == "MAXIMUM_ROWS")
                {
                    var max = x.Value.ToNumber<int>();
                    if (!string.IsNullOrEmpty(x.Value) && max == 0)
                        max = 99999999;
                    else if (string.IsNullOrEmpty(x.Value))
                        max = 10;
                    grid.MaximumRows = max;
                }
                else
                {
                    filterExpression.Func = (int)FilterFunc.Contains;
                    filterExpression.FilterValue = x.Value;
                    grid.FilterExpression.Add(filterExpression);
                }
            }
            return GrdExcise(grid);
        }

        private class FilterTypeParameters
        {
            public string FieldName { get; set; }
            public int DataType { get; set; }
            public int FilterFunc { get; set; } = -1;

        }


        [HttpPost]
        //[Route("GetOrgInfoByTin")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetNameByTin([FromBody] Dictionary<string, object> json)
        {
            var Tin = string.Empty;

            if (json != null && json.ContainsKey("Tin")) Tin = json["Tin"].ToString();

            Tin = string.IsNullOrEmpty(Tin) ? AuthUser.Tin : Tin;

            var name = new OrgController().GetOrgInfo(Tin, DateTime.Now, false, true, true);

            if (!name.TryGetValue("Name", out object fullNameObj))
            {
                return Success(name);
            }

            var fullName = fullNameObj.ToString();

            //string initials = string.Empty;
            //string[] unwantedLetters = { ",", ".", "\"" };
            //string[] headers = { "შპს", "სს", "სსიპ" };

            //if (AuthUser.SamFormaType != SamformaType.Company)
            //    initials = (fullName.Split(' ').Length > 1) ? fullName.Split(' ')[0].Substring(0, 1) +
            //                                                  fullName.Split(' ')[1].Substring(0, 1) : fullName.Split(' ')[0].Substring(0, 1);
            //else
            //{
            //    initials = fullName;

            //    foreach (string x in unwantedLetters)
            //    {
            //        initials = initials.Replace(x, "");
            //    }

            //    string header = (initials.IndexOf(' ') == -1) ? string.Empty : initials.Substring(0, initials.IndexOf(' '));

            //    foreach (string h in headers)
            //    {
            //        if (header == h) initials = initials.Substring(header.Length, 1);
            //    }
            //    initials = initials.Substring(0, 1);
            //}

            //name.Add("Initials", initials);

            return Success(name);
        }
        [HttpPost]
        //[Route("NonResidentInfo")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage NonResidentInfo([FromBody] Dictionary<string, object> json)
        {
            var data = new Dictionary<string, object>();
            var fullName = string.Empty;

            var Tin = string.Empty;

            if (json != null && json.ContainsKey("Tin")) Tin = json["Tin"].ToString();

            DataProviderManager<PKG_ORG_INFO>.Provider.get_non_resident(Tin, out fullName);
            data.Add("Name", fullName);


            return Success(data);
        }



        #endregion

        #region WOOD

        //        [HttpPost]
        //        //[Route("GrdWoodDocs")]
        //        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        //        public IHttpActionResult GrdWoodDocs([FromBody] GridData gridData)
        //        {
        //            //gridData.Criteria = string.Format("UN_ID IN ({0}) ", AuthUser.UnID);
        //            //gridData.GetData<dsInvoiceBarCodes>();

        //            var woodDocs = getWoodDocsAll();
        //            //var list = new List<RevenueDocument>();
        //            //list = woodDocs.OfType<RevenueDocument>().ToList();
        //            gridData.MaximumRows = woodDocs.Count;
        //            gridData.GetData<RevenueDocument>(woodDocs);
        //            return Ok(gridData.Data);
        //        }


        //        [HttpPost]
        //        //[Route("getWoodDocsAll")]
        //        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        //        public List<RevenueDocument> getWoodDocsAll()
        //        {
        //            RevenueServiceClient client = new RevenueServiceClient();
        //            var x = client.GetRevenueDocumentsByIdentNO("39ec55781c9dc083bce25638f4d34e9a", "206322102").ToList(); // აბრუნებს დოკუმენტის ნომრებს (ლიცენზიებს)
        ///*            var sp = client.GetBalance("39ec55781c9dc083bce25638f4d34e9a", "0314272499", "206322102"); // აბრუნებს საქონელს ცნობის დოკუმენტის ნომრის მიხედვით
        //            var sx = client.GetProductsBySpending("39ec55781c9dc083bce25638f4d34e9a", "0314272499", "206322102"); // აბრუნებს საქონელს წარმოშობის დოკუმენტის ნომრის მიხედვით
        //            var ssp = client.GetClassifiers("39ec55781c9dc083bce25638f4d34e9a"); // აბრუნებს ჯიშებს
        //            */
        //            return x;
        //        }

        #endregion
    }
}
