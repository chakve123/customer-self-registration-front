using BaseLib.Common;
using BaseLib.OraDataBase;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Data;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_INVOICE : DataProvider
    {
        public string save_invoices(string json, int userID, int subUserID, int action)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.SAVE_INVOICES");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = json;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_action", OracleDbType.Int32).Value = action;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";

            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error);

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public string get_seq_num(string operationPeriod, int unID, int userID)
        {
            var cmd = new OracleCommand("invoice.PKG_DECL.get_seq_nums");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_operation_period", OracleDbType.Varchar2).Value = operationPeriod;
            cmd.Parameters.Add("p_seq_nums", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.CommandType = CommandType.StoredProcedure;
            string error;
            var declNum = string.Empty;
            new OracleDb<PKG_INVOICE>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                if (string.IsNullOrEmpty(declNum)) declNum = reader[0].ToString();
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return declNum;
        }

        public string create_decl(int unID, int userID, int subUserID, string json)
        {
            var cmd = new OracleCommand("invoice.PKG_DECL.create_decl");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = json;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";

            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error);

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public string get_org_obj_address(string obIdentNo)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.GET_ORG_OBJ_ADDRESS");
            cmd.Parameters.Add("p_ob_ident_no", OracleDbType.Varchar2).Value = obIdentNo;
            cmd.Parameters.Add("p_res", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            var address = string.Empty;
            string error;
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                if (cmd.Parameters["p_res"].Value.ToString() != "null") address = cmd.Parameters["p_res"].Value.ToString();
                else address = string.Empty;
            });
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            return address;
        }

        public string GetInvoice(int InvoiceID, int InvoiceNumber, int userID, int subUserID, int parentInvoiceID = 0)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.get_invoice");
            cmd.Parameters.Add("p_invoice_id", OracleDbType.Int32).Value = InvoiceID;
            cmd.Parameters.Add("p_invoice_number", OracleDbType.Int32).Value = InvoiceNumber;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_parent_invoice_id", OracleDbType.Int32).Value = parentInvoiceID;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error); 

            return result;
        }
        public string GetInvoicesWithGoods(int userID, int subUserID, string TIN_SELLER, string TIN_BUYER, string ACTIVATE_DATE, int START_ROW_INDEX, int COUNT)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.get_invoices");
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_tin_seller", OracleDbType.Varchar2).Value = TIN_SELLER;
            cmd.Parameters.Add("p_tin_buyer", OracleDbType.Varchar2).Value = TIN_BUYER;
            cmd.Parameters.Add("p_activate_date", OracleDbType.Varchar2).Value = ACTIVATE_DATE;
            cmd.Parameters.Add("p_start_row_index", OracleDbType.Int32).Value = START_ROW_INDEX;
            cmd.Parameters.Add("p_count", OracleDbType.Int32).Value = COUNT;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }
        public string GetBarcode(int unID, string barcode)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.get_barcode");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_bar_code", OracleDbType.Varchar2).Value = barcode;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error); 

            return result;
        }

        public string GetNotificationsJson(int unID,int subUserID)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.GET_NOTIFICATIONS_JSON");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            cmd.Parameters.Add("p_sub_user", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error); 

            return result;
        }

        public string SaveInvoice(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.save_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            }); 

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }
        public string DeleteInvoice (int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.delete_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }
        public string ActivateInvoice(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.activate_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public string SaveInvoiceStatus(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.save_invoice_status");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public string CancelInvoice(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.cancel_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }


        public string ConfirmInvoice(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.confirm_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }


        public string RefuseInvoice(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.refuse_invoice");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }
        public string SaveInvoiceTemplate(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.save_invoice_template");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public string DeleteInvoiceTemplate(int userID, int subUserID, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.delete_invoice_template");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.Parameters.Add("p_subuser_id", OracleDbType.Int32).Value = subUserID;
            cmd.Parameters.Add("p_result", OracleDbType.Clob).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_client_ip", OracleDbType.Varchar2).Value = CommonFunctions.GetRemoteAddress;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                //result = ((OracleClob)(cmd.Parameters["p_result"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public void StartOilTransport(decimal invoiceID, int userID)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.START_OIL_TRANSPORT");
            cmd.Parameters.Add("p_invoice_id", OracleDbType.Int32).Value = invoiceID;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = userID;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }

        public string SaveBarcodes(int p_un_id, string InvoiceJson)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.save_barcodes");
            cmd.Parameters.Add("p_json", OracleDbType.Clob).Value = InvoiceJson;
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = p_un_id;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                //result = ((OracleClob)(cmd.Parameters["output"]).Value).Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }



        public string GetNameByTin(string Tin)
        {
            var cmd = new OracleCommand("invoice.PKG_INVOICE.get_name_by_tin");
            cmd.Parameters.Add("p_tin", OracleDbType.Varchar2,1000,Tin,ParameterDirection.Input);
            cmd.Parameters.Add("p_result", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            string result = "";
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                //result = ((Oracle.DataAccess.Types.OracleString)reader["result"].Value).IsNull ? string.Empty : reader["result"].Value.ToString();
                result = ((Oracle.DataAccess.Types.OracleString)cmd.Parameters["p_result"].Value).IsNull ? "" : cmd.Parameters["p_result"].Value.ToString();
                //count = cmd.Parameters["p_cached_counter"].Value.ToString().ToNumber<int>();
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public byte[] GetInvoiceReport(string id,int unID)
        {
            var cmd = new OracleCommand("plpdf.REPORTS_INVOICE_TAXDOC.RPT_INVOICE");
            cmd.Parameters.Add("p_invoice_id_list", OracleDbType.Clob,ParameterDirection.Input).Value = id;
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32,ParameterDirection.Input).Value = unID;
            cmd.Parameters.Add("rep_out", OracleDbType.Blob).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            //return cmd;
            byte[] result = new byte[0];
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
                result = ((OracleBlob)(cmd.Parameters["rep_out"].Value)).Value;
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);

            return result;
        }

        public DataTable download_barcode(int unID)
        {
            var dt = new DataTable();
            dt.Columns.Add("barcode");
            dt.Columns.Add("goods_name");
            dt.Columns.Add("other_unit_txt");
            dt.Columns.Add("unit_txt");

            var Cmd = new OracleCommand("INVOICE.PKG_INVOICE.download_barcodes");
            Cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = unID;
            Cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            Cmd.CommandType = CommandType.StoredProcedure;

            var error = String.Empty;
            new OracleDb<PKG_INVOICE>().ProcessEachRow(Cmd, out error, delegate (OracleDataReader reader)
            {
                dt.Rows.Add(new object[] { reader["barcode"], reader["goods_name"], reader["other_unit_txt"], reader["unit_txt"] });
            });

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return dt;
        }

        public void ClearBarCodes(int unID)
        {
            var cmd = new OracleCommand("INVOICE.PKG_INVOICE.CLEAR_BARCODES");
            cmd.Parameters.Add("p_un_id", OracleDbType.Int32, ParameterDirection.Input).Value = unID;
            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            //return cmd;
            int response = 0;
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }
        public void log_api_debug(string json, string p_error)
        {
            var cmd = new OracleCommand("INVOICE.PKG_INVOICE.log_api_debug");
            cmd.Parameters.Add("p_json", OracleDbType.Clob, ParameterDirection.Input).Value = json;
            cmd.Parameters.Add("p_error", OracleDbType.Clob, ParameterDirection.Input).Value = p_error;

            cmd.CommandType = CommandType.StoredProcedure;

            string error;
            //return cmd;
            int response = 0;
            new OracleDb<PKG_INVOICE>().ExecuteNonQuery(cmd, out error, delegate
            {
            });

            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
        }
    }
}