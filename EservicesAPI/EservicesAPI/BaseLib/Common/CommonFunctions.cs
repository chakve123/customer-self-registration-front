using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseLib.Classes;
using BaseLib.OraDataBase;
using BaseLib.OraDataBase.StoredProcedures;
using BaseLib.User;
using Microsoft.Security.Application;
using SevenZip;
using Image = System.Drawing.Image;

namespace BaseLib.Common
{
    public class CommonFunctions
    {

        #region CreateDataTable

        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        /// <summary>
        /// Convert an IList&lt;T&gt; into a DataTable
        /// </summary>
        /// <example><code>
        /// IList&lt;Person&gt; people = new List&lt;Person&gt;
        ///    {
        ///        new Person { Id= 1, DoB = DateTime.Now, Name = "Bob", Sex = Person.Sexes.Male }
        ///    };
        /// DataTable dt = DataUtil.ConvertToDataTable(people);
        /// </code></example>
        public static DataTable ConvertToDataTable<T>(IList<T> list) where T : class
        {
            var table = CreateDataTable<T>();
            var objType = typeof(T);
            var properties = TypeDescriptor.GetProperties(objType);
            //Debug.WriteLine("foreach (" + objType.Name + " item in list) {");
            foreach (var item in list)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    if (!CanUseType(property.PropertyType)) continue; //shallow only
                    //Debug.WriteLine("row[\"" + property.Name + "\"] = item." + property.Name + "; //.HasValue ? (object)item." + property.Name + ": DBNull.Value;");
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value; //can't use null
                }
                Debug.WriteLine("//===");
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// Convert an IList&lt;T&gt; into a DataTable schema
        /// </summary>
        public static DataTable CreateDataTable<T>() where T : class
        {
            var objType = typeof(T);
            var table = new DataTable(objType.Name);
            var properties = TypeDescriptor.GetProperties(objType);
            foreach (PropertyDescriptor property in properties)
            {
                var propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue; //shallow only

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType); //probably Int32
                //if you have nested application classes, they just get added. Check if this is valid?
                Debug.WriteLine("table.Columns.Add(\"" + property.Name + "\", typeof(" + propertyType.Name + "));");
                table.Columns.Add(property.Name, propertyType);
            }
            return table;
        }

        #endregion

        public static double SessionSize(Page p)
        {
            try
            {
                p.Trace.Warn("Session size Start", "Session Trace Info");

                double totalSessionBytes = 0;
                var b = new BinaryFormatter();
                foreach (string key in HttpContext.Current.Session)
                {
                    var obj = HttpContext.Current.Session[key];
                    var m = new MemoryStream();
                    if (obj != null)
                        b.Serialize(m, obj);
                    totalSessionBytes += m.Length;

                    p.Trace.Write(String.Format("{0}: {1} kb", key, Math.Round((double)m.Length / 1024, 3)));
                }

                p.Trace.Warn("Session size End", String.Format("Total Size of Session Data: {0} kb", Math.Round(totalSessionBytes / 1024, 3)));

                return totalSessionBytes;
            }
            catch (Exception ex)
            {
                CatchExceptions(ex, "Session Serialization Error", false);

            }

            return 0;
        }

        public static bool ValidateDateTime(TextBox txtDate)
        {
            DateTime dt;
            if (ParseDateTime(txtDate.Text, out dt))
            {
                txtDate.Style.Add("border", "1px solid #C5C5C5;");
                txtDate.ToolTip = string.Empty;

                return true;
            }
            else
            {
                txtDate.Style.Add("border", "1px solid #EA3838");
                txtDate.ToolTip = "არასწორი ფორმატი";
                return false;
            }
        }

        public static bool ParseDateTime(string date, out DateTime parseDate, string dateFormat = "")
        {
            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            // DateTime parseDate;
            return DateTime.TryParseExact(date, dateFormat, null, DateTimeStyles.None, out parseDate);
        }

        public static bool ValidatePage(Page p)
        {
            try
            {
                foreach (BaseValidator valControl in p.Validators)
                {
                    var assControl = (WebControl)FindControlRecursive(p, valControl.ControlToValidate);
                    if (!string.IsNullOrEmpty(valControl.ToolTip))
                    {
                        assControl.CssClass = assControl.CssClass.Replace("notValid", "");
                        assControl.ToolTip = string.Empty;
                    }
                }

                foreach (BaseValidator valControl in p.Validators)
                {
                    var assControl =
                    (WebControl)FindControlRecursive(p, valControl.ControlToValidate);

                    if (!valControl.IsValid)
                    {
                        assControl.CssClass = assControl.CssClass + " notValid";
                        assControl.ToolTip = valControl.ErrorMessage;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(valControl.ToolTip))
                        {
                            assControl.CssClass = assControl.CssClass.Replace("notValid", "");
                            assControl.ToolTip = string.Empty;
                        }
                    }
                }
            }
            catch
            { }

