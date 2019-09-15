using BaseLib.Common;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_ORG_INFO : DataProvider
    {
        public void get_un_id_from_tin(string tin, out int un_id, out string name)
        {
            var _un_id = -3;
            var _name = "";
            var cmd = new OracleCommand("tp.NTosService.get_un_id_from_tin");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("v_tin", tin);
            cmd.Parameters.Add("v_un_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("v_desc", OracleDbType.NVarchar2, 200).Direction = ParameterDirection.Output;

            string error;
            new OracleDb<PKG_ORG_INFO>().ExecuteNonQuery(cmd, out error, delegate (OracleParameterCollection reader)
            {
                try
                {
                    _un_id = int.Parse(cmd.Parameters["v_un_id"].Value.ToString());
                    _name = cmd.Parameters["v_desc"].Value.ToString();
                }
                catch (Exception ex)
                {
                    _un_id = -3;
                    _name = "";
                    CommonFunctions.CatchExceptions(ex, "", false);
                }
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            un_id = _un_id;
            name = _name;
        }

        public void get_user_info(ref string p_tin, DateTime p_operation_date, out string p_name, out string p_address, out bool p_is_vat, out bool p_is_diplomat, out bool p_is_register, out bool p_is_underaged, out string p_samforma)
        {
            var _p_tin = ""; var _p_name = ""; var _p_address = ""; var _p_is_vat = false; var _p_diplomat_end = 0; var _p_is_diplomat = false; var _p_is_register = false; var _p_is_underaged = false; var _p_samforma = "";

            var Cmd = new OracleCommand("cmn.PKG_ORG_INFO.get_user_info");
            Cmd.Parameters.Add("p_tin", OracleDbType.Varchar2, 1000, p_tin, ParameterDirection.InputOutput);
            Cmd.Parameters.Add("p_operation_date", OracleDbType.Date).Value = p_operation_date;
            Cmd.Parameters.Add("p_full_name", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_address", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_is_vat", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_is_diplomat", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_diplomat_end", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_is_register", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_is_underaged", OracleDbType.Int32).Direction = ParameterDirection.Output;
            Cmd.Parameters.Add("p_samforma", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

            Cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            new OracleDb<PKG_ORG_INFO>().ExecuteNonQuery(Cmd, out error, delegate (OracleParameterCollection reader)
            {
                _p_tin = ((Oracle.DataAccess.Types.OracleString)reader["p_tin"].Value).IsNull ? string.Empty : reader["p_tin"].Value.ToString();
                _p_name = ((Oracle.DataAccess.Types.OracleString)reader["p_full_name"].Value).IsNull ? string.Empty : reader["p_full_name"].Value.ToString();
                _p_address = ((Oracle.DataAccess.Types.OracleString)reader["p_address"].Value).IsNull ? string.Empty : reader["p_address"].Value.ToString();
                _p_is_vat = reader["p_is_vat"].Value.ToString().ToNumber<int>() == 1;
                _p_diplomat_end = reader["p_diplomat_end"].Value.ToString().ToNumber<int>();
                _p_is_register = reader["p_is_register"].Value.ToString().ToNumber<int>() > 0;
                _p_is_diplomat = reader["p_is_diplomat"].Value.ToString().ToNumber<int>() > 0;
                _p_is_underaged = reader["p_is_underaged"].Value.ToString().ToNumber<int>() == 1;
                _p_samforma = ((Oracle.DataAccess.Types.OracleString)reader["p_samforma"].Value).IsNull ? string.Empty : reader["p_samforma"].Value.ToString();
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            if (_p_diplomat_end > 0)
                throw new UserExceptions("მოცემული დიპლომატის საიდენტიფიკაციო კოდი გაუქმებულია");

            p_tin = _p_tin; p_name = _p_name; p_is_vat = _p_is_vat; p_is_register = _p_is_register; p_address = _p_address; p_is_diplomat = _p_is_diplomat; p_is_underaged = _p_is_underaged; p_samforma = _p_samforma;
        }

        public void get_non_resident(string p_tin, out string p_full_name)
        {
            var _p_full_name = "";

            var cmd = new OracleCommand("cmn.pkg_org_info.get_non_resident");
            cmd.Parameters.Add("p_doc_number", OracleDbType.Varchar2).Value = p_tin;
            cmd.Parameters.Add("p_out_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_ORG_INFO>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                _p_full_name = reader["full_name"].ToString();
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            p_full_name = _p_full_name;
        }

        public bool is_vat_payer(string tin, DateTime vatDate)
        {
            int result = 0;
            var cmd = new OracleCommand("cmn.pkg_org_info.is_vat_payer");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("result", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add("p_tin", OracleDbType.Varchar2, 50).Value = tin;
            cmd.Parameters.Add("p_vat_date", OracleDbType.Date).Value = vatDate;
            string error;

            new OracleDb<PKG_ORG_INFO>().ExecuteNonQuery(cmd, out error, delegate(OracleParameterCollection reader)
            {
                result = reader["result"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            var isVatPayer = result != 0;
            return isVatPayer;
        }


        public bool is_vat_payer(int unID, DateTime vatDate)
        {
            int result = 0;
            var cmd = new OracleCommand("cmn.pkg_org_info.is_vat_payer_un_id");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("result", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_vat_date", OracleDbType.Date).Value = vatDate;
            string error;

            new OracleDb<PKG_ORG_INFO>().ExecuteNonQuery(cmd, out error, delegate (OracleParameterCollection reader)
            {
                result = reader["result"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            var isVatPayer = result != 0;
            return isVatPayer;
        }

        public List<object> GetStreets(string pSettCode, string pText)
        {
            var data = new List<object>();
            var cmd = new OracleCommand("rstask.PKG_ORGINFO.get_addresses");
            cmd.Parameters.Add("p_parent_id", OracleDbType.Varchar2).Value = pSettCode;
            cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = pText;
            cmd.Parameters.Add("p_out_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_REGION>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                string[] arr = { reader["ID"].ToString(), reader["NAME"].ToString() };
                data.Add(arr);
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return data;
        }

        public string GetOficerInfo(string regionId, string districtId)
        {
            string number = null;
            var cmd = new OracleCommand("RSTASK.PKG_ORGINFO.get_officer_info") { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("p_region_id", OracleDbType.Varchar2).Value = regionId;
            cmd.Parameters.Add("p_district_id", OracleDbType.Varchar2).Value = districtId;
            cmd.Parameters.Add("p_number", OracleDbType.Varchar2, 20).Direction = ParameterDirection.Output;
            string error;
            new OracleDb<PKG_ORG_INFO>().ExecuteNonQuery(cmd, out error, delegate (OracleParameterCollection reader)
            {
                number = reader["p_number"].Value.ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return number;
        }

    }
}