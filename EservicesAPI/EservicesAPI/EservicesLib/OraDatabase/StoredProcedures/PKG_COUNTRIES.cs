using Oracle.DataAccess.Client;
using System;
using System.Data;
using BaseLib.OraDataBase;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_COUNTRIES : DataProvider
    {
        public DataTable get_countries()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("country_id");
            dt.Columns.Add("name");

            OracleCommand Cmd = new OracleCommand("cmn.PKG_COUNTRIES.get_countries");
            Cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            Cmd.CommandType = CommandType.StoredProcedure;

            string error = String.Empty;
            new OracleDb<PKG_COUNTRIES>().ProcessEachRow(Cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["country_id"], reader["name"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return dt;
        }
    }
}
