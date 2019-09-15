using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BaseLib.Common;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using EservicesLib.User;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using AuthUser = EservicesLib.User.AuthUser;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_TRANSACTION_LOG : DataProvider
    {
        /// <summary>
        /// აბრუნებს response ს თუ არსებობს მსგავსი UserID თა და Transaction ID ით, თუ არადა ქმნის ახალ ჩანაწერს response ის გარეშე
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionID"></param>
        public Tuple<int?, string> GET_CREATE_TRANSACTION(int userId, string transactionID)
        {
            var cmd = new OracleCommand("logs.PKG_TRANSACTION_LOG.GET_CREATE_TRANSACTION");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;
          
            cmd.Parameters.Add("p_transaction_id", OracleDbType.Varchar2).Value = transactionID;

            cmd.Parameters.Add("p_out_status", OracleDbType.Int32).Direction = ParameterDirection.Output;

            cmd.Parameters.Add("p_out_result", OracleDbType.Clob).Direction = ParameterDirection.Output;

            cmd.CommandType = CommandType.StoredProcedure;

            string result = string.Empty;
            int? status = null;
            new OracleDb<PKG_TRANSACTION_LOG>().ExecuteNonQuery(cmd, out string error, delegate
            {

                
                if ((OracleClob)(cmd.Parameters["p_out_result"]).Value != OracleClob.Null) result = ((OracleClob)(cmd.Parameters["p_out_result"]).Value).Value.ToString();

               
                if (cmd.Parameters["p_out_status"].Value.ToString() != "null") status = cmd.Parameters["p_out_status"].Value.ToString().ToNumber<int>();
               
            });
            
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return Tuple.Create(status, result); 
        }

        /// <summary>
        /// Status Code : 0 = > ჩაწერა Response ი. Response - > ცარიელი
        /// Stat Code :  -6 => ასეთი ტრანზაქცია არსებობს. ტრანსაქციის სტატუსი: მიმდინარე , Response - > ცარიელი
        /// Stat Code :  -7 => ასეთი ტრანზაქცია არსებობს. ტრანსაქციის სტატუსი: დასრულებული , Response - > წინა Transaction ID იანი Request ის დროს ჩაწერილი Response
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionID"></param>
        /// <param name="transactionResponse"></param>
        /// <returns></returns>
        public void SET_TRANSACTION_RESPONSE(int userId, string transactionID, string transactionResponse)
        {
            var cmd = new OracleCommand("logs.PKG_TRANSACTION_LOG.SET_TRANSACTION_RESPONSE");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;

            cmd.Parameters.Add("p_transaction_id", OracleDbType.Varchar2).Value = transactionID;

            cmd.Parameters.Add("p_transaction_result", OracleDbType.Clob).Value = transactionResponse;

            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out string error);

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }


        public void DELETE_TRANSACTION_RESPONSE(int userId, string transactionID)
        {
            var cmd = new OracleCommand("logs.PKG_TRANSACTION_LOG.DELETE_TRANSACTION_RESPONSE");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;

            cmd.Parameters.Add("p_transaction_id", OracleDbType.Varchar2).Value = transactionID;

            cmd.CommandType = CommandType.StoredProcedure;

            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out string error);

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            
        }


        public Tuple<int?, string> GET_TRANSACTION_RESULT(int userId, string transactionID)
        {
            var cmd = new OracleCommand("logs.PKG_TRANSACTION_LOG.GET_TRANSACTION_RESULT");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userId;

            cmd.Parameters.Add("p_transaction_id", OracleDbType.Varchar2).Value = transactionID;

            cmd.Parameters.Add("p_out_status", OracleDbType.Int32).Direction = ParameterDirection.Output;

            cmd.Parameters.Add("p_out_result", OracleDbType.Clob).Direction = ParameterDirection.Output;

            cmd.CommandType = CommandType.StoredProcedure;

            string result = string.Empty;
            int? status = null;
            new OracleDb<PKG_TRANSACTION_LOG>().ExecuteNonQuery(cmd, out string error, delegate
            {
                if ((OracleClob)(cmd.Parameters["p_out_result"]).Value != OracleClob.Null) result = ((OracleClob)(cmd.Parameters["p_out_result"]).Value).Value.ToString();

                if (cmd.Parameters["p_out_status"].Value.ToString() != "null") status = cmd.Parameters["p_out_status"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return Tuple.Create(status, result);
        }

        public int MethodWhichThrowsExceptionTest()
        {

            throw new Exception("test");
        }

    }
}