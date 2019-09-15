using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using BaseLib.Common;
using EservicesAPI.Common;
using EservicesLib.User;

namespace EservicesAPI.Controllers
{
    public class MainController : ApiController
    {
        public MainController()
        {
            SetRegionalSetting();
        }

        public static void SetRegionalSetting(string format = "dd-MMM-yyyy")
        {
            Language language;
            //if (AuthUser.Authenticated) language = AuthUser.CurrLanguage;
            //else language = Language.Georgia;

            language = Language.Georgia;

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

        protected HttpResponseMessage Success(object data = null)
        {
            return ResponseBuilder.Success(data);
        }

        protected HttpResponseMessage ResponseFromString(string json, HttpStatusCode httpStatusCode)
        {
          return  ResponseBuilder.GetResponseMessageByContentJson(json, httpStatusCode);
        }
        //protected void ThrowError(int statusID, string statusText)
        //{
        //    ResponseBuilder.ThrowError(statusID, statusText);
        //}

        //protected void ThrowError(int statusID)
        //{
        //    ResponseBuilder.ThrowError(statusID);
        //}

        protected void ThrowError(int statusID)
        {
            ResponseBuilder.ThrowError(statusID);
        }

        protected AuthUser AuthUser
        {
            get
            {
                object authUser;

                if (!Request.Properties.TryGetValue("AuthUser", out authUser)) return null;
                return (AuthUser)authUser;
            }
        }        

        public FileData _fileData;

        public FileData FileData
        {
            get
            {
                if (_fileData == null) _fileData = new FileData();
                return _fileData;
            }
            set
            {
                _fileData = value;
            }
        }

        protected string AccessToken {
            get
            {
                var accessToken = string.Empty;
                var header = HttpContext.Current.Request.Headers.GetValues("Authorization");
                if (header != null)
                {
                    accessToken = header[0].Split(' ')[1];
                }
                return accessToken;
            }
        }

        protected string ControllerName {
            get
            {
                return this.ControllerContext.RouteData.Values["controller"].ToString();
            }
        }

    }
}