using BaseLib.Attributes;
using BaseLib.OraDataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.OraDatabase.DataSources
{
    [ViewName("RSWEB.V_GRD_SEARCH_TAX_PAYERS")]
    public class dsSearchTaxPayers: DataSourceProvider
    {
        public string MOD_BULI { get; set; }
        public string REG_SUB_TIP { get; set; }
        public string DASAXELEBA { get; set; }
        public string SAM_FORMA { get; set; }
        public string SAID_KODI { get; set; }
        [DateTimeFormat("mm-dd-yyyy")]
        public string AGRIC_AKVA { get; set; }
        [DateTimeFormat("dd-MMM-yyyy HH:mm")]
        public string ACT_START_DATE { get; set; }
        [DateTimeFormat("mm-dd-yyyy HH:mm")]
        public string MOVE_DATE { get; set; }
        public decimal UN_ID { get; set; }
    }
}
