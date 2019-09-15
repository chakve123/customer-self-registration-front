using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.Models.Invoice;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace EservicesAPI.Controllers
{
    [RoutePrefix("Common")]
    public class CommonController : MainController
    {
        [HttpPost]
        [Authenticate]
        [Route("LogError")]
        public HttpResponseMessage LogError([FromBody] Dictionary<string, object> req)
        {
            CommonFunctions.CatchExceptions(new Exception(req["ERROR_TEXT"].ToString()), null, false);
            return Success();
        }

        [HttpPost]
        [Authenticate]
        [Route("GetUnits")]
        public HttpResponseMessage GetUnitis()
        {
            var units = DataProviderManager<CMN_PKG_MEASURE_UNITS>.Provider.get_units_list();
            return Success(units);
        }


        [HttpGet]
        [Route("GetImage")]
        public HttpResponseMessage GetImage(int moduleID)
        {
            var res = DataProviderManager<PKG_MENU>.Provider.GetModuleGroupIcon(moduleID);

            // return File(res, MediaTypeNames.Image.Jpeg);


            var stream = new MemoryStream(res);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(stream); // this file stream will be closed by lower layers of web api for you once the response is completed.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            response.Headers.CacheControl = new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromSeconds(3999999) };

            return response;
        }

        

        [HttpPost]
        [Authenticate]
        [Route("GetTransactionResult")]
        public HttpResponseMessage GetTransactionResult([FromBody] JObject jObject)
        {
            var transactionId = jObject["TransactionId"].ToString();
            
            
            var dbRes = DataProviderManager<PKG_TRANSACTION_LOG>.Provider.GET_TRANSACTION_RESULT(AuthUser.ID, transactionId);

            //var statusJson = string.Empty;

            /*მსგავსი TransactionId ით ტრანზაქცია ვერ მოიძებნა*/
            if (!dbRes.Item1.HasValue)
            {
                ThrowError(-6);
            }
            else
            {
                /*მიმდინარე ტრანზაქციაზე ითხოვს ინფოს*/
                if (dbRes.Item1.Value == 1)
                {
                    ThrowError(-7);
                }
                /*დაფიქსირდა გაურკვეველი(სისტემური) შეცდომა*/
                if (dbRes.Item1.Value == -1)
                {
                    ThrowError(-1);
                }
            }

            /*აქამდე თუ მოვიდა, ე.ი. ტრანზაქცია არსებობს და ვაბრუნებ პასუხს, ბაზიდან მომაქვს Data იან Status იანად ინფო და ვაბრუნებ*/
            return ResponseFromString(dbRes.Item2, HttpStatusCode.OK);
        }



        /// <summary>
        /// Convert JSON to Field Type
        public static Object JsonToFieldObject(JObject Json, Object obj, Type InitClass = null)
        {
            if (InitClass != null)
                obj = Activator.CreateInstance(InitClass);

            foreach (var json in Json)
            {
                FieldInfo getField = obj.GetType().GetField(json.Key, BindingFlags.Public | BindingFlags.Instance);
                if (getField != null)
                {
                    var getFieldValue = getField.GetValue(obj); // value returns multiple Field's fields and functions like (value, isValid, isVisible and etc...)
                    try
                    {
                        bool isList = false;
                        isList = getFieldValue is IList;
                        if (isList)
                        {
                            //IList list = null;
                            //if (getFieldValue != null && getFieldValue.GetType().GetGenericArguments().Single() != null && Activator.CreateInstance(getFieldValue.GetType().GetGenericArguments().Single()) != null)
                            //    list = (IList)Activator.CreateInstance(getFieldValue.GetType().GetGenericArguments().Single());
                            foreach (JObject items in (JArray)json.Value)
                            {
                                var item = JsonToFieldObject(items, getFieldValue, getFieldValue.GetType().GetGenericArguments().Single());
                                ((IList)getFieldValue).Add(item);
                            }
                            getField.SetValue(getField, getFieldValue);
                        }
                    }
                    catch (Exception ex) { }

                    var FieldValueProperty = getFieldValue.GetType().GetProperty("value"); // try to get (value) property to check if type is really Field
                    MethodInfo myObjPropType;
                    if (FieldValueProperty == null)
                    { // type is not Field, nor FieldGroup;
                        if (getFieldValue is FieldGroup) continue; // do not parse FieldGroup Types because they don't need to
                        try
                        {
                            getField.SetValue(obj, Convert.ChangeType(json.Value.ToString(), getFieldValue.GetType().GetTypeInfo())); // try to convert to simple types such as string, int etc... if types are lists of some sort of complicated we don't convert them
                            continue;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    myObjPropType = getFieldValue.GetType().GetMethod("GetTypeOfValue");
                    Type myType = (Type)myObjPropType.Invoke(getFieldValue, null);
                    try
                    {
                        FieldValueProperty.SetValue(getFieldValue, Convert.ChangeType(json.Value.ToString(), myType), null);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (myType.Name == "DateTime")
                                FieldValueProperty.SetValue(getFieldValue, Convert.ChangeType(json.Value, myType), null);
                        }
                        catch (Exception exx) { }
                    }
                }
            }
            return obj;

        }



    }


    public class Excel
    {
        public string GetJsonFromExcel(HttpPostedFile ExcelFile)
        {
            HttpContext context = HttpContext.Current;

            string GlobalTempPath = ConfigurationManager.AppSettings["GlobalTempFolder"].ToString();
            string sPath = GlobalTempPath;
            bool fnSuccess = false;
            string error = "";

            Dictionary<string, string> dict = null;

            List<ExpandoObject> json = new List<ExpandoObject>();

            var tmpFileName = Guid.NewGuid() + ExcelFile.FileName;

            if (ExcelFile.ContentLength > 0)
            {
                ExcelFile.SaveAs(sPath + Path.GetFileName(tmpFileName));
                string sConnectionString;
                if (ExcelFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") // for New Excel files
                {
                    sConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;\";", sPath + tmpFileName);
                }
                else  // for Excel 2003 (xls)
                {
                    sConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", sPath + tmpFileName);
                }
                var objConn = new OleDbConnection(sConnectionString);
                OleDbDataReader dataReader = null;
                try
                {
                    objConn.Open();
                    var objCmdSelect = new OleDbCommand("SELECT * FROM [Sheet1$]", objConn);
                    dataReader = objCmdSelect.ExecuteReader();
                    int count = 1;
                    while (dataReader.Read())
                    {
                        var dictionary = new List<object>();
                        count++;
                        try
                        {
                            string Error = ""; ;
                            if (!string.IsNullOrEmpty(dataReader[0].ToString()))
                            {
                                dynamic cols = new ExpandoObject();
                                for (var i = 0; i < dataReader.FieldCount; i++)
                                {
                                    dict = new Dictionary<string, string>();
                                    dict.Add("col" + i, dataReader[i].ToString());

                                    ((IDictionary<String, Object>)cols)["col" + i] = dataReader[i].ToString();


                                    if (dict.TryGetValue("ErrorText", out Error))
                                        throw new UserExceptions(String.Format("შეცდომა მოხდა მე-{0} სტრიქონში. შეცდომა : {1}", count, Error));
                                }

                                json.Add(cols);
                            }

                        }
                        catch (UserExceptions e)
                        {
                            error = e.Message;
                            break;
                        }
                        catch (Exception ex)
                        {
                            error = "მე" + count + "-ე სტრიქონში წერია არასწორი მონაცემები";
                            throw new UserExceptions(error);
                            break;
                        }
                    }

                }
                catch (UserExceptions e)
                {
                    error = e.Message;
                }
                catch (Exception ex)
                {
                    error = "აირჩიეთ სწორი ფაილი";
                    throw new UserExceptions(error);
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }
                }
                if (string.IsNullOrEmpty(error))
                {
                    fnSuccess = true;
                }
                else
                {
                    //throw new UserExceptions(error);
                    return error;
                }
                try
                {
                    File.Delete(sPath + tmpFileName);
                }
                catch
                {
                }

                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
                if (fnSuccess == true)
                {
                    var jsonOut = JsonConvert.SerializeObject(json);
                    return jsonOut;
                }
                else
                {
                    throw new UserExceptions(error); ;
                }
            }
            return error;
        }

        public byte[] CreateExcel(System.Data.DataTable data, string templateFullPath, ExcelDataTable excelColumns)
        {
            HttpContext context = HttpContext.Current;

            string GlobalTempPath = ConfigurationManager.AppSettings["GlobalTempFolder"].ToString();
            var filePath = GlobalTempPath + "tmp" + Guid.NewGuid() + "_upload.xls";
            var fileStream = new FileStream(filePath, FileMode.Create);
            var b = CommonFunctions.ReadFile(templateFullPath);
            fileStream.Write(b, 0, b.Length);
            fileStream.Dispose();

            var sConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=No;\";", filePath);
            var objConn = new OleDbConnection(sConnectionString);

            try
            {
                var dt = data;

                objConn.Open();
                var i = 2;
                foreach (DataRow row in dt.Rows)
                {
                    //var cmd = new OleDbCommand(string.Format("INSERT INTO [Sheet1$A{0}:A{0}] values ('aaa')", i), objConn);
                    ////cmd.Parameters.AddWithValue("@barcode", row["barcode"]);
                    ////cmd.Parameters.AddWithValue("@goods_name", row["goods_name"]);

                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //var cmd1 = new OleDbCommand(string.Format("INSERT INTO [Sheet1$E{0}:F{0}] values (@unit_txt,@other_unit_txt)", i), objConn);
                    //cmd1.Parameters.AddWithValue("@unit_txt", row["unit_txt"]);
                    //cmd1.Parameters.AddWithValue("@other_unit_txt", row["other_unit_txt"]);
                    //cmd1.ExecuteNonQuery();
                    //cmd1.Dispose();
                    if (string.IsNullOrEmpty(excelColumns.Sheet)) throw new Exception();
                    foreach (var a in excelColumns.ColumnsWithFieldNames)
                    {
                        var cmd = new OleDbCommand(string.Format("INSERT INTO [" + excelColumns.Sheet.ToString() + "$" + a.Key.ToString() + "{0}:" + a.Key.ToString() + "{0}] values (@" + a.Value + ")", i), objConn);
                        cmd.Parameters.AddWithValue("@" + a.Value, row[a.Value]);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw new UserExceptions("ფაილის გადმოწერა ვერ მოხერხდა");
            }
            finally
            {
                objConn.Close();
                objConn.Dispose();
            }
            var FileBytes = CommonFunctions.ReadFile(filePath);
            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }

            return FileBytes;
        }


        /* public byte[] CreateExcel(System.Data.DataTable data, List<string> HeaderColumns = null)
        {
            HttpContext context = HttpContext.Current;

            string GlobalTempPath = ConfigurationManager.AppSettings["GlobalTempFolder"].ToString();
            var filePath = GlobalTempPath + "tmp" + Guid.NewGuid() + "_upload.xlsx";
            
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("Grid");
            
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Header1");
            row.CreateCell(1).SetCellValue("Header2");
            row.CreateCell(2).SetCellValue("Header3");
            rowIndex++;

            //foreach (DataRow dr in data.Rows)
            //{ 
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue("abcde");
                row.CreateCell(1).SetCellValue("fghijkl");
                row.CreateCell(2).SetCellValue("lmnopq");

                //rowIndex++;
            //}

            using (var fileData = new FileStream(filePath,FileMode.Create, FileAccess.Write))
            {
                wb.Write(fileData);
            }


            //    HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "Content-Disposition");
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filePath);
            //HttpContext.Current.Response.BinaryWrite(System.IO.File.ReadAllBytes(filePath));
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.Close();
            //HttpContext.Current.Response.End();

                return CommonFunctions.ReadFile(filePath);

            /*
            var fileStream = new FileStream(filePath, FileMode.Create);

            var templateFullPath = HttpContext.Current.Request.PhysicalApplicationPath + "Docs\\Blank.xls"; // Empty Excel file

            var b = CommonFunctions.ReadFile(templateFullPath);
            fileStream.Write(b, 0, b.Length);
            fileStream.Dispose();

            var sConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES;\";", filePath);
            var objConn = new OleDbConnection(sConnectionString);

            try
            {
                var dt = data;

                objConn.Open();
                var i = 1;
                foreach (DataRow row in dt.Rows)
                {
                    int colsCount = dt.Columns.Count;
                    string cmdStr = "Sheet1$";
                    string cmdVals = " values ("; ;

                    for (int c = 1; c <= colsCount; c++) {
                        if (c == 1)
                        cmdStr += (ExcelColumns)c + i.ToString();
                        else
                        cmdStr += ":" + (ExcelColumns)c + i.ToString();
                        if (c == 1)
                        cmdVals += "'" + row[c - 1].ToString() + "'";
                        else
                        cmdVals += ",'" + row[c - 1].ToString() + "'";
                    }

                    cmdVals += ")";

                    var cmd = new OleDbCommand(string.Format("INSERT INTO [" + cmdStr + "]" + cmdVals, i), objConn);

                    //var cmd = new OleDbCommand(string.Format("INSERT INTO [Sheet1$A{0}:A{0}] values ('aaa')", 0), objConn);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    i++;
                    break;

                    var x = ExcelColumns.A;

                }
            }
            catch (Exception ex)
            {
                throw new UserExceptions("ფაილის გადმოწერა ვერ მოხერხდა");
            }
            finally
            {
                objConn.Close();
                objConn.Dispose();
            }
            var FileBytes = CommonFunctions.ReadFile(filePath);
            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }

            return FileBytes; */
        //} 

        public class ExcelDataTable
        {
            public string Sheet { get; set; }
            public Dictionary<ExcelColumns, string> ColumnsWithFieldNames { get; set; }

        }
        public enum ExcelColumns
        {
            None,
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z
        }

    }
}