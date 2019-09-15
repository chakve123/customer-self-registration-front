using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using Oracle.DataAccess.Client;


namespace BaseLib.OraDataBase.StoredProcedures
{
    public class PKG_ERROR_LOGS : DataProvider
    {
        public void save_logs(string ipAddress, string serverAddress, string userName, string errorText, string errorTrace, string innerException, string customText, string browserInfo)
        {
            var cmd = new OracleCommand(ConfigurationManager.AppSettings["error_log_DBprocedure"]);
            cmd.Parameters.Add("p_ip_address", OracleDbType.Varchar2).Value = ipAddress;
            cmd.Parameters.Add("p_server_address", OracleDbType.Varchar2).Value = serverAddress;
            cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = userName;
            cmd.Parameters.Add("p_error_text", OracleDbType.Varchar2).Value = errorText;
            cmd.Parameters.Add("p_error_trace", OracleDbType.Varchar2).Value = errorTrace;
            cmd.Parameters.Add("p_innerException", OracleDbType.Varchar2).Value = innerException;
            cmd.Parameters.Add("p_custom_text", OracleDbType.Clob).Value = customText;
            cmd.Parameters.Add("p_browser_info", OracleDbType.Varchar2).Value = browserInfo;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_ERROR_LOGS>().ExecuteNonQuery(cmd, out error, threads: new List<Thread>());

            if (!string.IsNullOrEmpty(error))
            {
                try
                {
                    string filePath = ConfigurationManager.AppSettings.GetValues("ServiceDownLogs")[0] + "Exceptions.xls";
                    string sConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;\";", filePath);

                    var objConn = new OleDbConnection(sConnectionString);

                    objConn.Open();

                    var cmd1 = new OleDbCommand("INSERT INTO [Sheet1$] " +
                    "(LOG_ERROR,LOG_TIME,USER_NAME,INNER_EXCEPTION,CUSTOM_TEXT,IP_ADDRESS,ERROR_TEXT,SERVER_ADDRESS) VALUES(@value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8)", objConn);
                    cmd1.Parameters.AddWithValue("@value1", error);
                    cmd1.Parameters.AddWithValue("@value2", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@value3", userName);
                    cmd1.Parameters.AddWithValue("@value4", innerException);
                    cmd1.Parameters.AddWithValue("@value5", customText);
                    cmd1.Parameters.AddWithValue("@value6", ipAddress);
                    cmd1.Parameters.AddWithValue("@value7", errorText);
                    cmd1.Parameters.AddWithValue("@value8", serverAddress);
                    cmd1.ExecuteNonQuery();

                    objConn.Close();
                }
                catch
                {
                    //MessageBox.Show(ConfigurationManager.AppSettings["error_text"] + " #2");
                }
            }
        }
    }
}
