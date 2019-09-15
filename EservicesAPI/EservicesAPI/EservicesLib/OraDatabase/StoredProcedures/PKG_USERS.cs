using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BaseLib.Common;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using BaseLib.Exceptions;
using EservicesLib.User;
using Oracle.DataAccess.Client;
using EservicesLib.OraDatabase.Models;
using AuthUser = EservicesLib.User.AuthUser;
using Oracle.DataAccess.Types;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_USERS : DataProvider
    {

        //public AuthInfo authenticate(string userName, string password, int authType, string deviceCode, string clientIP)
        //{
        //    var cmd = new OracleCommand("rstask.PKG_USERS_N.authenticate");
        //    cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = userName;
        //    cmd.Parameters.Add("p_user_psw", OracleDbType.Varchar2).Value = password;
        //    cmd.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = clientIP;
        //    cmd.Parameters.Add("p_vcode", OracleDbType.Varchar2).Value = deviceCode;
        //    cmd.Parameters.Add("p_check", OracleDbType.Int32).Value = authType;
        //    cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_confirm_login", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_is_canceled", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_masked_mobile", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_sub_user_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_tin", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_is_vat_payer", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_user_type", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_email", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_username", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        //    cmd.Parameters.Add("p_password_expire_date", OracleDbType.Date).Direction = ParameterDirection.Output;
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    string error;
        //    AuthInfo authInfo = null;

        //    new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
        //    {
        //        if (cmd.Parameters["p_user_id"].Value.ToString().ToNumber<int>() > 0)
        //        {
        //            authInfo = new AuthInfo();
        //            authInfo.PinToken = cmd.Parameters["p_pin_token"].Value.ToString() == "null" ? null : cmd.Parameters["p_pin_token"].Value.ToString();
        //            authInfo.ConfirmLogin = cmd.Parameters["p_confirm_login"].Value.ToString() == "null" ? null : cmd.Parameters["p_confirm_login"].Value.ToString();
        //            authInfo.IsCanceled = cmd.Parameters["p_is_canceled"].Value.ToString().ToNumber<int>() == 1;
        //            authInfo.MaskedMobile = cmd.Parameters["p_masked_mobile"].Value.ToString();
        //            authInfo.UserID = cmd.Parameters["p_user_id"].Value.ToString().ToNumber<int>();
        //            authInfo.UnID = cmd.Parameters["p_un_id"].Value.ToString().ToNumber<int>();
        //            authInfo.Tin = cmd.Parameters["p_tin"].Value.ToString();
        //            authInfo.SubUserID = cmd.Parameters["p_sub_user_id"].Value.ToString().ToNumber<int>();
        //            authInfo.IsVatPayer = cmd.Parameters["p_is_vat_payer"].Value.ToString().ToNumber<int>() == 1;
        //            authInfo.UserType = cmd.Parameters["p_user_type"].Value.ToString().ToNumber<int>();
        //            // authInfo.Mobile = cmd.Parameters["p_mobile"].Value.ToString();
        //            // authInfo.Email = cmd.Parameters["p_email"].Value.ToString();
        //        }
        //    });
        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        //    return authInfo;
        //}
        //public AuthUser authenticate_pin(string pinToken, string mobileCode)
        //{
        //    AuthUser currUser = null;
        //    var cmd = new OracleCommand("rstask.PKG_USERS_N.authenticate_pin");
        //    cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2).Value = pinToken;
        //    cmd.Parameters.Add("p_mobile_code", OracleDbType.Varchar2).Value = mobileCode;
        //    cmd.Parameters.Add("p_user", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    string error;

        //    new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out error, delegate (OracleDataReader reader, int index)
        //    {
        //        switch (index)
        //        {
        //            case 0:
        //                currUser = new AuthUser();
        //                currUser.ID = Convert.ToInt32(reader["user_id"]);

        //                currUser.UnID = string.IsNullOrEmpty(reader["un_id"].ToString())
        //                    ? 0
        //                    : Convert.ToInt32(reader["un_id"].ToString());

        //                currUser.Tin = reader["tin"].ToString();

        //                currUser.SubUserID = int.Parse(reader["sub_user_id"].ToString());
        //                currUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
        //                currUser.Phone = reader["tel"].ToString();
        //                currUser.Email = reader["email"].ToString();
        //                break;
        //        }
        //    });

        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);

        //    return currUser;
        //}

        //public void save_device_code(int userId, int subUserId, DeviceInfo deviceInfo)
        //{
        //    var cmd = new OracleCommand("rstask.PKG_USERS.save_user_vcode");
        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
        //    cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
        //    cmd.Parameters.Add("p_vcode", OracleDbType.Varchar2).Value = deviceInfo.device_code;
        //    cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = deviceInfo.address;
        //    cmd.Parameters.Add("p_browser", OracleDbType.Varchar2).Value = deviceInfo.browser;
        //    cmd.Parameters.Add("p_oper_system", OracleDbType.Varchar2).Value = deviceInfo.oper_system;
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    string error;

        //    new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error);

        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        //}

        //public int has_permission(int userID, int subUserID, int moduleID, int permission)
        //{
        //    var cmd = new OracleCommand("invoice.PKG_USERS.has_permission");
        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
        //    cmd.Parameters.Add("p_sub_user_id", OracleDbType.Int32).Value = subUserID;
        //    cmd.Parameters.Add("p_module_id", OracleDbType.Int32).Value = moduleID;
        //    cmd.Parameters.Add("p_permission", OracleDbType.Int32).Value = (long)1 << permission;
        //    cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    string error;
        //    var result = 0;

        //    new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
        //    {
        //        result = cmd.Parameters["p_result"].Value.ToString().ToNumber<int>();
        //    });

        //    if (!string.IsNullOrEmpty(error)) throw new Exception();

        //    return result;
        //}

        //public AuthUser get_user_data(int userId, int userType, string pUserName, string pUserPsw, string pIp, string vcode, int check)
        //{
        //    AuthUser currUser = null;
        //    var cmd = new OracleCommand("rstask.PKG_USERS_N.get_user_data_n");
        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
        //    cmd.Parameters.Add("p_user_type", OracleDbType.Int32).Value = userType;
        //    cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = pUserName;
        //    cmd.Parameters.Add("p_user_psw", OracleDbType.Varchar2).Value = pUserPsw;
        //    cmd.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = pIp;
        //    cmd.Parameters.Add("p_vcode", OracleDbType.Varchar2).Value = vcode;
        //    cmd.Parameters.Add("p_check", OracleDbType.Int32).Value = check;
        //    cmd.Parameters.Add("p_user", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.Parameters.Add("p_user_perm", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.Parameters.Add("p_union_users", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.Parameters.Add("p_switch_users", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    string error;

        //    new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out error, delegate (OracleDataReader reader, int index)
        //    {
        //        switch (index)
        //        {
        //            case 0:
        //                if (reader["is_canceled"].ToString().ToNumber<int>() == 1)
        //                {
        //                    currUser = new AuthUser
        //                    {
        //                        IsCanceled = true
        //                    };
        //                    break;
        //                }

        //                currUser = new AuthUser
        //                {
        //                    ID = Convert.ToInt32(reader["user_id"]),
        //                    Email = reader["email"].ToString(),
        //                    FullName = reader["real_name"].ToString(),
        //                    Phone = reader["tel"].ToString(),
        //                    UnID = string.IsNullOrEmpty(reader["un_id"].ToString())
        //                        ? 0
        //                        : Convert.ToInt32(reader["un_id"].ToString()),
        //                    Role = Convert.ToInt32("0" + reader["role"].ToString()),
        //                    IdentCode = reader["SAID_KOD"].ToString(),
        //                    Address = reader["ADDRESS_FULL"].ToString(),
        //                    Tin = reader["tin"].ToString(),
        //                    SamFormaID = reader["samforma"].ToString().ToNumber<int>(),
        //                    Username = reader["user_name"].ToString(),
        //                    Password = pUserPsw,
        //                    UserType = string.IsNullOrEmpty(reader["user_type"].ToString())
        //                        ? 0
        //                        : int.Parse(reader["user_type"].ToString()),
        //                    ConfirmLogin = reader["confirm_login"].ToString(),
        //                    SecretWord = reader["secret_word"].ToString().ToNumber<int>(),
        //                    MobileVerification = reader["mobile_verif"].ToString().ToNumber<int>()
        //                };
        //                currUser.SubUserID = currUser.UserType == 5 ? int.Parse(reader["sub_user_id"].ToString()) : 0;
        //                currUser.PassExpire = DateTime.Parse(reader["password_expire_date"].ToString());
        //                currUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
        //                currUser.ShowOrgInfo = reader["show_org_info"].ToString().ToNumber<int>() == 1;
        //                currUser.TestUser = reader["test_user"].ToString().ToNumber<int>() == 1;
        //                currUser.IsAdmin = reader["is_admin"].ToString().ToNumber<int>() == 1;
        //                currUser.InsCode = reader["ins_kodi"].ToString();
        //                currUser.SubUserName = reader["subuser_name"].ToString();
        //                if (currUser.UnionOrgs == null)
        //                    currUser.UnionOrgs = new List<UnionOrg>
        //                    {
        //                        new UnionOrg
        //                        {
        //                            IsAvtive = true,
        //                            UnID = currUser.UnID,
        //                            OrgName = string.Format("{0}({1})", currUser.FullName, currUser.Tin)
        //                        }
        //                    };
        //                break;
        //            case 1:
        //                if (currUser == null || currUser.IsCanceled)
        //                    break;
        //                DateTime pinDate;
        //                DateTime? pinDateNull = null;

        //                if (DateTime.TryParseExact(reader["PINDATE"].ToString(), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out pinDate))
        //                    pinDateNull = pinDate;

        //                currUser.Modules.Add(new Module(reader["MODULE_ID"].ToString().ToNumber<int>(),
        //                    reader["APP_NAME"].ToString(), reader["APP_NAME_EN"].ToString(),
        //                    reader["GROUP_NAME"].ToString(), reader["APP_URL"].ToString(),
        //                    reader["PERMISSION"].ToString().ToNumber<long>(), reader["COLOR"].ToString(),
        //                    reader["PINNED"].ToString().ToNumber<int>(), reader["DESCRIPTION"].ToString(),
        //                    reader["NOTIFICATION_PROCEDURE"].ToString(),
        //                    reader["login_notif_procedure"].ToString(), reader["EXTERNAL_SERVICE"].ToString().ToNumber<int>(), pinDateNull));
        //                break;
        //            case 2:
        //                if (currUser == null || currUser.IsCanceled)
        //                    break;
        //                currUser.UnionOrgs.Add(new UnionOrg
        //                {
        //                    UnID = reader["old_un_id"].ToString().ToNumber<int>(),
        //                    OrgName = reader["org_name"].ToString()

        //                });
        //                break;
        //            case 3:
        //                if (currUser == null || currUser.IsCanceled)
        //                    break;
        //                currUser.TrustingUsers.Add(new TrustingUser(reader["full_name"].ToString(),
        //                    reader["user_id"].ToString().ToNumber<int>(),
        //                    reader["user_type"].ToString().ToNumber<int>(),
        //                    reader["user_name"].ToString(),
        //                    reader["tin"].ToString(),
        //                    reader["samforma_id"].ToString().ToNumber<int>()
        //                ));
        //                break;
        //        }
        //    });
        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        //    return currUser;
        //}

        //public int HasPermission(int userID, int subUserID, int moduleID, int permission)
        //{
        //    var cmd = new OracleCommand("invoice.PKG_USERS.has_permission");
        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
        //    cmd.Parameters.Add("p_sub_user_id", OracleDbType.Int32).Value = subUserID;
        //    cmd.Parameters.Add("p_module_id", OracleDbType.Int32).Value = moduleID;
        //    cmd.Parameters.Add("p_permission", OracleDbType.Int32).Value = permission;
        //    cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    string error;
        //    var result = 0;

        //    new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
        //    {
        //        result = cmd.Parameters["p_result"].Value.ToString().ToNumber<int>();
        //    });

        //    if (!string.IsNullOrEmpty(error)) throw new Exception();

        //    return result;
        //}

        //public List<Module> Get_User_Modules_list(int userId)
        //{
        //    var res = new List<Module>();
        //    var cmd = new OracleCommand("rstask.PKG_USERS.get_permissions_list");

        //    cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
        //    cmd.Parameters.Add("p_user_perm", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out string error,
        //        delegate (OracleDataReader reader, int index)
        //        {
        //            res.Add(new Module(reader["MODULE_ID"].ToString().ToNumber<int>(),
        //                    reader["APP_NAME"].ToString(), reader["APP_NAME_EN"].ToString(),
        //                    reader["GROUP_NAME"].ToString(), reader["APP_URL"].ToString(),
        //                    reader["PERMISSION"].ToString().ToNumber<long>(), reader["COLOR"].ToString(),
        //                    reader["PINNED"].ToString().ToNumber<int>(), reader["DESCRIPTION"].ToString(),
        //                    reader["NOTIFICATION_PROCEDURE"].ToString(),
        //                    reader["login_notif_procedure"].ToString(),
        //                    reader["EXTERNAL_SERVICE"].ToString().ToNumber<int>(),
        //                    null /*PIN DATE ს არ ვაბრუნებ*/
        //                )
        //            );
        //        });
        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        //    return res;
        //}

        public int SendSms(string mobile)
        {
            var localCode = 0;
            var cmd = new OracleCommand("rstask.PKG_USERS.send_sms");
            cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = mobile;
            cmd.Parameters.Add("p_code", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                localCode = cmd.Parameters["p_code"].Value.ToString().ToNumber<int>();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return localCode;
        }

        public void TwoStepVerification(int userId, int subUserId, string check, out int count, out int status)
        {
            var localCount = 0;
            var localstatus = 0;
            var cmd = new OracleCommand("rstask.PKG_USERS.two_step_verification");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_check", OracleDbType.Int32).Value = check.ToNumber<int>();
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_count", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                localstatus = cmd.Parameters["p_status"].Value.ToString().ToNumber<int>();
                localCount = cmd.Parameters["p_count"].Value.ToString().ToNumber<int>();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            count = localCount;
            status = localstatus;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var cmd = new OracleCommand("rstask.PKG_USERS.change_password_by_name");
            cmd.Parameters.Add("p_user_uname", OracleDbType.Varchar2).Value = username;
            cmd.Parameters.Add("p_old_password", OracleDbType.Varchar2).Value = oldPassword;
            cmd.Parameters.Add("p_new_password", OracleDbType.Varchar2).Value = newPassword;
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            var status = 0;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                status = cmd.Parameters["p_status"].Value.ToString().ToNumber<int>();
            });

            switch (status)
            {
                case -1:
                    throw new UserExceptions("პაროლის სიგრძე არასწორია");
                case -2:
                    throw new UserExceptions("ძველი პაროლი არასწორია");
                case -3:
                    throw new UserExceptions("პაროლის ცვლილება ვერ განხორციელდა");
            }

            if (string.IsNullOrEmpty(error))
                return true;
            throw new Exception(error);
        }

        public bool ChangeSubPassword(string username, string oldPassword, string newPassword)
        {
            var cmd = new OracleCommand("rstask.PKG_SUBUSERS.change_sub_password_by_name");
            cmd.Parameters.Add("p_user_uname", OracleDbType.Varchar2).Value = username;
            cmd.Parameters.Add("p_old_password", OracleDbType.Varchar2).Value = oldPassword;
            cmd.Parameters.Add("p_new_password", OracleDbType.Varchar2).Value = newPassword;
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            var status = 0;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                status = cmd.Parameters["p_status"].Value.ToString().ToNumber<int>();
            });

            switch (status)
            {
                case -1:
                    throw new UserExceptions("პაროლის სიგრძე არასწორია");
                case -2:
                    throw new UserExceptions("ძველი პაროლი არასწორია");
                case -3:
                    throw new Exception("პაროლის ცვლილება ვერ განხორციელდა");
            }

            if (string.IsNullOrEmpty(error))
                return true;
            throw new Exception(error);
        }

        public string SendCodeByWord(int userId, int subUserId, string word, string mobile)
        {
            string localCode = null;
            var cmd = new OracleCommand("rstask.PKG_USERS.send_code_by_word");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_word", OracleDbType.Varchar2).Value = word;
            cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = mobile;
            cmd.Parameters.Add("p_code", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                localCode = cmd.Parameters["p_code"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return localCode;
        }

        public string SendCode(int userId, int subUserId, string mobile)
        {
            string localCode = null;
            var cmd = new OracleCommand("rstask.PKG_USERS.send_code");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = mobile;
            cmd.Parameters.Add("p_code", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                localCode = cmd.Parameters["p_code"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return localCode;
        }

        public List<SavedDevices> GetSavedDevices(int userId, int subUserId)
        {
            var localValue = new List<SavedDevices>();
            var cmd = new OracleCommand("rstask.PKG_USERS.get_saved_devices");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                var saved = new SavedDevices();
                saved.UserId = reader["USER_ID"].ToString().ToNumber<decimal>();
                saved.SubUserId = reader["SUB_USER_ID"].ToString().ToNumber<decimal>();
                saved.InsertDate = Convert.ToDateTime(reader["INSERT_DATE"].ToString());
                saved.Vcode = reader["VCODE"].ToString();
                saved.Address = reader["ADDRESS"].ToString();
                saved.Browser = reader["BROWSER"].ToString();
                saved.OperSystem = reader["OPERATING_SYSTEM"].ToString();
                localValue.Add(saved);
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return localValue;
        }
        public int DeleteSaveDevice(int userId, int subUserId, string vcode)
        {
            var count = 0;
            var cmd = new OracleCommand("rstask.PKG_USERS.delete_saved_device");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_vcode", OracleDbType.Varchar2).Value = vcode;
            cmd.Parameters.Add("p_count", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                count = cmd.Parameters["p_count"].Value.ToString().ToNumber<int>();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return count;
        }

        public int NotificationAboutChange(string oldNumber, string newNumber, string email, string change)
        {
            var localCode = 0;
            var cmd = new OracleCommand("rstask.PKG_USERS.notification_about_change");
            cmd.Parameters.Add("p_old_number", OracleDbType.Varchar2).Value = oldNumber;
            cmd.Parameters.Add("p_new_number", OracleDbType.Varchar2).Value = newNumber;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_change", OracleDbType.Varchar2).Value = change;
            cmd.Parameters.Add("p_status", OracleDbType.Int16).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                localCode = cmd.Parameters["p_status"].Value.ToString().ToNumber<int>();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return localCode;
        }

        public string get_samforma_name_by_tin(string pTin)
        {
            var samforma = "";
            var cmd = new OracleCommand("rstask.PKG_USERS.get_samforma_name_by_tin");
            cmd.Parameters.Add("p_tin", OracleDbType.Varchar2).Value = pTin;
            cmd.Parameters.Add("p_out_samforma", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error,delegate 
            { samforma = cmd.Parameters["p_out_samforma"].Value.ToString(); });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return samforma;
        }
        //public byte[] GetModuleGroupIcon(int moduleId)
        //{
        //    byte[] reportBytes;
        //    var cmd = new OracleCommand("rstask.PKG_MENU.get_group_icon");
        //    cmd.Parameters.Add("p_moduleID", OracleDbType.Int32).Value = moduleId;
        //    cmd.Parameters.Add("rep_out", OracleDbType.Blob, ParameterDirection.Output);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    new OracleDb<PKG_MENU>().ExecuteNonQuery(cmd, true, out string error);
        //    if (((Oracle.DataAccess.Types.OracleBlob)cmd.Parameters["rep_out"].Value).IsNull)
        //    {
        //        reportBytes = null;
        //    }
        //    else
        //    {
        //        reportBytes = (byte[])(((Oracle.DataAccess.Types.OracleBlob)cmd.Parameters["rep_out"].Value).Value);
        //    }
        //    if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        //    return reportBytes;
        //}

        public byte[] get_image_blob(string id)
        {
            byte[] reportBytes;
            var cmd = new OracleCommand("rstask.PKG_USERS.get_file_blob");
            cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("rep_out", OracleDbType.Blob, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            //new OracleDb<PKG_QUESTIONNAIRES>().ExecuteNonQuery(cmd, out error, delegate
            //{
            //    if (!((OracleBlob)cmd.Parameters["rep_out"].Value).IsNull)
            //    {
            //        reportBytes = (byte[])(((OracleBlob)cmd.Parameters["rep_out"].Value).Value);
            //    }
            //});

            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, true, out  error);
            try
            {
                reportBytes = (byte[])(((Oracle.DataAccess.Types.OracleBlob)cmd.Parameters["rep_out"].Value).Value);
            }
            catch(Exception ex) {
                reportBytes = null;
            }
  
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return reportBytes;
        }

    }
}