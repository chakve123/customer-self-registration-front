using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BaseLib.Common;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using EservicesLib.User;
using Oracle.DataAccess.Client;
using AuthUser = EservicesLib.User.AuthUser;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_ESERVICES_LOGS : DataProvider
    {
        public void SaveEservicesLogs(int moduleId, string requestUri, int userId, int unId, int subUserId, string ip, TimeSpan requestTimeInterval, string requestParams)
        {
            var cmd = new OracleCommand("logs.PKG_ESERVICES_LOGS.set_eServicesLogs");
            cmd.Parameters.Add("p_module_id", OracleDbType.Int32).Value = moduleId;
            cmd.Parameters.Add("p_request_uri", OracleDbType.Varchar2).Value = requestUri;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = ip;
           // cmd.Parameters.Add("p_request_time", OracleDbType.Int32).Value = requestTime;
            cmd.Parameters.Add("p_request_time_interval", OracleDbType.IntervalDS).Value = requestTimeInterval;
            
            cmd.Parameters.Add("p_request_params", OracleDbType.Clob).Value = requestParams;

            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_ESERVICES_LOGS>().ExecuteNonQuery(cmd, out string error);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

    }
}