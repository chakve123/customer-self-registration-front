using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Attributes;
using BaseLib.OraDataBase;

namespace EservicesLib.OraDatabase.DataSources
{
    [ViewName("RSTASK.V_GRD_NOTIFICATIONS_N")]
    public class dsNotifications : DataSourceProvider
    {
        public decimal? NOTIF_ID { get; set; }

        public decimal UN_ID { get; set; }

        public string NOTIF_SUBJ { get; set; }

        [DateTimeFormat("dd-MMM-yyyy HH:mm:ss")]
        public string NOTIF_DATE { get; set; }

        public string ACTIVE { get; set; }

        public string NOTIF_TYPE { get; set; }

        [DateTimeFormat("dd-MMM-yyyy HH:mm:ss")]
        public string READ_DATE { get; set; }

        public string STATUS { get; set; }

        public decimal TYPE { get; set; }

        public string NOTIFY_ID { get; set; }

        public decimal NOTIF_ID_M { get; set; }

        public string NOTIF_DOC_M { get; set; }

        public string NOTIF_TEXT { get; set; }
    }

    [ViewName("RSTASK.v_grd_notif_rpt_1")]
    public class dsNotificationRpt1 : DataSourceProvider
    {
        public string SERIA { get; set; }

        public string NOMERI { get; set; }

        public decimal AMOUNT { get; set; }

        public string PERIODI { get; set; }

        public decimal NOTIFICATION_ID { get; set; }
    }

    [ViewName("RSTASK.v_grd_notif_rpt_2")]
    public class dsNotificationRpt2 : DataSourceProvider
    {
        public string GAD_NAME { get; set; }

        public decimal SALDO { get; set; }

        public string SAXAZCODES { get; set; }

        public decimal SAURAVI { get; set; }

        public decimal ZEDMETOBA { get; set; }

        public decimal NOTIFICATION_ID { get; set; }
    }
}
