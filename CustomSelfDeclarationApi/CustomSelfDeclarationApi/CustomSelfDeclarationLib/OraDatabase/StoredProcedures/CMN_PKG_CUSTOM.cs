using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using System;
using System.Data;

namespace CustomSelfDeclarationLib.OraDatabase.StoredProcedures
{
    public class CMN_PKG_CUSTOM : DataProvider
    {
        public DataTable get_custom_houses()
        {
            var dt = new DataTable();
            dt.Columns.Add("cust_id");
            dt.Columns.Add("cust_name_uni");

            var cmd = new OracleCommand("cmn.PKG_CUSTOM.get_custom_houses");
            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            new OracleDb<CMN_PKG_CUSTOM>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["cust_id"], reader["cust_name_uni"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return dt;
        }
    }
}
