using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_SEARCHTAXPAYERS: DataProvider
    {
        public byte[] reportTypeReg(int unid)
        {

            var cmd = new OracleCommand("PLPDF.REPORTS_RSGE.GET_REGISTRY");
            cmd.Parameters.Add("p_unid", OracleDbType.Long).Value = unid;
            cmd.Parameters.Add("rep_out", OracleDbType.Blob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            byte[] result = new byte[0];
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleBlob)(cmd.Parameters["rep_out"].Value)).Value;
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public byte[] reportTypeOther(int unid)
        {

            var cmd = new OracleCommand("PLPDF.REPORTS_RSGE.GET_OTHER");
            cmd.Parameters.Add("p_unid", OracleDbType.Long).Value = unid;
            cmd.Parameters.Add("rep_out", OracleDbType.Blob, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            byte[] result = new byte[0];
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleBlob)(cmd.Parameters["rep_out"].Value)).Value;
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;

        }
    }
}