            return !p.IsValid;

        }

        /// <summary>
        /// clone object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static string getFilesContentType(string s)
        {
            switch (GetExtension(s))
            {
                case "pdf": return "application/pdf";
                case "xls": return "application/vnd.ms-excel";
                case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "doc": return "application/msword";
                case "docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "gif": return "image/gif";
                case "png": return "image/x-png";
                case "rar": return "application/x-rar-compressed";
                case "jpeg": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "csv": return "application/csv";
                default: return "application/octet-stream";
            }
        }

        public static string GetExtension(string s)
        {
            var type = s.Split('.');
            return type[type.Length - 1].ToLower();
        }

        public static string getFilesExtension(string s)
        {
            switch (s)
            {
                case "application/pdf": return "pdf";
                case "application/ms-excel": return "xls";
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": return "xlsx";
                case "application/msword": return "doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "docx";
                case "image/gif": return "gif";
                case "image/x-png": return "png";
                case "application/x-rar-compressed": return "rar";
                case "image/jpeg": return "jpg";
                default: return "rar";
            }
        }

        public static byte[] StreamToByteArray(Stream inputStream)
        {
            inputStream.Position = 0;
            var reader = new BinaryReader(inputStream);

            var data = reader.ReadBytes((int)inputStream.Length);

            reader.Close();
            inputStream.Close();

            return data;
        }

        /// <summary>
        /// Checks the first two bytes in a GZIP file, which must be 31 and 139.
        /// </summary>
        public static bool IsGZip(byte[] arr)
        {
            return arr.Length >= 2 &&
             arr[0] == 31 &&
             arr[1] == 139;
        }

        /// <summary>
        /// Compress Files to 7zip format (lzma:6m method)
        /// </summary>
        /// <param name="s">In Stream</param>
        /// <param name="fileName">File Name</param>
        /// <returns>Compresed Byte Array</returns>
        public static byte[] CompressFile(Stream s, string fileName)
        {
            using (var ms = new MemoryStream())
            {
                SevenZipCompressor.SetLibraryPath(Path.Combine(HttpContext.Current.Server.MapPath("~"), "Lib", "7z.dll"));

                var sz = new SevenZipCompressor();
                sz.DefaultItemName = fileName;
                sz.CompressionMethod = CompressionMethod.Lzma;
                sz.ArchiveFormat = OutArchiveFormat.SevenZip;
                sz.CompressStream(s, ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exists
        /// </summary>
        /// <param name="container">The container to search for the control passed. Remember
        /// all controls (Panel, GroupBox, Form, etc are all containsers for controls
        /// </param>
        /// <param name="name">Name of the control to look for</param>
        /// <returns></returns>
        /// 
        public static Control FindControlRecursive(Control container, string name)
        {
            if (container.ID == name)
                return container;

            foreach (Control ctrl in container.Controls)
            {
                var foundCtrl = FindControlRecursive(ctrl, name);
                if (foundCtrl != null)
                    return foundCtrl;
            }
            return null;
        }

        /// <summary>
        /// generate new random password
        /// </summary>
        /// <returns>new random password</returns>
        public static string GenerateNewPassword()
        {
            //var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            var random = new Random();
            var codeString = "";
            var codeLength = random.Next(8, 10);
            for (var i = 0; i < codeLength; i++)
            {
                var index = random.Next(0, chars.Length - 1);
                codeString += chars[index];
            }

            return codeString;
        }

        public static string GenerateNumberPassword()
        {
            var random = new Random();
            var code = random.Next(100000, 1000000).ToString();
            return code;
        }

        public static string GetRemoteAddress
        {
            get
            {
                var publicIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;
                var clientIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(clientIP) && publicIP != clientIP) return publicIP + " clientIP:" + clientIP;
                return publicIP;
            }
        }

        public static string GetServerAddress
        {
            get
            {
                var strHostName = Dns.GetHostName();

                var ipEntry = Dns.GetHostByName(strHostName);
                var addr = ipEntry.AddressList;
                return addr[0].ToString();
            }
        }

        public static string CatchExceptions(Exception ex, string customText = "", bool messageBoxShow = true, string userName = "")
        {
            if (ex is ThreadAbortException) return "";
            var innerException = ""; string browserInfo;
            try
            {
                var e = ex;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    innerException += string.Format("StackTrace: {0} Message: {1} **************", e.StackTrace, e.Message);
                }
            }
            catch (Exception e)
            {
                innerException = "can't parse innerExceptions. error=" + e.Message;
            }

            try
            {
                browserInfo = HttpContext.Current.Request.UserAgent;
            }
            catch (Exception e)
            {
                browserInfo = "error get browser info. error=" + e.Message;
            }

            if (ex.Message.Contains("ORA-12170: TNS:Connect timeout occurred"))
            {
                if (messageBoxShow)
                {
                    var errorList = (List<string>)HttpContext.Current.Session["ServiceDown"];
                    errorList[1] = "ServiceDown";
                    HttpContext.Current.Session["ServiceDown_UserName"] = errorList;
                    throw ex;
                }
                return "ServiceDown";
            }

            DataProviderManager<PKG_ERROR_LOGS>.Provider.save_logs(GetRemoteAddress,
                GetServerAddress,
                string.IsNullOrEmpty(userName) ? "Unknown-User" : userName,
                ex.Message,
                ex.StackTrace,
                innerException,
                customText,
                browserInfo);

            return string.Empty;

            // if (messageBoxShow)
            // {
            //MessageBox.Show(ConfigurationManager.AppSettings["error_text"]);
            //  }

            //if (HttpContext.Current.Session == null)
            //     DataProviderManager<PKG_ERROR_LOGS>.Provider.save_logs(GetRemoteAddress, GetServerAddress, "UploaderError", ex.Message, ex.StackTrace, innerException, customText, browserInfo);
            // else if (AuthUser.Authenticated)
            //     DataProviderManager<PKG_ERROR_LOGS>.Provider.save_logs(GetRemoteAddress, GetServerAddress, AuthUser.CurrentUser.Username, ex.Message, ex.StackTrace, innerException, customText, browserInfo);
            // else

        }

        public static string Clean(string s)
        {
            var result = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                var b = (byte)c;
                if (b > 15)
                    result.Append(c);
            }
            return result.ToString();
        }

        public static void SavePeriod(string start, string end)
        {
            var reportPeriod = string.Format("პერიოდი:  {0}-დან\r\n{1}-მდე", start, end);
            HttpContext.Current.Session["ReportPeriod"] = reportPeriod;
        }

        public static T GetAttribute<T>(Type attr) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(attr, typeof(T));
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                var length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                var sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        public static void SetRegionalSetting(string format = "dd-MMM-yyyy")
        {
            Language language;
            if (AuthUser.Authenticated) language = AuthUser.CurrLanguage;
            else language = Language.Georgia;

            CultureInfo cultureInfo;

            switch (language)
            {
                case Language.Georgia:
                    cultureInfo = new CultureInfo("ka-GE");
                    break;
                case Language.English:
                    cultureInfo = new CultureInfo("en-US");
                    break;
                default:
                    cultureInfo = new CultureInfo("ka-GE");
                    break;
            }

            cultureInfo.DateTimeFormat.ShortDatePattern = format;
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberGroupSeparator = ",";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        public static void SequantialFlushing(Stream file)
        {
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());

            var fileBytes = new byte[4096];

            while (true)
            {
                var count = (file).Read(fileBytes, 0, fileBytes.Length);

                HttpContext.Current.Response.OutputStream.Write(fileBytes, 0, count);

                if (count == 0 || !HttpContext.Current.Response.IsClientConnected) break;
                try
                {
                    HttpContext.Current.Response.Flush();
                }
                catch
                {
                    //MessageBox.Show("ინტერნეტ კავშირის შეცდომა - სცადეთ ხელმეორედ");
                }
            }
        }

        public static Dictionary<string, object> CompareObjects<T>(T obj1, T obj2, List<string> notCheckedList = null)
        {
            var unionValues = new Dictionary<string, object>();

            foreach (var prop in typeof(T).GetProperties().Where(p => !typeof(IList).IsAssignableFrom(p.PropertyType)))
            {
                var mainValue = prop.GetValue(obj1, null) == null ? string.Empty : prop.GetValue(obj1, null).ToString();
                var oldValue = prop.GetValue(obj2, null) == null ? string.Empty : prop.GetValue(obj2, null).ToString();

                if ((notCheckedList == null || !notCheckedList.Exists(s => s == prop.Name)) && mainValue != oldValue && !mainValue.Equals(oldValue))
                {
                    unionValues.Add(prop.Name, new Dictionary<string, object>
                        {
                            {"val",prop.GetValue(obj1, null)},
                            {"corVal",prop.GetValue(obj2, null)}
                        });
                }
                else
                {
                    unionValues.Add(prop.Name, prop.GetValue(obj2, null));
                }
            }

            return unionValues;
        }

        public static Dictionary<string, object> ClassToDictionary(object obj)
        {
            if (obj is Dictionary<string, object>) return obj as Dictionary<string, object>;
            return obj.GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
        }

        public static void ObjSanitizer<T>(List<T> obj)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                // Only work with strings

                if (p.PropertyType != typeof(string)) { continue; }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead) { continue; }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null) { continue; }
                if (mset == null) { continue; }

                foreach (T item in obj)
                {
                    var val = (string)p.GetValue(item, null);

                    if (string.IsNullOrEmpty(val))
                    {
                        p.SetValue(item, Sanitizer.GetSafeHtmlFragment(val), null);
                    }
                }
            }
        }

        public static string CharToString(char[] chars)
        {
            return chars.Aggregate("", (current, t) => current + t);
        }

        public static void RegisterStartupScript(Type type, string name, string script)
        {
            var cs = ((Page)HttpContext.Current.Handler).ClientScript;

            var csText = new StringBuilder();
            csText.Append("\n<script type='text/javascript'>  \n");
            csText.Append(script);
            csText.Append("</script> \n");
            cs.RegisterStartupScript(type, name, csText.ToString());
        }

        /// <param name="updatePeriod"> Minute</param>
        public static StaticObject<T> GetStaticObject<T>(StaticObject<T> obj, Func<int, T> procedure, object[] parameters, int updatePeriod = 0) where T : class
        {
            return GetStaticObject(obj, (object)procedure, parameters, updatePeriod);
        }

        /// <param name="updatePeriod"> Minute</param>
        public static StaticObject<T> GetStaticObject<T>(StaticObject<T> obj, Func<T> procedure, int updatePeriod = 0) where T : class
        {
            return GetStaticObject(obj, procedure, null, updatePeriod);
        }

        public static StaticObject<T> GetStaticObject<T>(StaticObject<T> obj, object procedure, object[] parameters, int updatePeriod = 0) where T : class
        {
            try
            {
                if (obj == null || obj.Value == null || obj.WatchedDate == null || (updatePeriod > 0 && obj.WatchedDate < DateTime.Now.AddMinutes(-1 * updatePeriod)))
                {
                    obj = new StaticObject<T>
                    {
                        WatchedDate = DateTime.Now,
                        Value = (T)((Delegate)procedure).DynamicInvoke(parameters),

                    };
                }
            }
            catch (Exception ex)
            {
                CatchExceptions(ex);
            }

            return obj;
        }

        public static T GetSessionObject<T>(string pageId, string sessionName, bool removePageId = false) where T : new()
        {
            var obj = new T();
            var sessionObject = (HttpContext.Current.Session[sessionName] as Dictionary<string, T>);
            if (sessionObject != null && removePageId)
            {
                sessionObject.Remove(pageId);
                sessionObject.Remove("");
            }

            if (sessionObject == null)
            {
                HttpContext.Current.Session[sessionName] = new Dictionary<string, T> { { pageId, obj } };
            }
            else if (!sessionObject.TryGetValue(pageId, out obj))
            {
                obj = new T();
                sessionObject.Add(pageId, obj);
            }
            return obj;
        }

        public static T SetSessionObject<T>(string pageId, string sessionName, T sessionObjectValue)
        {
            var sessionObject = (HttpContext.Current.Session[sessionName] as Dictionary<string, T>);
            if (sessionObject != null)
            {
                if (sessionObject.ContainsKey(pageId))
                {
                    sessionObject[pageId] = sessionObjectValue;
                }
                else
                {
                    sessionObject.Add(pageId, sessionObjectValue);
                }
            }
            else
            {
                HttpContext.Current.Session[sessionName] = new Dictionary<string, T> { { pageId, sessionObjectValue } };
            }
            return sessionObjectValue;
        }

        public static bool ValidateUsername(string username)
        {
            var regexUsername = new Regex(@"^[a-zA-Z0-9\._\-]{0,22}?[a-zA-Z0-9]{0,2}$");
            return username.Length > 6 && regexUsername.IsMatch(username);
        }

        public static bool ValidatePassword(string password)
        {
            var regexPassword = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])");
            return password.Length > 5 && regexPassword.IsMatch(password);
        }

        public static bool ValidateMobileNumber(string number)
        {
            var regexpNumber = new Regex(@"^5[-. ]?([0-9]{2})[-. ]?([0-9]{2})[-. ]?([0-9]{1})[-. ]?([0-9]{1})[-. ]?([0-9]{2})$");
            return regexpNumber.IsMatch(number);
        }

        public static bool ValidateEmail(string email)
        {
            try { new MailAddress(email); }
            catch (Exception ex) { return false; }
            return true;
        }

        public static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size, bool p_Scale = false)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            int destWidth, destHeight;

            if (!p_Scale)
            {

                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                destWidth = (int)(sourceWidth * nPercent);
                destHeight = (int)(sourceHeight * nPercent);
            }
            else
            {
                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);

                destWidth = (int)(sourceWidth * nPercentW);
                destHeight = (int)(sourceHeight * nPercentH);
            }
            Bitmap b = new Bitmap(destWidth, destHeight);

            Graphics g = System.Drawing.Graphics.FromImage((Image)b);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.High;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (System.Drawing.Image)b;
        }
    }
}
