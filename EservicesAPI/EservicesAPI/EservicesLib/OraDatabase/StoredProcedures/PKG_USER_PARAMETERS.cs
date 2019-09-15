using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using EservicesLib.OraDatabase.Models;
using System.Data;
using BaseLib.ExtensionMethods;
using System.Threading;
using BaseLib.Exceptions;
using System.Globalization;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_USER_PARAMETERS : DataProvider
    {
        public PersonalInfo GetPersonalInfo(int userID)
        {
            var personalInfo = new PersonalInfo();
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.get_personal_info_n");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_USER_PARAMETERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                personalInfo.Email = reader["EMAIL"].ToString();
                personalInfo.Phone = reader["PHONE"].ToString();
                personalInfo.Address = reader["ADDRESS"].ToString();
                personalInfo.SecretWord = reader["SECRET_WORD"].ToString().ToNumber<int>();
                personalInfo.PasswordExpireDate = (DateTime)reader["PASSWORD_EXPIRE_DATE"];
                personalInfo.PasswordChangeDate = personalInfo.PasswordExpireDate.AddMonths(-2).AddDays(1);
                personalInfo.TwoWayAuthStatus = reader["TWO_WAY_AUTH_STATUS"].ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception();
            return personalInfo;
        }

        public PersonalInfo GetSubPersonalInfo(int userID, int subUserID)
        {
            var personalInfo = new PersonalInfo();
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.get_subuser_info_n");
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_USER_PARAMETERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                personalInfo.Email = reader["EMAIL"].ToString();
                personalInfo.Phone = reader["PHONE_NUMBER"].ToString();
                personalInfo.SecretWord = reader["SECRET_WORD"].ToString().ToNumber<int>();
                personalInfo.TwoWayAuthStatus = reader["TWO_WAY_AUTH_STATUS"].ToString().ToNumber<int>();
                personalInfo.PasswordChangeDate= (DateTime)(reader["PASSWORD_CHANGE_DATE"]);

            });

            if (!string.IsNullOrEmpty(error)) throw new Exception();
            return personalInfo;
        }
        public void UpdateUserEmail(int userID, int subUserID, string email)
        {
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.update_user_email");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_user_email", OracleDbType.Varchar2).Value = email;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }
        public void UpdateUserPhone(int userID, int subUserID, string phone)
        {
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.update_user_phone");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_user_phone", OracleDbType.Varchar2).Value = phone;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

  
        public int UpdateSecretWord(int userId, int subUserId, string word)
        {
            var status = 0;
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.update_secret_word");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_word", OracleDbType.Varchar2).Value = word;
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error, delegate
            {
                status = cmd.Parameters["p_status"].Value.ToString().ToNumber<int>();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return status;
        }

        public void UpdateUserAddress(int unId ,string region, string district, string streets, string placenumber)
        {
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.update_user_address");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unId;
            cmd.Parameters.Add("p_region", OracleDbType.Varchar2).Value = region;
            cmd.Parameters.Add("p_district", OracleDbType.Varchar2).Value = district;
            cmd.Parameters.Add("p_streets", OracleDbType.Varchar2).Value = streets;
            cmd.Parameters.Add("p_placenumber", OracleDbType.Varchar2).Value = placenumber;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public AddressDetail GetAddressDetails(int un_id)
        {
            var address = new AddressDetail();
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.get_address_details");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = un_id;
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                address.RegionId = reader["region_id"].ToString();
                address.DistrictId = reader["district_id"].ToString();
                address.StreetId = reader["street_id"].ToString();
                address.StreetText = reader["street_text"].ToString();
                address.PlaceNumber = reader["place_number"].ToString();
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return address;
        }

        public List<UserContact> GetContacts(int unId)
        {
            var userContacts = new List<UserContact>();
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.get_contact_list");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unId;
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                var userContact = new UserContact();
                userContact.SetId = decimal.Parse(reader["set_id"].ToString());
                userContact.UnId = decimal.Parse(reader["un_id"].ToString());
                userContact.AppId = decimal.Parse(reader["app_id"].ToString());
                userContact.Visbility = decimal.Parse(reader["visibility"].ToString()) == 1;
                //userContact.Notify = decimal.Parse(reader["notify"].ToString()) == 1;
                //userContact.NotifyMain = decimal.Parse(reader["notify_main"].ToString()) == 1;
                userContact.AppName = reader["app_name"].ToString();
                userContact.AppColor = reader["app_icolor"].ToString();
                userContact.Mobile = reader["mobile"].ToString();
                //userContact.Mobile2 = reader["mobile2"].ToString();
                userContact.Phone = reader["phone"].ToString();
                userContact.Email = reader["email"].ToString();
                userContact.SendNotifyChecks = reader["send_notify_checks"].ToString().ToNumber<int>() == 1;
                userContact.SwitcherClass = (userContact.Visbility ? " PUBLIC " : " PRIVATE "); //+ (userContact.SendNotifyChecks && (userContact.Notify || userContact.NotifyMain) ? " NOTIFY " : " ");
                userContact.VisibilityTooltip = userContact.Visbility
                    ? "საკონტაქტო ინფორმაცია ხელმისაწვდომია გადამხდელთათვის"
                    : "საკონტაქტო ინფორმაცია დაფარულია";
                //userContact.NotifyTooltip = userContact.Notify ? "ამ მოდულზე თქვენ გეგზავნებათ შეტყობინებები" : "";
                userContact.ModuleVisibility = reader["module_visibility"].ToString();
                userContacts.Add(userContact);
            });
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return userContacts;
        }

        public UserContact GetModuleContact(int unId,string settingId, string moduleId)
        {
            var userContact = new UserContact();
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.get_module_contact");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unId;
            cmd.Parameters.Add("p_set_id", OracleDbType.Int32).Value = int.Parse(settingId);
            cmd.Parameters.Add("p_app_id", OracleDbType.Int32).Value = int.Parse(moduleId);
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                userContact.SetId = decimal.Parse(reader["set_id"].ToString());
                userContact.UnId = decimal.Parse(reader["un_id"].ToString());
                userContact.AppId = decimal.Parse(reader["app_id"].ToString());
                userContact.Visbility = decimal.Parse(reader["visibility"].ToString()) == 1;
                //UserContact.Notify = decimal.Parse(reader["notify"].ToString()) % 2 == 1;
                //UserContact.NotifyMain = decimal.Parse(reader["notify_main"].ToString()) > 1;
                userContact.AppName = reader["app_name"].ToString();
                userContact.AppColor = reader["app_icolor"].ToString();
                userContact.Mobile = reader["mobile"].ToString();
                //UserContact.Mobile2 = reader["mobile2"].ToString();
                userContact.Phone = reader["phone"].ToString();
                userContact.Email = reader["email"].ToString();
                userContact.SwitcherClass = (userContact.Visbility ? " PUBLIC " : " PRIVATE "); // + (UserContact.Notify ? " NOTIFY " : " ");
                userContact.VisibilityTooltip = userContact.Visbility
                    ? "საკონტაქტო ინფორმაცია ხელმისაწვდომია გადამხდელთათვის"
                    : "საკონტაქტო ინფორმაცია დაფარულია";
                userContact.NotifyTooltip = userContact.Notify ? "ამ მოდულზე თქვენ გეგზავნებათ შეტყობინებები" : "";
                userContact.ModuleVisibility = reader["module_visibility"].ToString();
                userContact.SendNotifyText = reader["send_notify_text"].ToString();
                userContact.SendNotifyChecks = reader["send_notify_checks"].ToString().ToNumber<int>() == 1;
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return userContact;
        }
        public void SaveModuleContact(int unId,UserContact contact)
        {
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.save_module_contact");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unId ;
            cmd.Parameters.Add("p_app_id", OracleDbType.Int32).Value = contact.AppId;
            cmd.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = contact.Mobile;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = contact.Email;
            //Cmd.Parameters.Add("p_notify", OracleDbType.Int32).Value = contact.Notify ? (contact.NotifyMain ? 3 : 1) : (contact.NotifyMain ? 2 : 0);
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Value = contact.Visbility ? 1 : 0;
            cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = contact.Phone;
            //cmd.Parameters.Add("p_mobile2", OracleDbType.Varchar2).Value = contact.Mobile2;
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }
        public void ToggleModuleContact(string settingId,int UnId)
        {
            var cmd = new OracleCommand("tp.PKG_USER_PARAMETERS.toggle_module_contact");
            cmd.Parameters.Add("setting_id", OracleDbType.Int32).Value = int.Parse(settingId);
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = UnId;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_USER_PARAMETERS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }


    }
}
