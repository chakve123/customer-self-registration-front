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
using EservicesLib.User;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_REGION:DataProvider
    {
        public List<string[]> GetAddressList(string parentID = "0000001")
        {
            var data = new List<string[]>();

            var cmd = new OracleCommand("cmn.pkg_address.get_address_list");
            cmd.Parameters.Add("p_parent_id", OracleDbType.NVarchar2, 1000).Value = parentID;
            cmd.Parameters.Add("p_out_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_REGION>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader) 
            {
                data.Add(new string[] { reader["ID"].ToString(), reader["NAME"].ToString() });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return data;
        }

    


    }
}
