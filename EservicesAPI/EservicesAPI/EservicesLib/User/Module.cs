using System;
using System.Collections.Generic;
using BaseLib.ExtensionMethods;

namespace EservicesLib.User
{
    [Serializable]
    public class Module
    {
        public ModuleType ModuleType
        {
            get
            {
                return Notifications.Exists(n => n.Mandatory) ? ModuleType.WithMandatory : ModuleType.WithoutMandatory;
            }
        }

        public int ID { get; set; }

        public string Name
        {
            get
            {
                // return AuthUser.CurrLanguage == Language.English ? NameEn : NameGeo;
                return null;
            }
        }

        public string NameEn { get; set; }
        public string NameGeo { get; set; }

        public string GroupName
        {
            get
            {
                //return StaticData.MultiLanguage.TranslateText(_groupNameMultiLang);
                return _groupNameMultiLang;
            }
            set { _groupNameMultiLang = value; }
        }
        public string _groupNameMultiLang;

        public string Url { get; set; }

        public long Permission { get; set; }

        public string Color { get; set; }

        public bool Pinned { get; set; }

        public DateTime? PinDate { get; set; }

        public string Description { get; set; }

        public int ProtocolId { get; set; }

        public int ExternalService { get; set; }

        public string NotificationProcedure { get; set; }

        public string NotificationProcedureLogin { get; set; }

        private List<ModuleNotificaiton> _notifications;

        public List<ModuleNotificaiton> Notifications
        {
            get
            {
                if (_notifications == null)
                    return new List<ModuleNotificaiton>();
                return _notifications;
            }
            set
            {
                _notifications = value;
            }
        }

        private List<ModuleNotificaiton> _loginNotifications;

        public List<ModuleNotificaiton> LoginNotifications
        {
            get
            {
                if (_loginNotifications == null)
                    return new List<ModuleNotificaiton>();
                return _loginNotifications;
            }
            set
            {
                _loginNotifications = value;
            }
        }

        public Module()
        {
        }

        public Module(int id, string name, string nameEn, string groupName, string url, long permission, string color, int pinned, string description, string notificationProcedure, string notificationProcedureLogin, int externalService, DateTime? pinDate = null)
        {
            ID = id;
            NameGeo = name;
            NameEn = nameEn;
            GroupName = groupName;
            Url = url;
            Permission = permission;
            Color = color;
            Pinned = pinned == 1;
            NotificationProcedure = notificationProcedure;
            NotificationProcedureLogin = notificationProcedureLogin;
            PinDate = Pinned ? pinDate : null;
            Description = description;
            ExternalService = externalService;
        }

        public void SetColor(string color)
        {
            //Color = DataProviderManager<PKG_MENU>.Provider.set_module_color(ID, color);
        }

        public void SetPin()
        {
            //DataProviderManager<PKG_MENU>.Provider.set_module_pin(this);
        }

        public bool HasPermission(Permissions p)
        {
            int perm = 1 << (int)p;
            return (perm & this.Permission) != 0;
        }

        public ModuleProperties GetProperties(string pageUrl = "")
        {
            return new ModuleProperties(this, pageUrl);
        }

        public void GetNotifications(ModuleNotifType type = ModuleNotifType.All)
        {
            // if (!string.IsNullOrEmpty(NotificationProcedure))
            // Notifications = DataProviderManager<PKG_MENU>.Provider.get_module_notifications(this, type);
        }

        public void SetNotifications(List<ModuleNotificaiton> notifs)
        {
            foreach (ModuleNotificaiton m in notifs)
            {
                if (Notifications.Exists(n => n.Name == m.Name && n.Viewed && n.Value.ToNumber<double>() >= m.Value.ToNumber<double>()) || !AuthUser.CurrentUser.ModulesInitialised)
                    m.Viewed = true;
            }
            Notifications = notifs;
        }

        public void GetLoginNotifications()
        {
            //if (!string.IsNullOrEmpty(NotificationProcedureLogin))
            //  LoginNotifications = DataProviderManager<PKG_MENU>.Provider.get_module_notifications(this, ModuleNotifType.All, true);
        }

        public void SetLoginNotifications(List<ModuleNotificaiton> notifs)
        {
            foreach (ModuleNotificaiton m in notifs)
            {
                if (LoginNotifications.Exists(n => n.Name == m.Name && n.Viewed && n.Value.ToNumber<double>() >= m.Value.ToNumber<double>()))
                    m.Viewed = true;
            }
            LoginNotifications = notifs;
        }

        public bool HasMandatoryNotifications()
        {
            return Notifications.Exists(n => n.Mandatory) || LoginNotifications.Exists(l => l.Mandatory);
        }

        public ModuleNotificaiton GetMandatoryNotification()
        {
            if (LoginNotifications.Exists(n => n.Mandatory))
                return LoginNotifications.Find(n => n.Mandatory);
            if (Notifications.Exists(n => n.Mandatory))
                return Notifications.Find(n => n.Mandatory);
            return null;
        }
    }

