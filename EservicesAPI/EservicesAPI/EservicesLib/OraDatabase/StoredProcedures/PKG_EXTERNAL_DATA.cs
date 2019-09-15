using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using BaseLib.OraDataBase;
using System.Threading.Tasks;
using EservicesLib;
using EservicesLib.OraDatabase.Models;

namespace EservicesLib.OraDatabase.StoredProcedures
{
   public class PKG_EXTERNAL_DATA: DataProvider
    {
        #region XLS MODULE REGION
        public List<ED_Template> GetTemplatesList(int unID)
        {
            var templateList = new List<ED_Template>();
            var cmd = new OracleCommand("external_data.pkg_external_data.GET_TEMPLATES_LIST");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_OUT_CURS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_EXTERNAL_DATA>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                var template = new ED_Template
                {
                    ID = int.Parse(reader["ID"].ToString()),
                    templateName = reader["TEMPLATE_NAME"].ToString(),
                    templateDate=reader["UPLOAD_DATE"].ToString()      
                };

                templateList.Add(template);
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception();

            return templateList;
        }

        public byte[] GetLastUpload(int unID,int id)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.GET_LAST_UPLOAD");
            cmd.Parameters.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input).Value = id;
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32, ParameterDirection.Input).Value = unID;
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

        public byte[] GetTemplate(int unID,int id)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.GET_TEMPLATE");
            cmd.Parameters.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input).Value = id;
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32, ParameterDirection.Input).Value = unID;
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

        public string SaveTemplate(int unID , int subUserID, byte[] upload_file,string upload_file_name,int template_id,string json)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.save_template");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_SUBUSER_ID", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("P_UPLOAD_FILE", OracleDbType.Blob).Value = upload_file;
            cmd.Parameters.Add("P_UPLOAD_FILE_NAME", OracleDbType.Varchar2).Value = upload_file_name;
            cmd.Parameters.Add("P_TEMPLATE_ID", OracleDbType.Int32).Value = template_id;
            cmd.Parameters.Add("P_JSON", OracleDbType.Clob).Value = json;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            string result = "";
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
               //result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return result;
        }
        #endregion

        #region JSON SERVICE REGION

        public string SendJsonData(int unID, int tempId, int subUserId)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.SEND_JSON_DATA");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_template_id", OracleDbType.Int32).Value = tempId;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserId;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public void GetJson(int unID, int subUserID, int template_id, string json)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.INSERT_DATA");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_SUBUSER_ID", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("P_TEMPLATE_ID", OracleDbType.Int32).Value = template_id;
            cmd.Parameters.Add("P_JSON", OracleDbType.Clob).Value = json;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            string result = "";
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        public void SaveTemplateJson(int unID, int subUserID, string json, int template_id)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.SAVE_TEMPLATE_JSON");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_SUBUSER_ID", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("P_JSON", OracleDbType.Clob).Value = json;
            cmd.Parameters.Add("P_TEMPLATE_ID", OracleDbType.Int32).Value = template_id;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            string result = "";
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        #endregion

        #region CONTACT REGION

        public void SaveContactInfo(int unID, string phone ,string email)
        {
            var cmd = new OracleCommand("external_data.pkg_external_data.SAVE_CONTACT_INFO");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = phone;
            cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            string result = "";
            new OracleDb<PKG_EXTERNAL_DATA>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = cmd.Parameters["p_result"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        public ED_Contact GetContactInfo(int unID)
        {
            ED_Contact contact = null;
            var cmd = new OracleCommand("external_data.pkg_external_data.GET_CONTACT_INFO");
            cmd.Parameters.Add("P_UN_ID", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("P_OUT_CURS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;

            new OracleDb<PKG_EXTERNAL_DATA>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                  contact = new ED_Contact
                {
                    UN_ID = int.Parse(reader["UN_ID"].ToString()),
                    PHONE = reader["USER_PHONE"].ToString(),
                    EMAIL = reader["USER_EMAIL"].ToString()
                };
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception();

            return contact;
        }

        #endregion
    }
}
