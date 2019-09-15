
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.User;
using EservicesLib.User;
using Oracle.DataAccess.Client;
using AuthUser = EservicesLib.User.AuthUser;
using Oracle.DataAccess.Types;

namespace EservicesLib.OraDatabase.StoredProcedures
{
    public class PKG_NOTIFY : DataProvider
    {
        //public string get_notif_details(int p_un_id, int p_notif_id, int isEmp, int notIdM)
        //{
        //    var cmd = new OracleCommand("rstask.pkg_notify.get_notif_details");
        //    cmd.Parameters.Add("p_un_id", OracleDbType.Int32).Value = p_un_id;
        //    cmd.Parameters.Add("p_is_emp", OracleDbType.Int32).Value = isEmp;
        //    cmd.Parameters.Add("p_notification_id", OracleDbType.Int32).Value = p_notif_id;
        //    cmd.Parameters.Add("p_id_m", OracleDbType.Int32).Value = notIdM;
        //    cmd.Parameters.Add("p_notif_curs", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.Parameters.Add("p_add_docs_curs", OracleDbType.RefCursor, ParameterDirection.Output);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    string result = "";
        //    string error;
        //    new OracleDb<PKG_NOTIFY>().ProcessEachMultisetRow(cmd, out error, delegate (OracleDataReader reader, int index)
        //    {
        //        switch (index)
        //        {
        //            case 0:
        //                {
        //                    notification.NotifID = reader["notification_id"].ToString().ToNumber<int>();
        //                    notification.Subject = reader["notification_subject"].ToString();
        //                    notification.Text = reader["notification_text"].ToString();
        //                    notification.NotifDate = reader["notification_date"].ToString();
        //                    notification.Active = reader["active"].ToString();
        //                    notification.NotifType = reader["notification_type"].ToString();
        //                    notification.TypeID = reader["type_id"].ToString().ToNumber<int>();
        //                    notification.ReadDate = reader["read_date"].ToString();
        //                    notification.Status = reader["status"].ToString();
        //                    notification.ID = reader["id"].ToString().ToNumber<int>();
        //                    notification.DocName = reader["DOC_NAME"].ToString();
        //                    notification.DocNameM = reader["DOC_NAME_m"].ToString();
        //                    notification.Type = reader["type"].ToString().ToNumber<int>();
        //                    notification.NotifIDM = reader["notification_id_m"].ToString().ToNumber<int>();
        //                    if (isEmp == 1)
        //                        notification.ReadDays = 0;
        //                    else
        //                        notification.ReadDays = (DateTime.Now - DateTime.Parse(notification.ReadDate)).Days;
        //                    notification.CountRpt1 = reader["count_rpt1"].ToString().ToNumber<int>();
        //                    notification.CountRpt2 = reader["count_rpt2"].ToString().ToNumber<int>();
        //                    notification.EmpName = reader["emp_name"].ToString();
        //                    notification.RepName = reader["rep_name"].ToString();
        //                    notification.RepTin = reader["rep_no"].ToString();
        //                    break;
        //                }
        //            case 1:
        //                {
        //                    notification.AddDocs.Add(new NotificationAddDoc
        //                    {
        //                        ID = reader["id"].ToString().ToNumber<int>(),
        //                        DocName = reader["filename"].ToString(),
        //                        Description = reader["description"].ToString()
        //                    });
        //                    break;
        //                }
        //        }
        //    });

        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        throw new Exception(error);
        //    }
        //    return notification;
        //}
    }
}