using BaseLib.OraDataBase;
using CustomSelfDeclarationLib.OraDatabase.Models;
using Oracle.DataAccess.Client;
using System;
using System.Data;

namespace CustomSelfDeclarationLib.OraDatabase.StoredProcedures
{
    public class PKG_PERSON_INFO : DataProvider
    {
        public PersonInfo get_person_info(string personID)
        {
            PersonInfo personInfo = null;

            var cmd = new OracleCommand("cmn.PKG_PERSON_INFO.get_person_info");
            cmd.Parameters.Add("p_person_id", OracleDbType.Varchar2, 1000).Value = personID;
            cmd.Parameters.Add("p_initials", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_address", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_birth_location", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_birth_date", OracleDbType.Date).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = string.Empty;
            new OracleDb<PKG_PERSON_INFO>().ExecuteNonQuery(cmd, out error, delegate (OracleParameterCollection reader)
            {
                personInfo = new PersonInfo();
                personInfo.Initials = ((Oracle.DataAccess.Types.OracleString)reader["p_initials"].Value).IsNull ? string.Empty : reader["p_initials"].Value.ToString();
                personInfo.Address = ((Oracle.DataAccess.Types.OracleString)reader["p_address"].Value).IsNull ? string.Empty : reader["p_address"].Value.ToString();
                personInfo.BirthLocation = ((Oracle.DataAccess.Types.OracleString)reader["p_birth_location"].Value).IsNull ? string.Empty : reader["p_birth_location"].Value.ToString();
                personInfo.BirthDate = DateTime.Parse(reader["p_birth_date"].Value.ToString());
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return personInfo;
        }

        public PersonInfo get_guest_info(string personID)
        {
            PersonInfo personInfo = null;

            var cmd = new OracleCommand("cmn.PKG_PERSON_INFO.get_customs_cross_person_info");
            cmd.Parameters.Add("p_document_no", OracleDbType.Varchar2, 1000).Value = personID;
            cmd.Parameters.Add("p_initials", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
          
            cmd.Parameters.Add("p_birth_date", OracleDbType.Date).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = string.Empty;
            new OracleDb<PKG_PERSON_INFO>().ExecuteNonQuery(cmd, out error, delegate (OracleParameterCollection reader)
            {
                personInfo = new PersonInfo();
                personInfo.Initials = ((Oracle.DataAccess.Types.OracleString)reader["p_initials"].Value).IsNull ? string.Empty : reader["p_initials"].Value.ToString();
               
                personInfo.BirthDate = DateTime.Parse(reader["p_birth_date"].Value.ToString()).ToUniversalTime();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return personInfo;
        }
    }
}