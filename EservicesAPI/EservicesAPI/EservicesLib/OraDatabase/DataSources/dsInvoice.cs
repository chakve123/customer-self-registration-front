using System;
using System.Globalization;
using BaseLib.Attributes;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;

namespace EservicesLib.OraDatabase.DataSources
{
    #region Invoice

    public class dsInvoice : DataSourceProvider
    {
        public decimal ID { get; set; }

        public string INV_SERIE { get; set; }

        public string INV_NUMBER { get; set; }

        public decimal SEQNUM_SELLER { get; set; }

        public decimal SEQNUM_BUYER { get; set; }

        public string INV_CATEGORY_NAME { get; set; }

        public string INV_TYPE_NAME { get; set; }

        public double AMOUNT_FULL { get; set; }

        public double? AMOUNT_EXCISE { get; set; }

        private double? _AMOUNT_VAT;
        public double? AMOUNT_VAT
        {
            get
            {
                return _AMOUNT_VAT;
            }
            set
            {
                _AMOUNT_VAT = value.Value.ToString().ToNumber<double>();
            }
        }

        public string DOCMOSNOM_SELLER { get; set; }

        public string DOCMOSNOM_BUYER { get; set; }

        public string TRANS_START_ADDRESS { get; set; }

        public string TRANS_END_ADDRESS { get; set; }

        public string TRANS_START_ADDRESS_NO { get; set; }

        public string TRANS_END_ADDRESS_NO { get; set; }

        public string TRANS_NAME { get; set; }

        public string TRANS_COMPANY { get; set; }

        public string TRANS_DRIVER { get; set; }

        public string TRANS_CAR_MODEL { get; set; }

        public string TRANS_CAR_NO { get; set; }

        public string TRANS_TRAILER_NO { get; set; }

        public decimal? TRANS_COST { get; set; }

        public string TRANS_COST_PAYER { get; set; }

        public string INV_COMMENT { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string DELETE_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string CREATE_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string OPERATION_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string ACTIVATE_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string TRANS_START_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string CONFIRM_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string REFUSE_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string DELIVERY_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string REQUEST_CANCEL_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string AGREE_CANCEL_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string CORRECT_DATE { get; set; }
        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string CHANGE_DATE { get; set; }

        public decimal ACTION { get; set; }
        public decimal SELLER_ACTION { get; set; }
        public decimal BUYER_ACTION { get; set; }
        public string PARENT_INV_NUMBER { get; set; }
        public decimal CORRECT_REASON_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public decimal PREV_CORRECTION_ID { get; set; }
        public decimal NEXT_CORRECTION_ID { get; set; }
    }

    [ViewName("invoice.V_INVOICE_SELLER")]
    public class dsInvoiceSeller : dsInvoice
    {
        public decimal SUBUSER_ID_SELLER { get; set; }

        public decimal UNID_SELLER { get; set; }

        public string SELLER_ACTION_TXT { get; set; }

        public string BUYER { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string BUYER_VIEW_DATE { get; set; }

        //public int? CORRECT_REASON_ID { get; set; }
    }

    [ViewName("invoice.V_INVOICE_BUYER")]
    public class dsInvoiceBuyer : dsInvoice
    {
        public decimal SUBUSER_ID_BUYER { get; set; }

        public decimal UNID_BUYER { get; set; }

        public decimal BUYER_ACTION { get; set; }

        public string BUYER_ACTION_TXT { get; set; }

        public string SELLER { get; set; }
    }
    [ViewName("invoice.V_INVOICE_TRANSPORTER")]
    public class dsInvoiceTransporter : dsInvoice
    {
        public decimal TRANS_COMPANY_UNID { get; set; }
    }
    [ViewName("invoice.V_LIST_GOODS")]
    public class dsListGoods:DataSourceProvider {
        public string BARCODE { get; set; }
        public string GOODS_NAME { get; set; }
        public decimal UNID_SELLER { get; set; }
        public decimal UNID_BUYER { get; set; }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string ACTIVATE_DATE { get; set; }
    }

