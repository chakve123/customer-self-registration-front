using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_POLL:DataProvider
    {
        public void insert_FeedBack(int p_ModID, string p_FeedBack, string p_contact, int? p_userID, int? p_subUserID, string p_IP, string p_webPage)
        {
            var cmd = new OracleCommand("rsweb.PKG_POLL.insert_FeedBack");
            cmd.Parameters.Add("p_ModID", OracleDbType.Int32).Value = p_ModID;
            cmd.Parameters.Add("p_FeedBack", OracleDbType.Varchar2).Value = p_FeedBack;
            cmd.Parameters.Add("p_contact", OracleDbType.Varchar2).Value = p_contact;
            cmd.Parameters.Add("p_userID", OracleDbType.Int32).Value = p_userID;
            cmd.Parameters.Add("p_subUserID", OracleDbType.Int32).Value = p_subUserID;
            cmd.Parameters.Add("p_IP", OracleDbType.Varchar2).Value = p_IP;
            cmd.Parameters.Add("p_webPage", OracleDbType.Varchar2).Value = p_webPage;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_POLL>().ExecuteNonQuery(cmd, out error);

            if (!String.IsNullOrEmpty(error))
            { throw new Exception(error); }
        }
    }
}
