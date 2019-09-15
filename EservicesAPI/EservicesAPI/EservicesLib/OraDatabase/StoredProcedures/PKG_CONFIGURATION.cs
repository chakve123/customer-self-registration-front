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
using Oracle.DataAccess.Types;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_CONFIGURATION : DataProvider
    {

      public Dictionary<string , byte[]> GetRouteEncryptionKeys()
        {
            var res = new Dictionary<string, byte[]>();
            var cmd = new OracleCommand("PKG_CONFIG.GET_RouteEncryptionKeys");
            
            cmd.Parameters.Add("P_Keys", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_MENU>().ProcessEachRow(cmd, out string error, delegate(OracleDataReader reader)
            {
                string key = (string)reader["KEY"];
                byte[] value = (byte[])reader["VALUE"];

                if (!String.IsNullOrWhiteSpace(key))
                {
                    res.Add(key,value);
                }
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return res;
        }

        public void SetRouteEncryptionKeys(byte[] key, byte[] iv)
        {
            var cmd = new OracleCommand("PKG_CONFIG.SET_RouteEncryptionKeys");
            cmd.Parameters.Add("P_Key", OracleDbType.Blob, ParameterDirection.Input).Value = key;
            cmd.Parameters.Add("P_Key", OracleDbType.Blob, ParameterDirection.Input).Value = iv;
            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_USERS>().ExecuteNonQuery(cmd,out string error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

    }
}