    #endregion

    #region Details

    [ViewName("invoice.V_INVOICE_GOODS")]
    public class dsInvoiceGoods : DataSourceProvider
    {
        public decimal ID { get; set; }

        public decimal INVOICE_ID { get; set; }

        public string GOODS_NAME { get; set; }

        public string BARCODE { get; set; }

        public string UNIT_TXT { get; set; }

        public double QUANTITY { get; set; }

        public double UNIT_PRICE { get; set; }

        public double AMOUNT { get; set; }


        private double _VAT_AMOUNT { get; set; }

        public double VAT_AMOUNT { get { return _VAT_AMOUNT; } set { _VAT_AMOUNT = value.ToString().ToNumber<double>(); } }

        public double EXCISE_AMOUNT { get; set; }

        public decimal EXCISE_ID { get; set; }

        public string VAT_TYPE_TXT { get; set; }
    }

    [ViewName("invoice.V_INVOICE_BARCODES")]
    public class dsInvoiceBarCodes : DataSourceProvider
    {
        public string BARCODE { get; set; }
        public string GOODS_NAME { get; set; }
        public decimal UN_ID { get; set; }
        public decimal UNIT_ID { get; set; }
        public string UNIT_TXT { get; set; }
        public decimal VAT_TYPE { get; set; }
        public string VAT_TYPE_TXT { get; set; }
        public decimal UNIT_PRICE { get; set; }
    }

    [ViewName("WAYBILL.V_GET_GOOD_EXCISE")]
    public class dsExcise : DataSourceProvider
    {
        public int ID { get; set; }

        public string PRODUCT_NAME { get; set; }  //  GOOD_NAME

        public decimal UNIT_ID { get; set; }

        public string UNIT_TEXT { get; set; }

        public long EXCISE_CODE { get; set; } // sakon_kodi

        public decimal EXCISE_RATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string END_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string EFFECT_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string EFFECT_DATE_MONTH { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string END_DATE_NVL { get; set; }
    }

    [ViewName("TP.V_SPEC_INVOICES_PRODUCTS")]
    public class dsOilProducts : DataSourceProvider
    {
        public decimal ID { get; set; }
        public string GOODS_NAME { get; set; }
        public string SSF_CODE { get; set; }
        public string SSN_CODE { get; set; }
    }

    [ViewName("invoice.V_INVOICE_OIL_DOCS")]
    public class dsInvoiceOilDoc : DataSourceProvider
    {
        public decimal ID { get; set; }

        public decimal INVOICE_ID { get; set; }

        public string DOC_SERIE { get; set; }

        public string DOC_NUMBER { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string DOC_DATE { get; set; }

        [DateTimeFormat("dd-MM-yyyy")]
        public string DOC_DATE_NAME { get; set; }

        public decimal DOC_CATEGORY { get; set; }

        public string DOC_CATEGORY_NAME { get; set; }

        public decimal DOC_TYPE { get; set; }

        public string DOC_TYPE_NAME { get; set; }
    }

    [ViewName("invoice.V_INVOICE_ADVANCE_ALL")]
    public class dsInvoiceAdvance : DataSourceProvider
    {
        public decimal ID { get; set; }

        public string INV_SERIE { get; set; }

        public string INV_NUMBER { get; set; }

        public decimal AMOUNT_MAX { get; set; }

        public decimal SEQNUM_SELLER { get; set; }

        public decimal SEQNUM_BUYER { get; set; }

        public decimal INV_CATEGORY { get; set; }

        public decimal INV_TYPE { get; set; }

        public decimal AMOUNT_FULL { get; set; }