    [Serializable]
    public class ModuleProperties
    {
        public ModuleProperties(Module module, string pageUrl = "")
        {
            ID = module.ID;
            Name = module.Name;
            GroupName = module.GroupName;
            Url = module.Url;
            Color = module.Color;
            Pinned = module.Pinned;
            Notifications = new List<ModuleNotificaiton>();
            if (pageUrl.ToLower() == module.Url.ToLower())
            {
                Notifications.AddRange(module.Notifications);
                Notifications.AddRange(module.LoginNotifications);
            }
            else
            {
                Notifications.AddRange(module.Notifications.FindAll(n => n.Type != ModuleNotifType.OnModule));
                Notifications.AddRange(module.LoginNotifications.FindAll(n => n.Type != ModuleNotifType.OnModule));
            }
            PinDate = module.PinDate;
            Description = module.Description;
            ProtocolId = module.ProtocolId;
            ExternalService = module.ExternalService;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public string Url { get; set; }

        public string Color { get; set; }

        public bool Pinned { get; set; }

        public string Description { get; set; }

        public int ProtocolId { get; set; }

        public List<ModuleNotificaiton> Notifications { get; set; }

        public DateTime? PinDate { get; set; }
        public int ExternalService { get; set; }
    }

    [Serializable]
    public class ModuleNotificaiton
    {
        public ModuleNotificaiton(string name, string text, string textEn, string value, bool onlyOnLogin, ModuleNotifType type = ModuleNotifType.All)
        {
            Name = name;
            TextGeo = text;
            TextEn = textEn;
            Value = value;
            Viewed = false;
            Type = type;
            OnlyOnLogin = onlyOnLogin;
        }

        public string Name { get; set; }

        public string TextGeo { get; set; }

        public string TextEn { get; set; }

        public string Script { get; set; }

        public string Value { get; set; }

        public bool Viewed { get; set; }

        public bool Mandatory { get; set; }

        public bool OnlyOnLogin { get; set; }

        public ModuleNotifType Type { get; set; }
    }

    [Serializable]
    public class ModuleNotificationsList
    {
        public ModuleNotificationsList(string procName)
        {
            ProcedureName = procName;
            Notifications = new List<ModuleNotificaiton>();
        }

        public string ProcedureName { get; set; }
        public List<ModuleNotificaiton> Notifications { get; set; }

        public void GetNotificationList(int UnID, int SubUserID, ModuleNotifType type = ModuleNotifType.OnMain)
        {
            //if (!string.IsNullOrEmpty(ProcedureName))
            //Notifications = DataProviderManager<PKG_MENU>.Provider.get_notifications(ProcedureName, UnID, SubUserID, type);
        }
    }

    public enum Modules
    {
        DeclarationOld = -1,
        None = 0,
        Declaration = 1,
        Accounting = 2,
        InvoiceOld = 3,
        Nsap = 4,
        Waybill = 5,
        Pharmac = 6,
        Ags = 7,
        Pay = 8,
        Services = 9,
        Notification = 10,
        Complaint = 11,
        PayParcel = 12,
        Agent = 13,
        Warehouse = 14,
        ManageRepresentative = 15,
        UserPermissions = 16,
        UserParameters = 17,
        TaxCalendar = 18,
        Balance = 19,
        ParcelServices = 20,
        DutyFree = 21,
        Loans = 22,
        OrgInfo = 23,
        Decl21 = 24,
        CustomsDecl = 25,
        UserNotifications = 26,
        RailWayNewRSPopup = 27,
        PayerInfo = 28,
        Phytovet = 29,
        Terminal = 30,
        PhytoVetNotif = 32,
        EJouranl = 33,
        T1Form = 34,
        HumanitaryLoad = 35,
        Form222 = 36,
        SpecInvoice = 37,
        RailwayDoc = 38,
        Fatca = 39,
        Inventory = 40,
        ReceiptEquivalent = 45,
        TerminalLock = 46,
        CustomsWarehouseExit = 47,
        AuditNotification = 48,
        InfoPermit = 50,
        EJournalTiz = 51,
        GoodsCustomDec = 52,
        AccountingDecl = 53,
        Communicator = 55,
        MoeServices = 63,
        MoaServices = 64,
        MoeDeclaration = 65,
        MoeStat = 66,
        MoeAccounting = 67,
        MoeLicense = 70,
        Invoice = 72,
        MinstrtyOfEconomy = 76,
        ExternalData = 77,
        TaxFree = 79,
        WareHouseBalances = 80,
        ShipsJournal = 81
    }

    public enum Permissions
    {
        None = -1,
        Read = 0,
        CreateEdit,
        Delete,
        Send,
        Activate,
        Close,
        Confirm,
        CloseSubWaybill,
        CreateInvoice,
        PermitForAnyRecord,
        Correct,
        Cancel,
        AttachToDecl,
        CreateInvoiceRequest,
        Executor,
        Administration,
        TermAuto,
        TermContainer,
        PayerInfoDec,
        PayerInfoCashMachines,
        PayerInfoMapObjects,
        PayerInfoPermits,
        PayerInfoPenalties,
        PayerInfoAudit,
        PayerInfoCustoms,
        PayerInfoStatObjects,
        PayerInfoTaxFree,
        PayerInfoSalaries,
        PayerInfoTurnover,
        PayerInfoRoadCards
    }

    public enum ModuleNotifType
    {
        None = 0,
        OnModule = 1,
        OnMain = 2,
        All = 3
    }

    public enum ModuleType
    {
        WithoutMandatory = 0,
        WithMandatory = 1
    }
}
