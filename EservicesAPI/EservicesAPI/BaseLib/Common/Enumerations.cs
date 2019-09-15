using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Common
{
    public enum DownloadType
    {
        ComplaintsOld = 1, //MUST REMOVE
        ApplicationDoc,
        AdditionalDoc,
        ApplicationAnswer = 4,
        ApplicationAnswer2 = 5,
        AppPrintVersion = 6,
        AppAttachment = 7,
        Representative,
        NotificationBlob,
        NotificationHtml,
        Notification = 11,
        Waybill,
        WaybillList,
        ApplicationDefects,
        Invoice,
        Decl21,
        CustomsDecl,
        rsGridExport_xls = 18,
        rsGridExport_xlsx = 19,
        rsGridExport_pdf = 20,
        rsGridExport_csv = 21,
        WaybillStock = 22,
        LoansReport = 23,
        InvoiceList = 24,
        WaybillOffline,
        ComplaintFile = 26,
        MedicamentList,
        InvoiceRequestList,
        PhytoVetDoc,
        PhytoVetRouteMap,
        ComplaintsReport,
        PhytoVetList,
        AppOldAnswer = 33,
        Questionnaires = 34,
        PvpUploadDoc = 35,
        PvpVetDoc = 36,
        PvpPaymentDoc = 37,
        AgsReport = 38,
        PvpPrint = 39,
        PvpActPrint = 40,
        InvoiceWaybillList = 41,
        RegistrationReport = 42,
        DownloadWaybillCar = 43,
        DownloadWaybillBarCode = 44,
        PvpPrintPublic,
        NotificationEmpLocal,
        ServiceRequestsNotSavedVersion = 45,
        ServiceRequestsPrintVersion = 46,
        ServiceRequestsSrAnswer = 47,
        ServiceRequestsSrAppxDoc = 48,
        ServiceRequestsSrDefect = 49,
        //DownloadInvoiceList = 50,
        //DownloadInvoiceRequestList = 51,
        ServicesPrintVersion = 53,
        ServicesSrAnswer = 54,
        ServicesSrAppxDoc = 55,
        ServicesSrDefect = 56,
        PvpOrder
    } 

    //public enum rsGrid
    //{
    //    ServiceRequest = 1,
    //    Waybills,
    //    WaybillGoods,
    //    WaybillGoodsEdit,
    //    WaybillsGoodsExport,
    //    SubWaybillGoodsEdit,
    //    SubWaybills,
    //    WaybillStartAddresses,
    //    WaybillEndAddresses,
    //    WaybillBuyerHist,
    //    WaybillDriverHist,
    //    WaybillStock,
    //    WaybillBarCodes,
    //    Notifications,
    //    ExciseList,
    //    Invoices,
    //    CancelInvoice,
    //    InvoicesTmp,
    //    InvoiceGoods,
    //    InvoiceGoodsEdit,
    //    InvoicesGoodsExport,
    //    InvoiceWaybills,
    //    InvoiceWaybillsEdit,
    //    InvoiceRequests,
    //    InvoiceRequestWaybills,
    //    InvoiceRequestWaybillsEdit,
    //    Medicaments,
    //    MedicamentsGoods,
    //    MedicamentsGoodsEdit,
    //    MedicamentsGoodsExport,
    //    PhytoVets,
    //    PhytoVetGoods,
    //    PhytoVetGoodsEdit,
    //    PhytoVetGoodsExport,
    //    OrgInfo,
    //    PvpGoodsExport,
    //    AgsLocks,
    //    PayerInfoDeclSalaries,
    //    OrgTaxRefundPersons,
    //    PayerInfoDecls,
    //    PayerInfoCashMachines,
    //    PayerInfoCashMachinesZ,
    //    PayerInfoCashMachinesOld,
    //    PayerInfoMapObjects,
    //    PayerInfoGambling,
    //    PayerInfoGamblingInv,
    //    PayerInfoCustoms,
    //    PayerInfoPenalties,
    //    PayerInfoAudit,
    //    PayerInfoCustomsTrucks,
    //    PayerInfoCustomsAutos,
    //    PayerInfoCustomsSpecs,
    //    PayerInfoCustomsEasy,
    //    PayerInfoCustomsPerson,
    //    PayerInfoStationaryObjects,
    //    PayerInfoTaxFreeInfo,
    //    PayerInfoTaxFreeList,
    //    TradeCenterRenters,
    //    ModPermits,
    //    MohPermits,
    //    EconomyPermits,
    //    ReceivedContainers
    //}

    public enum SummaryFunction
    {
        None = 0,
        Sum = 1,
        Average,
        Count
    }

    public enum DataType
    {
        tpString = 0,
        tpNumber = 1,
        tpDate = 2,
        tpFormatNumber = 3,
        tpObject = 4
    }

    public enum DateType
    {
        Day = 0,
        Month,
        Year,
        DateTime
    }

    public enum ReturnDay
    {
        Default = 0,
        First = 1,
        Last = 2
    }

    public enum FilterFunc
    {
        Equal = 0,
        Contains = 1,
        Begin = 2,
        InList = 3,
        InListContains = 4,
        Between = 5,
        Less = 6,
        NotContains = 7,
        Greater = 8,
        LessEqualMonth = 9,
        NotEqual = 10,
        InInterval = 11,
        MultiContains = 12
    }

    public enum PollModules
    {
        Waybill = 5,
        Notification = 10,
        WaybillNew = 98,
        Gez = 99,
        Invoice = 3,
        PayerInfo = 28,
        Ags = 7
    }

    public enum SessionState
    {
        FullAccess,
        ReadOnly,
        Disabled
    }

    public enum AuthenticationType
    {
        NoAuthenticate,
        Authenticate
    }

    public enum InputType
    {
        Text,
        Password,
        Number,
        Email
    }

    public enum Language
    {
        [Description("EN")]
        English = 0,
        [Description("ქა")]
        Georgia
    }
}