        private decimal? _AMOUNT_VAT;
        public decimal? AMOUNT_VAT
        {
            get
            {
                return _AMOUNT_VAT;
            }
            set
            {
                _AMOUNT_VAT = value.Value.ToString().ToNumber<decimal>();
            }
        }

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string CREATE_DATE { get; set; }
        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string OPERATION_DATE { get; set; }
        //public string OPERATION_DATE_STR
        //{
        //    get
        //    {
        //        return OPERATION_DATE.ToString("dd-MMM-yyyy HH:mm:ss");
        //    }
        //    set { }
        //}

        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string ACTIVATE_DATE { get; set; }
        //public string ACTIVATE_DATE_STR
        //{
        //    get
        //    {
        //        return ACTIVATE_DATE.ToString("dd-MMM-yyyy HH:mm:ss");
        //    }
        //    set { }
        //}

        public decimal UNID_SELLER { get; set; }
        public decimal UNID_BUYER { get; set; }
        public string TIN_SELLER { get; set; }
        public string TIN_BUYER{ get; set; }
        public decimal SELLER_ACTION { get; set; }
        public decimal BUYER_ACTION { get; set; }

        //public new decimal AMOUNT_FULL { get; set; }
        //public new decimal AMOUNT_VAT { get; set; }

    }

    public class dsHistories : DataSourceProvider
    {
        public decimal USER_ID_SELLER { get; set; }
        public string TIN_SELLER { get; set; }

        public string TRANS_START_ADDRESS { get; set; }
        public string TRANS_END_ADDRESS { get; set; }

    }

    [ViewName("invoice.V_BUYER_HISTORY")]
    public class dsBuyerHistory : dsHistories
    {
        public string TIN_BUYER { get; set; }
        public decimal UNID_BUYER { get; set; }
        public string NAME_BUYER { get; set; }
        public string BUYER { get; set; }

        public decimal FOREIGN_BUYER { get; set; }

    }

    [ViewName("invoice.V_DRIVER_HISTORY")]
    public class dsDriverHistory : dsHistories
    {
        public string TRANS_DRIVER_TIN { get; set; }
        public string TRANS_DRIVER_NAME { get; set; }
        public string TRANS_DRIVER { get; set; }

        public decimal TRANS_DRIVER_FOREIGN { get; set; }
        public string TRANS_DRIVER_FOREIGN_ICON { get; set; }
        public string TRANS_CAR_NO { get; set; }
        public string TRANS_TRAILER_NO { get; set; }
        public string TRANS_CAR_MODEL { get; set; }

    }

    [ViewName("invoice.V_START_ADDRESS_HISTORY")]
    public class dsStartAddressHistory : dsHistories
    {
    }

    [ViewName("invoice.V_END_ADDRESS_HISTORY")]
    public class dsEndAddressHistory : dsHistories
    {
    }

    #endregion

    #region OrgObjects

    public class dsOrgObjects : DataSourceProvider
    {
        public string OBJ_CODE { get; set; }
        public string OB_IDENT_NO { get; set; }
        public string ADDRESS { get; set; }
        public decimal UN_ID { get; set; }
    }

    [ViewName("INVOICE.V_ORG_OBJ_ST_RETAIL")]
    public class dsOrgObjStRetail : dsOrgObjects
    {

    }

    [ViewName("INVOICE.V_ORG_OBJ_TR_INT")]
    public class dsOrgObjTrInt : dsOrgObjects
    {

    }

    [ViewName("INVOICE.V_ORG_OBJ_TR_ST_WHOLE")]
    public class dsOrgObjTrStWhole : dsOrgObjects
    {

    }

    [ViewName("INVOICE.V_ORG_OBJ_TR_END_RETAIL")]
    public class dsOrgObjTrEndRetail : dsOrgObjects
    {

    }

    [ViewName("INVOICE.V_ORG_OBJ_END_WHOLE")]
    public class dsOrgObjTrEndWhole : dsOrgObjects
    {

    }

    [ViewName("TP.V_SPEC_INVOICES_ORG_OBJ_IMP")]
    public class dsOrgObjectsImport : DataSourceProvider
    {
        public string OB_IDENT_NO { get; set; }
        public string ADDRESS { get; set; }
    }

    #endregion
}