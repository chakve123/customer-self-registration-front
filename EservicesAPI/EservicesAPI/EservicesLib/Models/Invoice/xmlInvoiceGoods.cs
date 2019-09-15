using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.Models.Invoice
{
    public class xmlInvoiceGoods
    {
        public decimal ID { get; set; }

        public decimal INVOICE_ID { get; set; }

        public string GOODS_NAME { get; set; }

        public string BARCODE { get; set; }

        public decimal UNIT_ID { get; set; }
        public string UNIT_TXT { get; set; }

        public double QUANTITY { get; set; }

        public double UNIT_PRICE { get; set; }

        public double AMOUNT { get; set; }

        public double VAT_AMOUNT { get; set; }

        public double EXCISE_AMOUNT { get; set; }

        public decimal EXCISE_ID { get; set; }

        public decimal VAT_TYPE { get; set; }
        public string VAT_TYPE_TXT { get; set; }
        public decimal LOCAL_ID { get; set; } // this does nothing here but gives a property which is essential for goods in angular 
    }

}
