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
    public class PKG_MENU : DataProvider
    {

        public string GetNotificationsJson(string moduleIds, int notifType, int unID, int subUserID)
        {
            var cmd = new OracleCommand("RSTASK.PKG_MENU.get_notifications_json");

            cmd.Parameters.Add("p_modules", OracleDbType.Clob).Value = moduleIds;
            cmd.Parameters.Add("p_type", OracleDbType.Int32).Value = notifType;
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_sub_user", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out string error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public byte[] GetModuleGroupIcon(int moduleId)
        {
            byte[] reportBytes;
            var cmd = new OracleCommand("rstask.PKG_MENU.get_group_icon");
            cmd.Parameters.Add("p_moduleID", OracleDbType.Int32).Value = moduleId;
            cmd.Parameters.Add("rep_out", OracleDbType.Blob, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;
            new OracleDb<PKG_MENU>().ExecuteNonQuery(cmd, true, out string error);
            if (((Oracle.DataAccess.Types.OracleBlob)cmd.Parameters["rep_out"].Value).IsNull)
            {
                reportBytes = null;
            }
            else
            {
                reportBytes = (byte[])(((Oracle.DataAccess.Types.OracleBlob)cmd.Parameters["rep_out"].Value).Value);
            }
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return reportBytes;
        }

        public List<Tuple<int, byte[]>> GetModuleGroupIcons(string moduleIds)
        {
            var res = new List<Tuple<int, byte[]>>();
            var cmd = new OracleCommand("rstask.PKG_MENU.get_group_icons");
            cmd.Parameters.Add("p_moduleIDs", OracleDbType.Clob).Value = moduleIds;
           
            cmd.Parameters.Add("rep_out", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_MENU>().ProcessEachRow(cmd, out string error, delegate(OracleDataReader reader)
            {
                //var image = (byte[]) (((Oracle.DataAccess.Types.OracleBlob) reader["Image"]).Value);

                byte[] temp = (byte[])reader["Image"];




                if (temp != null)
                {
                    var test = (temp);

                    if (temp.Length > 0) {
                        res.Add(new Tuple<int, byte[]>(Int32.Parse(reader["ModuleID"].ToString()), (temp)));
                    }
                    
                }
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return res;
        }


        public bool ToggleModulePin(int moduleID, int authUserID)
        {
            OracleCommand Cmd = new OracleCommand("rstask.PKG_MENU.SET_MODULE_PIN");
            Cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = authUserID;
            Cmd.Parameters.Add("p_module_id", OracleDbType.Int32).Value = moduleID;
            Cmd.Parameters.Add("p_out_pinned", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_out_date", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            Cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_MENU>().ExecuteNonQuery(Cmd, out string error, delegate (OracleParameterCollection reader)
            {

            });

            return string.IsNullOrEmpty(error);
        }
    }
}