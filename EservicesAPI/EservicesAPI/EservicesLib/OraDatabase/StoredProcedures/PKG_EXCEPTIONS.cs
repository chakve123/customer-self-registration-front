using System;
using System.Collections.Generic;
using System.Data;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_EXCEPTIONS : DataProvider
    {
        public void Throw(int statusID, int lang = 0, string customText = null)
        {
            var cmd = new OracleCommand("invoice.PKG_EXCEPTIONS.THROW");
            cmd.Parameters.Add("P_ID", OracleDbType.Int32).Value = statusID;
            cmd.Parameters.Add("P_LANG", OracleDbType.Int32).Value = lang;
            cmd.Parameters.Add("P_CUSTOM_TEXT", OracleDbType.Varchar2).Value = customText;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            
            new OracleDb<PKG_EXCEPTIONS>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public List<ErrorCode> get_error_codes()
        {
            var errorCodes = new List<ErrorCode>();

            var cmd = new OracleCommand("invoice.PKG_EXCEPTIONS.get_error_codes");
            cmd.Parameters.Add("p_result_curs", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_EXCEPTIONS>().ProcessEachRow(cmd, out error, delegate(OracleDataReader reader)
            {
                var errorCode = new ErrorCode();
                errorCode.ID = reader["id"].ToString().ToNumber<int>();
                errorCode.ErrorText = reader["error_text"].ToString();
                errorCode.ErrorTextEn = reader["error_Text_en"].ToString();
                errorCodes.Add(errorCode);
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return errorCodes;
        }
    }

    public class ErrorCode
    {
        public int ID { get; set; }

        public string ErrorText { get; set; }

        public string ErrorTextEn { get; set; }
    }
}
