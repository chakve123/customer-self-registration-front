using BaseLib.ExtensionMethods;
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
   public class PKG_COMMON:DataProvider
    {

        public Dictionary<string, int> GetUsersCount()
        {
            var usersCount = new Dictionary<string, int>();
            var cmd = new OracleCommand("CMN.PKG_STATISTICS.GET_USERS_COUNT");

            cmd.Parameters.Add("p_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error = String.Empty;
            new OracleDb<PKG_COMMON>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                usersCount.Add("Count", reader["COUNT"].ToString().ToNumber<int>());
                usersCount.Add("Individual", reader["individual"].ToString().ToNumber<int>());
                usersCount.Add("Company", reader["company"].ToString().ToNumber<int>());
            });

            if (!String.IsNullOrEmpty(error))
            {
                //throw new Exception(error);
            }

            return usersCount;
        }
    }
}
