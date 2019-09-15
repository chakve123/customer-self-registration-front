using System;
using System.Data;
using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class CMN_PKG_MEASURE_UNITS : DataProvider
    {
        public DataTable get_units_list()
        {
            var dt = new DataTable();
            dt.Columns.Add("value");
            dt.Columns.Add("label");

            var cmd = new OracleCommand("cmn.PKG_MEASURE_UNITS.get_units_list");
            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<CMN_PKG_MEASURE_UNITS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["id"], reader["name"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return dt;
        }

        public DataTable get_currency_list()
        {
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");

            var cmd = new OracleCommand("cmn.PKG_MEASURE_UNITS.get_currency_list");
            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<CMN_PKG_MEASURE_UNITS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["id"], reader["name"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return dt;
        }

        public DataTable get_positions_list()
        {
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");

            var cmd = new OracleCommand("cmn.PKG_MEASURE_UNITS.get_positions_list");
            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<CMN_PKG_MEASURE_UNITS>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["id"], reader["name"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return dt;
        }
    }
}
