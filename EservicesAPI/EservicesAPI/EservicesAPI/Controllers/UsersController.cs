using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.OraDatabase.Models;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Controllers
{
    //[EnableCors("*", "*", "*")]
    //[RoutePrefix("Users")]
    public class UsersController : MainController
    {
        #region Authenticate

        [HttpPost]
        public HttpResponseMessage Authenticate(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var userName = request["USERNAME"].ToString().Trim();
            var password = request["PASSWORD"].ToString();
            var authType = request.ContainsKey("AUTH_TYPE") && request["AUTH_TYPE"] != null ? request["AUTH_TYPE"].ToString().ToNumber<int>() : (int)AuthType.Tin;
            var deviceCode = request.ContainsKey("DEVICE_CODE") && request["DEVICE_CODE"] != null ? request["DEVICE_CODE"].ToString() : string.Empty;

            var authUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.authenticate(userName, password, authType, deviceCode, CommonFunctions.GetRemoteAddress, string.Empty, string.Empty);

            if (authUser.IsPinCodeAuth)
            {
                response.Add("PIN_TOKEN", authUser.PinToken);
                response.Add("MASKED_MOBILE", authUser.MaskedMobile);
            }
            else
            {
                DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(
                    authUser.SubUserID == 0 ? authUser.ID : authUser.SubUserID,
                    authUser.UserType,
                    authUser);

                               
                TokenManager.CreateToken(authUser);

                response.Add("ACCESS_TOKEN", authUser.AccessToken);
                response.Add("EXPIRES_IN", authUser.ExpiresIn);
                response.Add("MASKED_MOBILE", string.Empty);
            }

            //response.Add("MASKED_MOBILE", currUser.MaskedMobile);
            return Success(response);
        }

        [HttpPost]
        public HttpResponseMessage AuthenticatePrivate(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var userName = request["USERNAME"].ToString().Trim();
            var password = request["PASSWORD"].ToString();
            var clientIP = request["CLIENT_IP"].ToString();
            var secretKey = request["SECRET_KEY"].ToString();
            var authType = request.ContainsKey("AUTH_TYPE") && request["AUTH_TYPE"] != null ? request["AUTH_TYPE"].ToString().ToNumber<int>() : (int)AuthType.Tin;
            var deviceCode = request.ContainsKey("DEVICE_CODE") && request["DEVICE_CODE"] != null ? request["DEVICE_CODE"].ToString() : string.Empty;
            var latitude = request.ContainsKey("LATITUDE") && request["LATITUDE"] != null ? request["LATITUDE"].ToString() : string.Empty;
            var longitude = request.ContainsKey("LONGITUDE") && request["LONGITUDE"] != null ? request["LONGITUDE"].ToString() : string.Empty;

            if (secretKey != "ae0434da-1366-44f5-8798-524c5be11766") ThrowError(-1);

            var authUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.authenticate(userName, password, authType, deviceCode, clientIP, latitude, longitude);

            if (authUser.IsPinCodeAuth)
            {
                response.Add("PIN_TOKEN", authUser.PinToken);
                response.Add("MASKED_MOBILE", authUser.MaskedMobile);
            }
            else
            {
                DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(
                    authUser.SubUserID == 0 ? authUser.ID : authUser.SubUserID,
                    authUser.UserType,
                    authUser);

                
                TokenManager.CreateToken(authUser);

                response.Add("ACCESS_TOKEN", authUser.AccessToken);
                response.Add("EXPIRES_IN", authUser.ExpiresIn);
                response.Add("MASKED_MOBILE", string.Empty);
            }

            response.Add("SECRET_WORD", authUser.SecretWord);
            return Success(response);
        }

        [HttpPost]
        public HttpResponseMessage AuthenticatePin(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var pinToken = request["PIN_TOKEN"].ToString();
            var mobileCode = request["PIN"].ToString();

            var deviceInfo = new DeviceInfo
            {
                device_code = request.ContainsKey("DEVICE_CODE") && request["DEVICE_CODE"] != null ? request["DEVICE_CODE"].ToString() : string.Empty,
                address = request.ContainsKey("ADDRESS") && request["ADDRESS"] != null ? request["ADDRESS"].ToString() : string.Empty,
                browser = request.ContainsKey("BROWSER") && request["BROWSER"] != null ? request["BROWSER"].ToString() : string.Empty,
                oper_system = request.ContainsKey("OPER_SYSTEM") && request["OPER_SYSTEM"] != null ? request["OPER_SYSTEM"].ToString() : string.Empty
            };

            var authUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.authenticate_pin(pinToken, mobileCode);
            if (authUser != null)
            {
                if (!string.IsNullOrEmpty(deviceInfo.device_code))
                {
                    DataProviderManager<PKG_AUTHENTICATION>.Provider.save_device_code(authUser.ID, authUser.SubUserID, deviceInfo);
                }

                DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(
                    authUser.SubUserID == 0 ? authUser.ID : authUser.SubUserID,
                    authUser.UserType,
                    authUser);

                authUser.PinToken = pinToken;

                TokenManager.CreateToken(authUser);
                response.Add("ACCESS_TOKEN", authUser.AccessToken);
                response.Add("EXPIRES_IN", authUser.ExpiresIn);
            }
            return Success(response);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage AuthenticateTrustedUser(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var userId = request["USER_ID"].ToString().ToNumber<long>();
            var userType = request["USER_TYPE"].ToString().ToNumber<int>();

            if (AuthUser.TrustingUsers.Count(s => s.UserID == userId && s.UserType == userType) == 0)
            {
                ThrowError(-109);
            }

            if (AuthUser.MobileVerification == 0 && userType == 5)
            {
                ThrowError(-110);
            }

            var authUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.authenticate_trusted_user(userId, userType, CommonFunctions.GetRemoteAddress);
            DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(authUser.SubUserID == 0 ? authUser.ID : authUser.SubUserID, authUser.UserType, authUser);

            TokenManager.CreateToken(authUser);
            response.Add("ACCESS_TOKEN", authUser.AccessToken);
            response.Add("EXPIRES_IN", authUser.ExpiresIn);
            return Success(response);
        }
        
        [HttpPost]
        public HttpResponseMessage RequestPhoneChange(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var pinToken = request["PIN_TOKEN"].ToString();
            var mobile = request["MOBILE_NUMBER"].ToString();
            var word = request["SECRET_WORD"].ToString();
            var token = DataProviderManager<PKG_AUTHENTICATION>.Provider.request_phone_change(pinToken, mobile, word);
            response.Add("CHANGE_TOKEN", token);
            return Success(response);
        }

        [HttpPost]
        public HttpResponseMessage ConfirmPhoneChange(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var pinToken = request["CHANGE_TOKEN"].ToString();
            var mobileCode = request["PIN"].ToString();
            
            DataProviderManager<PKG_AUTHENTICATION>.Provider.confirm_phone_change(pinToken, mobileCode);
            return Success(response);
        }

        [HttpPost]
        public HttpResponseMessage ResendCode(Dictionary<string, object> request)
        {
            var response = new Dictionary<string, object>();
            var pinToken = request["PIN_TOKEN"].ToString();
            var type = request["TYPE"].ToString().ToNumber<int>();
            
            DataProviderManager<PKG_AUTHENTICATION>.Provider.resend_code(pinToken, type);
            return Success(response);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetUserData()
        {
            var data = new Dictionary<string, object>();

            //var currUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(AuthUser.SubUserID == 0 ? AuthUser.ID : AuthUser.SubUserID,
            //    AuthUser.UserType);
            //currUser.IsAdmin = AuthUser.IsAdmin;

            //data.Add("CURR_USER", currUser);

            data.Add("CURR_USER", AuthUser);

            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage SignOut()
        {
            TokenManager.DeleteToken(AccessToken);
            return Success();
        }
        
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage RefreshToken()
        {
            return Success();
        }
        
        #endregion

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage EmptyResponse()
        {
            ThrowError(-201);
            return Success();
        }

        [HttpPost]
        [Authenticate]
        //[Route("GetUserDetails")]
        public HttpResponseMessage GetUserDetails()
        {
            var data = new Dictionary<string, object>();
            data.Add("UnID", AuthUser.UnID);
            data.Add("IsVatPayer", AuthUser.IsVatPayer);
            data.Add("SubUserID", AuthUser.SubUserID);

            return Success(data);
        }

        #region User Menu 

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///  იუზერის მოდულებს ნოტიფიკაციებთან ერთად
        /// </returns>
        [HttpPost]
        [Authenticate(Refresh = false)]
        //[Route("GetUserModulesWithNotifications")]
        public HttpResponseMessage GetUserModules()
        {
            try
            {
                /*groupby da Select ი იმიტომაა რომ ხანდახან module ები დუბლირებულად მოდის(ერთი და იგივე რამოდენიმეჯერ)*/
                var modulesList = AuthUser.Modules.GroupBy(x => x.ID).Select(g => g.First()).ToList();
                /**
                 * get module Notifications
                 */
                var moduleIds = modulesList.Select(i => i.ID).ToList();

                var externalModuleIDs = modulesList.Where(i => i.ExternalService == 1).Select(i => i.ID).ToList();

                /*notification type 2 == OnMain notifications*/ // .Where(i=>i.Pinned)

                var moduleIdsAsString = moduleIds.FirstOrDefault().ToString();

                for (int i = 1; i < moduleIds.Count; i++)
                {
                    moduleIdsAsString = $"{moduleIdsAsString},{moduleIds[i].ToString()}";
                }


                var externalModuleIdsAsStr = string.Empty;
                for (int i = 1; i < externalModuleIDs.Count; i++)
                {
                    externalModuleIdsAsStr = $"{externalModuleIdsAsStr},{externalModuleIDs[i].ToString()}";
                }

                var notifications =
                    DataProviderManager<PKG_MENU>.Provider.GetNotificationsJson(moduleIdsAsString, 3, AuthUser.UnID,
                        AuthUser.SubUserID);

                // var externalGroupIcons = DataProviderManager<PKG_MENU>.Provider.GetModuleGroupIcons($",{externalModuleIdsAsStr},");

                //   var externalGroupIconsProp = new JProperty("ExternalGroupIcons", JArray.FromObject(externalGroupIcons.Select(i=> new { Image = i.Item2, ModuleID = i.Item1 })));


                /* mandatory ნოტიფიკაციაში js ფუნქციაში (" da ") ან აურია JSON ი და ქვევით escape ს ვუკეთებ " - ას */

                notifications = notifications.Replace("(\"", "(\\\"");
                notifications = notifications.Replace("\")", "\\\")");
                // old =  (" new = (\"
                var notifProp = new JProperty("ModuleNotifications", JArray.Parse(notifications));
                var modulesProp = new JProperty("UserModules", JArray.Parse(JsonConvert.SerializeObject(modulesList)));


                var moduleJson = new JObject(modulesProp);
                var notifJson = new JObject(notifProp);
                // var externalGroupIconsJson = new JObject(externalGroupIconsProp);
                var mergeSettings = new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union,
                    MergeNullValueHandling = MergeNullValueHandling.Merge
                };

                moduleJson.Merge(notifJson, mergeSettings);
                //  moduleJson.Merge(externalGroupIconsJson, mergeSettings);
                return Success(moduleJson);
            }
            catch(Exception ex)
            {
                CommonFunctions.CatchExceptions(ex, "", false, AuthUser.Username);
            }
            return Success();
        }

        [HttpPost]
        [Authenticate]
        //[Route("GetModuleGroupIcon")]
        public HttpResponseMessage GetModuleGroupIcon([FromBody]int moduleID)
        {

            var imageBytes = DataProviderManager<PKG_MENU>.Provider.GetModuleGroupIcon(moduleID);
            var encodedImgBytes = Convert.ToBase64String(imageBytes);
            //var response = new Dictionary<string, object>();
            // response.Add("img", encodedImgBytes);
            return Success(encodedImgBytes);
        }
        
        [HttpPost]
        [Authenticate]
        //[Route("ToggleModulePin")]
        public HttpResponseMessage ToggleModulePin([FromBody]int moduleID)
        {
            var wasSuccess = DataProviderManager<PKG_MENU>.Provider.ToggleModulePin(moduleID, AuthUser.ID);
            return wasSuccess ? Success("S") : Success("E");

        }

        #endregion

        #region UserParameters

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetPersonalInfo()
        {
            var data = new Dictionary<string, object>();
            var personalInfo = AuthUser.SubUserID != 0 ?
                DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetSubPersonalInfo(AuthUser.ID, AuthUser.SubUserID)
                : DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetPersonalInfo(AuthUser.ID);
            data.Add("IS_SUBUSER", AuthUser.SubUserID != 0 ? 1 : 0);
            data.Add("SAMFORMA_TYPE", AuthUser.SamFormaType == SamformaType.Individual ? 1 : 0);
            data.Add("PERSONAL_INFO", personalInfo);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage UpdateUserEmail([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var email = json.GetValue("EMAIL").ToString();
            if (string.IsNullOrEmpty(email) && !CommonFunctions.ValidateEmail(email))
            {
                ThrowError(-10);
            }
            string pinCode = new Random().Next(1000, 9999).ToString();
            DataProviderManager<PKG_SMS>.Provider.SendEmail(email, "ელ-ფოსტის დადასტურება", pinCode + " კოდს ვადა გასდის 5 წუთში.");
            TokenManager.SetConfirmCode($"ConfirmCode:{AccessToken}", pinCode, 5);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ConfirmEmailCode([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var email = json.GetValue("EMAIL").ToString();
            var code = json.GetValue("CODE").ToString();
            if (string.IsNullOrEmpty(email) && !CommonFunctions.ValidateEmail(email))
            {
                ThrowError(-10);
            }
            var redisKey = $"ConfirmCode:{AccessToken}";
            var codeFromRedis = TokenManager.GetConfirmCode(redisKey);
            if (string.IsNullOrEmpty(codeFromRedis))
            {
                ThrowError(-108);
            }
            if (codeFromRedis == code)
            {
                DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserEmail(AuthUser.ID, AuthUser.SubUserID, email);
                var status = DataProviderManager<PKG_USERS>.Provider.NotificationAboutChange(AuthUser.Phone, null, AuthUser.Email, "ელ-ფოსტა");
                AuthUser.Email = email;
                TokenManager.UpdateAuthUser(AccessToken, AuthUser);
                TokenManager.DeleteConfirmCode(redisKey);
            } else {
                ThrowError(-102);
            }
            return Success(data);
        }
        
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage SendSms()
        {
            var data = new Dictionary<string, object>();
            string pinCode = new Random().Next(1000, 9999).ToString();
            DataProviderManager<PKG_SMS>.Provider.SendSms(AuthUser.Phone, pinCode + " კოდს ვადა გასდის 5 წუთში.", ControllerName);
            TokenManager.SetConfirmCode($"ConfirmCode:{AccessToken}", pinCode, 5);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage TwoStepVerification([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var check = json.GetValue("CHECK").ToString();
            var code = json.GetValue("CODE").ToString().ToNumber<int>();
            var redisKey = $"ConfirmCode:{AccessToken}";
            var codeFromRedis = TokenManager.GetConfirmCode(redisKey);
            if (check.ToNumber<int>() == 1 && (codeFromRedis.ToNumber<int>() != code || codeFromRedis.ToNumber<int>() == 0))
            {
                ThrowError(-102);
            }

            DataProviderManager<PKG_USERS>.Provider.TwoStepVerification(AuthUser.ID, AuthUser.SubUserID, check, out int count, out int turnOnOff);
            if (turnOnOff == 0)
            {
                ThrowError(-2);
            }
            TokenManager.DeleteConfirmCode(redisKey);
            AuthUser.MobileVerification = (turnOnOff == 2 || turnOnOff == 0) ? 0 : 1;
            TokenManager.UpdateAuthUser(AccessToken, AuthUser);
            data.Add("DATA_COUNT", count);
            data.Add("STATUS", turnOnOff);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ResendSms([FromBody]JObject json)
        {
            var data = new Dictionary<string, object>();
            var phone = json.GetValue("PHONE").ToString();
            if (string.IsNullOrEmpty(phone) || !CommonFunctions.ValidateMobileNumber(phone) || phone.Length != 9)
            {
                ThrowError(-8);
            }
            string pinCode = new Random().Next(1000, 9999).ToString();
            DataProviderManager<PKG_SMS>.Provider.SendSms(phone, pinCode + " კოდს ვადა გასდის 5 წუთში.", ControllerName);
            TokenManager.SetConfirmCode($"ConfirmCode:{AccessToken}", pinCode, 5);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ConfirmCode([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var code = json.GetValue("CODE").ToString();
            var number = json.GetValue("PHONE").ToString();
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(number))
            {
                ThrowError(-9);
            }
          
            var redisKey = $"ConfirmCode:{AccessToken}";
            var codeFromRedis = TokenManager.GetConfirmCode(redisKey);
            if (string.IsNullOrEmpty(codeFromRedis))
            {
                ThrowError(-108);
            }

            if (codeFromRedis == code)
            {
                var oldNumber = AuthUser.Phone;
                DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserPhone(AuthUser.ID, AuthUser.SubUserID, number);
                AuthUser.Phone = number;
                TokenManager.UpdateAuthUser(AccessToken, AuthUser);
                data.Add("PHONE", number);
                var status = DataProviderManager<PKG_USERS>.Provider.NotificationAboutChange(oldNumber, number, AuthUser.Email, "ტელეფონის ნომერი");
                data.Add("STATUS", status);
                TokenManager.DeleteConfirmCode(redisKey);
                return Success(data);
            } else {
                ThrowError(-102);
            }
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage UpdateUserAddress([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            if (AuthUser.SamFormaType == SamformaType.Individual)
            {
                DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateUserAddress(AuthUser.UnID, json.GetValue("REGION").ToString(), 
                    json.GetValue("DISTRICT").ToString(), json.GetValue("STREETS").ToString(), json.GetValue("PLACENUMBER").ToString());
            }
            else
            {
                ThrowError(-105);
            }
            return Success();
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage UpdateSecretWord([FromBody] JObject json)
        {
            var status = DataProviderManager<PKG_USER_PARAMETERS>.Provider.UpdateSecretWord(AuthUser.ID, AuthUser.SubUserID, json.GetValue("SECRET_WORD").ToString());
            AuthUser.SecretWord = status == 1 ? 1 : 0;
            TokenManager.UpdateAuthUser(AccessToken, AuthUser);
            return Success(status);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetDistricts([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            data.Add("DISTRICTS", DataProviderManager<PKG_REGION>.Provider.GetAddressList(json.GetValue("REGION").ToString()));
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetStreets([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(json.GetValue("VALUE").ToString()))
                ThrowError(-9);

            data.Add("STREETS", DataProviderManager<PKG_ORG_INFO>.Provider.GetStreets(json.GetValue("VALUE").ToString(), json.GetValue("TEXT").ToString()));
            data.Add("OFFICER", DataProviderManager<PKG_ORG_INFO>.Provider.GetOficerInfo(json.GetValue("REGION_ID").ToString(), json.GetValue("VALUE").ToString()));
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ChangePassword([FromBody]JObject json)
        {
            var data = new Dictionary<string, object>();
            var oldPass = json.GetValue("OLD_PASSWORD").ToString();
            var newPass = json.GetValue("NEW_PASSWORD").ToString();
            var repPass = json.GetValue("REP_PASSWORD").ToString();

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(repPass))
            {
                ThrowError(-9);
            }
            if (newPass.Length < 6)
            {
                ThrowError(-11);
            }
            if (newPass != repPass)
            {
                ThrowError(-112);
            }
            if (newPass == oldPass)
            {
                ThrowError(-111);
            }

            if (AuthUser.SubUserID != 0)
            {
                DataProviderManager<PKG_USERS>.Provider.ChangeSubPassword(AuthUser.Username, oldPass, newPass);
            } else {
                DataProviderManager<PKG_USERS>.Provider.ChangePassword(AuthUser.Username, oldPass, newPass);
            }
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetContactsList()
        {
            var data = new Dictionary<string, object>();
            if (AuthUser.SubUserID != 0) return Success(data);
            var contactList = DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetContacts(AuthUser.UnID);
            data.Add("CONTACT_LIST", contactList);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetModuleContacts([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            data.Add("CONTACT_INFO", DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetModuleContact(AuthUser.UnID, json.GetValue("SETTING_ID").ToString(), json.GetValue("MODULE_ID").ToString()));
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage SaveModuleContact([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var contact = new UserContact();
            contact.AppId = json.GetValue("APP_ID").ToString().ToNumber<decimal>();
            contact.Mobile = json.GetValue("MOBILE").ToString();
            contact.Phone = json.GetValue("PHONE").ToString();
            contact.Email = json.GetValue("EMAIL").ToString();
            contact.Visbility = bool.Parse(json.GetValue("VISIBILITY").ToString());

            DataProviderManager<PKG_USER_PARAMETERS>.Provider.SaveModuleContact(AuthUser.UnID, contact);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage ToggleModuleContact([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            DataProviderManager<PKG_USER_PARAMETERS>.Provider.ToggleModuleContact(json.GetValue("SETTING_ID").ToString(), AuthUser.UnID);
            data.Add("SETTING_ID", json.GetValue("SETTING_ID").ToString());
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetSavedDevices()
        {
            var data = new Dictionary<string, object>();
            var savedDevices = DataProviderManager<PKG_USERS>.Provider.GetSavedDevices(AuthUser.ID, AuthUser.SubUserID);
            data.Add("SavedDevices", savedDevices);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage DeleteSavedDevice([FromBody] JObject json)
        {
            var data = new Dictionary<string, object>();
            var count = DataProviderManager<PKG_USERS>.Provider.DeleteSaveDevice(AuthUser.ID, AuthUser.SubUserID, json.GetValue("DELETE_DEVICE").ToString());
            data.Add("DATA_COUNT", count);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetAddressDetails()
        {
            var result = new Dictionary<string, object>();
            result.Add("ADDRESS_DETAILS", DataProviderManager<PKG_USER_PARAMETERS>.Provider.GetAddressDetails(AuthUser.UnID));
            result.Add("REGION", DataProviderManager<PKG_REGION>.Provider.GetAddressList());
            return Success(result);
        }

        #endregion

    }


}