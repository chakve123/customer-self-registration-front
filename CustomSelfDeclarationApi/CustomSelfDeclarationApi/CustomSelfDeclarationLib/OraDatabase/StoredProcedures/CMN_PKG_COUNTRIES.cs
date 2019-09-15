using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSelfDeclarationLib.OraDatabase.StoredProcedures
{
    public class CMN_PKG_COUNTRIES : DataProvider
    {
        public DataTable get_countries()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("country_id");
            dt.Columns.Add("name");

            OracleCommand Cmd = new OracleCommand("cmn.PKG_COUNTRIES.get_countries");
            Cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            Cmd.CommandType = CommandType.StoredProcedure;

            string error = string.Empty;
            new OracleDb<CMN_PKG_COUNTRIES>().ProcessEachRow(Cmd, out error, delegate (OracleDataReader reader)
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
