using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.Reflection;

namespace EservicesLib.OraDatabase.StoredProcedures
{
   public class PKG_SMS : DataProvider
    {
        public bool SendEmail(string mailTo, string mailSubject, string mailMessage)
        {
            string returnValue = "";
            OracleCommand Cmd = new OracleCommand("cmn.PKG_NOTIFICATIONS.SEND_EMAIL_HTML");
            Cmd.Parameters.Add("mail_to", OracleDbType.Varchar2).Value = mailTo;
            Cmd.Parameters.Add("mail_subject", OracleDbType.Varchar2).Value = mailSubject;
            Cmd.Parameters.Add("mail_message", OracleDbType.Varchar2).Value = mailMessage;
            Cmd.CommandType = CommandType.StoredProcedure;

            string error = String.Empty;
            new OracleDb<PKG_SMS>().ExecuteNonQuery(Cmd, out error);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(returnValue));
        }

        public bool SendSms(string phoneNumber, string messageText, string controller, bool log = false)
        {
            string returnValue = "";
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;

            OracleCommand Cmd = new OracleCommand("cmn.PKG_NOTIFICATIONS.send_sms");
            Cmd.Parameters.Add("p_return_value", OracleDbType.Varchar2, 20).Direction = ParameterDirection.ReturnValue;
            Cmd.Parameters.Add("p_phone_number", OracleDbType.Varchar2).Value = phoneNumber;
            Cmd.Parameters.Add("p_message_text", OracleDbType.Varchar2).Value = messageText;
            Cmd.Parameters.Add("p_log", OracleDbType.Int32).Value = log ? 1 : 0;
            Cmd.Parameters.Add("p_module_name", OracleDbType.Varchar2, 2000).Value = "Web: " + assemblyName + ", " + controller;

            Cmd.CommandType = CommandType.StoredProcedure;

            string error = String.Empty;
            new OracleDb<PKG_SMS>().ExecuteNonQuery(Cmd, out error, delegate (OracleParameterCollection reader)
            {
                returnValue = Cmd.Parameters["p_return_value"].Value.ToString();
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(returnValue));
        }
    }
}
