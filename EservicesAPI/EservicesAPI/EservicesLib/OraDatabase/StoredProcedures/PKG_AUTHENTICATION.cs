using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using EservicesLib.User;
using Oracle.DataAccess.Client;
using AuthUser = EservicesLib.User.AuthUser;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_AUTHENTICATION : DataProvider
    {
        public AuthUser authenticate(string userName, string password, int authType, string deviceCode, string clientIP, string latitude, string longitude)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.authenticate");
            cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = userName;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = password;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = clientIP;
            cmd.Parameters.Add("p_device_code", OracleDbType.Varchar2).Value = deviceCode;
            cmd.Parameters.Add("p_auth_type", OracleDbType.Int32).Value = authType;
            cmd.Parameters.Add("p_latitude", OracleDbType.Varchar2).Value = latitude;
            cmd.Parameters.Add("p_longitude", OracleDbType.Varchar2).Value = longitude;
            cmd.Parameters.Add("p_auth_info", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            AuthUser authUser = null;
            new OracleDb<PKG_USERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                authUser = new AuthUser();
                authUser.ID = reader["user_id"].ToString().ToNumber<int>();
                authUser.SubUserID = reader["sub_user_id"].ToString().ToNumber<int>();
                authUser.UnID = reader["un_id"].ToString().ToNumber<int>();
                authUser.Tin = reader["tin"].ToString();
                authUser.PinToken = reader["pin_token"].ToString();
                authUser.ConfirmLogin = reader["confirm_login"].ToString();
                authUser.IsCanceled = reader["is_canceled"].ToString().ToNumber<int>() == 1;
                authUser.MaskedMobile = reader["masked_mobile"].ToString();
                authUser.Phone = reader["mobile"].ToString();
                authUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
                authUser.UserType = reader["user_type"].ToString().ToNumber<int>();
                authUser.IsAdmin = reader["is_admin"].ToString().ToNumber<int>() == 1;
                authUser.Email = reader["email"].ToString();
                authUser.Username = reader["username"].ToString();
                authUser.SecretWord = reader["secret_word"].ToString().ToNumber<int>();
                authUser.IsConfirmed = reader["confirmed"].ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return authUser;
        }

        public AuthUser authenticate_pin(string pinToken, string mobileCode)
        {
            AuthUser currUser = null;
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.authenticate_pin");
            cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2).Value = pinToken;
            cmd.Parameters.Add("p_mobile_code", OracleDbType.Varchar2).Value = mobileCode;
            cmd.Parameters.Add("p_user", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out error, delegate (OracleDataReader reader, int index)
            {
                switch (index)
                {
                    case 0:
                        currUser = new AuthUser();
                        currUser.ID = Convert.ToInt32(reader["user_id"]);

                        currUser.UnID = string.IsNullOrEmpty(reader["un_id"].ToString())
                            ? 0
                            : Convert.ToInt32(reader["un_id"].ToString());

                        currUser.Tin = reader["tin"].ToString();

                        currUser.SubUserID = int.Parse(reader["sub_user_id"].ToString());
                        currUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
                        currUser.Phone = reader["tel"].ToString();
                        currUser.Email = reader["email"].ToString();
                        currUser.UserType = reader["user_type"].ToString().ToNumber<int>();
                        currUser.Username = reader["user_name"].ToString();
                        break;
                }
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return currUser;
        }

        public AuthUser authenticate_trusted_user(long userId, int userType, string clientIP)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.authenticate_trusted_user");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int64).Value = userId;
            cmd.Parameters.Add("p_user_type", OracleDbType.Int32).Value = userType;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = clientIP;
            cmd.Parameters.Add("p_auth_info", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            AuthUser authUser = null;
            new OracleDb<PKG_USERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                authUser = new AuthUser();
                authUser.ID = reader["user_id"].ToString().ToNumber<int>();
                authUser.SubUserID = reader["sub_user_id"].ToString().ToNumber<int>();
                authUser.UnID = reader["un_id"].ToString().ToNumber<int>();
                authUser.Tin = reader["tin"].ToString();
                authUser.PinToken = reader["pin_token"].ToString();
                authUser.ConfirmLogin = reader["confirm_login"].ToString();
                authUser.IsCanceled = reader["is_canceled"].ToString().ToNumber<int>() == 1;
                authUser.MaskedMobile = reader["masked_mobile"].ToString();
                authUser.Phone = reader["mobile"].ToString();
                authUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
                authUser.UserType = reader["user_type"].ToString().ToNumber<int>();
                authUser.IsAdmin = reader["is_admin"].ToString().ToNumber<int>() == 1;
                authUser.Email = reader["email"].ToString();
                authUser.Username = reader["username"].ToString();
                authUser.SecretWord = reader["secret_word"].ToString().ToNumber<int>();
                authUser.IsConfirmed = reader["confirmed"].ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return authUser;
        }

        public string request_phone_change(string pinToken, string mobile, string secretWord)
        {
            var token = "";
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.request_phone_change");
            cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2).Value = pinToken;
            cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = mobile;
            cmd.Parameters.Add("p_secret_word", OracleDbType.Varchar2).Value = secretWord;
            cmd.Parameters.Add("p_token", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_AUTHENTICATION>().ExecuteNonQuery(cmd, out error, delegate
            {
                token = cmd.Parameters["p_token"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return token;
        }

        public void confirm_phone_change(string pinToken, string mobileCode)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.confirm_phone_change");
            cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2).Value = pinToken;
            cmd.Parameters.Add("p_mobile_code", OracleDbType.Varchar2).Value = mobileCode;
            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_AUTHENTICATION>().ExecuteNonQuery(cmd, out string error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public void resend_code(string pinToken, int type)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.resend_code");
            cmd.Parameters.Add("p_pin_token", OracleDbType.Varchar2).Value = pinToken;
            cmd.Parameters.Add("p_type", OracleDbType.Int32).Value = type;
            cmd.CommandType = CommandType.StoredProcedure;
            
            new OracleDb<PKG_AUTHENTICATION>().ExecuteNonQuery(cmd, out string error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public void save_device_code(int userId, int subUserId, DeviceInfo deviceInfo)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.save_device_code");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_vcode", OracleDbType.Varchar2).Value = deviceInfo.device_code;
            cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = deviceInfo.address;
            cmd.Parameters.Add("p_browser", OracleDbType.Varchar2).Value = deviceInfo.browser;
            cmd.Parameters.Add("p_oper_system", OracleDbType.Varchar2).Value = deviceInfo.oper_system;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error);

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public int has_permission(int userID, int subUserID, int moduleID, int permission)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.has_permission");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_sub_user_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_module_id", OracleDbType.Int32).Value = moduleID;
            cmd.Parameters.Add("p_permission", OracleDbType.Int32).Value = (long)1 << permission;
            cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            var result = 0;

            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = cmd.Parameters["p_result"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception();

            return result;
        }

        public void get_user_data(int userId, int userType, AuthUser authUser)
        {
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.get_user_data");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_user_type", OracleDbType.Int32).Value = userType;
            cmd.Parameters.Add("p_user", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add("p_user_perm", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add("p_union_users", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add("p_switch_users", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;
            string error;

            new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out error, delegate (OracleDataReader reader, int index)
            {
                switch (index)
                {
                    case 0:
                        if (reader["is_canceled"].ToString().ToNumber<int>() == 1)
                        {
                            authUser.IsCanceled = true;                            
                            break;
                        }

                        authUser.ID = Convert.ToInt32(reader["user_id"]);
                        authUser.Email = reader["email"].ToString();
                        authUser.FullName = reader["real_name"].ToString();
                        authUser.Phone = reader["tel"].ToString();
                        authUser.UnID = string.IsNullOrEmpty(reader["un_id"].ToString())
                            ? 0
                            : Convert.ToInt32(reader["un_id"].ToString());
                        authUser.Role = Convert.ToInt32("0" + reader["role"]);
                        authUser.IdentCode = reader["said_kod"].ToString();
                        authUser.Address = reader["address_full"].ToString();
                        authUser.Tin = reader["tin"].ToString();
                        authUser.SamFormaID = reader["samforma"].ToString().ToNumber<int>();
                        authUser.SamFormaName = reader["samforma_name"].ToString();
                        authUser.Username = reader["user_name"].ToString();
                       
                        authUser.UserType = string.IsNullOrEmpty(reader["user_type"].ToString())
                            ? 0
                            : int.Parse(reader["user_type"].ToString());
                        authUser.SecretWord = reader["secret_word"].ToString().ToNumber<int>();
                        authUser.MobileVerification = reader["mobile_verif"].ToString().ToNumber<int>();
                        authUser.SubUserID = authUser.UserType == 5 ? int.Parse(reader["sub_user_id"].ToString()) : 0;
                        authUser.PassExpire = DateTime.Parse(reader["password_expire_date"].ToString());
                        authUser.IsVatPayer = reader["is_vat_payer"].ToString().ToNumber<int>() == 1;
                        authUser.ShowOrgInfo = reader["show_org_info"].ToString().ToNumber<int>() == 1;
                        authUser.TestUser = reader["test_user"].ToString().ToNumber<int>() == 1;
                        authUser.InsCode = reader["ins_kodi"].ToString();
                        authUser.SubUserName = reader["subuser_name"].ToString();
                        if (authUser.UnionOrgs == null)
                            authUser.UnionOrgs = new List<UnionOrg>
                            {
                                new UnionOrg
                                {
                                    IsAvtive = true,
                                    UnID = authUser.UnID,
                                    OrgName = string.Format("{0}({1})", authUser.FullName, authUser.Tin)
                                }
                            };
                        break;
                    case 1:
                        if (authUser == null || authUser.IsCanceled)
                            break;
                        DateTime pinDate;
                        DateTime? pinDateNull = null;

                        if (DateTime.TryParseExact(reader["PINDATE"].ToString(), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out pinDate))
                            pinDateNull = pinDate;

                        authUser.Modules.Add(new Module(reader["MODULE_ID"].ToString().ToNumber<int>(),
                            reader["APP_NAME"].ToString(), reader["APP_NAME_EN"].ToString(),
                            reader["GROUP_NAME"].ToString(), reader["APP_URL"].ToString(),
                            reader["PERMISSION"].ToString().ToNumber<long>(), reader["COLOR"].ToString(),
                            reader["PINNED"].ToString().ToNumber<int>(), reader["DESCRIPTION"].ToString(),
                            reader["NOTIFICATION_PROCEDURE"].ToString(),
                            reader["login_notif_procedure"].ToString(), reader["EXTERNAL_SERVICE"].ToString().ToNumber<int>(), pinDateNull));
                        break;
                    case 2:
                        if (authUser == null || authUser.IsCanceled)
                            break;
                        authUser.UnionOrgs.Add(new UnionOrg
                        {
                            UnID = reader["old_un_id"].ToString().ToNumber<int>(),
                            OrgName = reader["org_name"].ToString()

                        });
                        break;
                    case 3:
                        if (authUser == null || authUser.IsCanceled)
                            break;
                        authUser.TrustingUsers.Add(new TrustingUser(reader["full_name"].ToString(),
                            reader["user_id"].ToString().ToNumber<int>(),
                            reader["user_type"].ToString().ToNumber<int>(),
                            reader["user_name"].ToString(),
                            reader["tin"].ToString(),
                            reader["samforma_id"].ToString().ToNumber<int>(),
                            reader["samforma_name"].ToString()
                        ));
                        break;
                }
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public string get_dutyfree_point(int subUserID)
        {
            var result = string.Empty;
            string error;
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.get_dutyfree_point");
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            
            new OracleDb<PKG_AUTHENTICATION>().ExecuteNonQuery(cmd, out error, delegate {
                result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return result;
        }

        public string get_taxfree_license(int subUserID)
        {
            var result = string.Empty;
            string error;
            var cmd = new OracleCommand("rstask.PKG_AUTHENTICATION.get_taxfree_license");
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_AUTHENTICATION>().ExecuteNonQuery(cmd, out error, delegate {
                result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return result;
        }

        public List<Module> Get_User_Modules_list(int userId)
        {
            var res = new List<Module>();
            var cmd = new OracleCommand("rstask.PKG_USERS.get_permissions_list");

            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_user_perm", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_USERS>().ProcessEachMultisetRow(cmd, out string error,
                delegate (OracleDataReader reader, int index)
                {
                    res.Add(new Module(reader["MODULE_ID"].ToString().ToNumber<int>(),
                            reader["APP_NAME"].ToString(), reader["APP_NAME_EN"].ToString(),
                            reader["GROUP_NAME"].ToString(), reader["APP_URL"].ToString(),
                            reader["PERMISSION"].ToString().ToNumber<long>(), reader["COLOR"].ToString(),
                            reader["PINNED"].ToString().ToNumber<int>(), reader["DESCRIPTION"].ToString(),
                            reader["NOTIFICATION_PROCEDURE"].ToString(),
                            reader["login_notif_procedure"].ToString(),
                            reader["EXTERNAL_SERVICE"].ToString().ToNumber<int>(),
                            null /*PIN DATE ს არ ვაბრუნებ*/
                        )
                    );
                });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return res;
        }
    }
}
