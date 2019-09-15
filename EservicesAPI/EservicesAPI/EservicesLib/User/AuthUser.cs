using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EservicesLib.User
{
    [Serializable]
    public class AuthUser : BaseLib.User.AuthUser
    {
        public DateTime CreateDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ExpiresIn { get; set; }

        public bool IsPinCodeAuth
        {
            get { return !string.IsNullOrEmpty(ConfirmLogin); }
        }

        public string MaskedMobile { get; set; }

        public string AccessToken { get; set; }


        public string PinToken { get; set; }

        public string Screen { get; set; }

        public bool PermitForAnyRecord { get; set; }

        public string Vcode { get; set; }

        public string Check { get; set; }

        public int TaxUserID { get; set; }

        public int OperatorID { get; set; }

        public int SubUserID { get; set; }

        public string Tin { get; set; }

        public string IdentCode { get; set; }

        public string FullName { get; set; }

        public string SubUserName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public DateTime CodeExpire { get; set; }

        public int SecretWord { get; set; }

        public string Email { get; set; }

        public int UnID { get; set; }

        public int UserType { get; set; }
        public string SamForma
        {
            get
            {
                switch (SamFormaType)
                {
                    case SamformaType.Company: return "იურიდიული პირი";
                    case SamformaType.Industrialist: return "ინდ მეწარმე";
                    case SamformaType.Individual: return "ფიზიკური პირი";
                    default: return "";
                }
            }
        }

        public SamformaType SamFormaType
        {
            get
            {
                if (new[] { 28, 18, 19, 20, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 15, 21, 8, 22, 23, 24, 25, 26, 27, 29 }.Contains(SamFormaID))
                {
                    return SamformaType.Company;
                }
                else if (new[] { 1 }.Contains(SamFormaID))
                {
                    return SamformaType.Industrialist;
                }
                else // 14, 16, 17, 99
                    return SamformaType.Individual;
            }
        }

        public int SamFormaID { get; set; }

        public string SamFormaName { get; set; }

        public string InsCode { get; set; }

        public new static AuthUser CurrentUser { get; set; }

        public DateTime PassExpire { get; set; }

        public bool ShowOrgInfo { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsCanceled { get; set; }

        public string ConfirmLogin { get; set; }

        public string ConfirmChange { get; set; }

        public bool TwoStepAuth
        {
            get { return MobileVerification == 1; }
        }

        public int MobileVerification { get; set; }

        #region User Roles

        public int Role { get; set; }

        public bool IsEmployee { get; set; }

        private bool _isRemoteRegisterer;

        public bool IsRemoteRegisterer
        {
            get
            {
                _isRemoteRegisterer = (UserType == (int)User.UserType.Employee ||
                                       UserType == (int)User.UserType.RegistratorNapr ||
                                       UserType == (int)User.UserType.Daisy ||
                                       UserType == (int)User.UserType.CashUnReg ||
                                       UserType == (int)User.UserType.KasaGe ||
                                       UserType == (int)User.UserType.InterModule ||
                                       UserType == (int)User.UserType.InvestGroup) &&
                                      !HasPermission(User.Modules.AccountingDecl);

                return _isRemoteRegisterer;
            }
        }

        public bool IsVisibleApp { get; set; }

        public bool IsMedViewer { get; set; }

        public bool IsMedFullControl { get; set; }

        public bool IsWarehouseEmp { get; set; }

        public bool IsPayer
        {
            get { return UnID > 0; }
        }

        public bool IsVatPayer { get; set; }

        #endregion

        public int UnIDRequestor { get; set; }

        public string RequestorPersonID { get; set; }

        public string RequestorPersonIDCommon { get; set; }

        public int ScreenWidth { get; set; }

        public int IsConfirmed { get; set; }
        
        public string GetUnIdList
        {
            get
            {
                string unIds = string.Empty;

                if (UnionOrgs != null)
                {
                    foreach (var unionOrg in UnionOrgs.FindAll(f => f.IsAvtive))
                    {
                        if (string.IsNullOrEmpty(unIds))
                            unIds = unionOrg.UnID.ToString();
                        else
                            unIds += "," + unionOrg.UnID;
                    }
                }

                return unIds;
            }
        }

        #region User Modules

        public List<Module> Modules = new List<Module>();

        private DateTime? _notifNextFetch;
        private DateTime? NotifNextFetch
        {
            get
            {
                if (_notifNextFetch == null)
                    _notifNextFetch = DateTime.Now;
                return _notifNextFetch;
            }
            set { _notifNextFetch = value; }
        }

        public bool ModulesInitialised { get; set; }

        public List<ModuleProperties> GetMainPageModules(string pageUrl)
        {
            List<ModuleProperties> modProps = Modules.Select(m => m.GetProperties(pageUrl)).ToList().FindAll(mp => !string.IsNullOrEmpty(mp.Url) && (mp.Pinned || mp.Notifications.Count > 0));
            modProps.Sort(delegate (ModuleProperties x, ModuleProperties y)
            {
                if (x.Pinned)
                    if (y.Pinned)
                        if (x.PinDate > y.PinDate)
                            return 1;
                        else if (x.PinDate < y.PinDate)
                            return -1;
                        else return 0;
                    else
                        return -1;
                if (y.Pinned)
                    return 1;
                if (x.ID > y.ID)
                    return -1;
                if (x.ID < y.ID)
                    return 1;
                return 0;

            });
            ModulesInitialised = true;
            return modProps;
        }

        public void GetNotificaitons(bool forceFetch = false)
        {
            if (NotifNextFetch < DateTime.Now) //Two minutes passed after last fetch
            {
                NotifNextFetch = DateTime.Now.AddMinutes(5);
                var notifications = (from m in Modules where !string.IsNullOrEmpty(m.NotificationProcedure) select new ModuleNotificationsList(m.NotificationProcedure)).ToList();
                int unID = CurrentUser.UnID;
                int subUserID = CurrentUser.SubUserID;

                Parallel.ForEach(notifications, currentNotifs => currentNotifs.GetNotificationList(unID, subUserID));

                foreach (ModuleNotificationsList notifs in notifications)
                {
                    if (Modules.Exists(m => m.NotificationProcedure == notifs.ProcedureName))
                        Modules.Find(m => m.NotificationProcedure == notifs.ProcedureName).SetNotifications(notifs.Notifications);
                }
            }
            else
            {
                if (forceFetch) //Two minutes didn't pass, force fetch (ONLY MANDATORY)
                {
                    // NotifLastFetch = DateTime.Now;
                    var notifications = (from m in Modules where !string.IsNullOrEmpty(m.NotificationProcedure) && m.ModuleType == ModuleType.WithMandatory select new ModuleNotificationsList(m.NotificationProcedure)).ToList();
                    int unID = CurrentUser.UnID;
                    int subUserID = CurrentUser.SubUserID;

                    Parallel.ForEach(notifications, currentNotifs => currentNotifs.GetNotificationList(unID, subUserID));

                    foreach (ModuleNotificationsList notifs in notifications)
                    {
                        if (Modules.Exists(m => m.NotificationProcedure == notifs.ProcedureName))
                            Modules.Find(m => m.NotificationProcedure == notifs.ProcedureName).SetNotifications(notifs.Notifications);
                    }
                }
            }
        }

        public void GetLoginNotificaitons()
        {
            var notifications = (from m in Modules where !string.IsNullOrEmpty(m.NotificationProcedureLogin) select new ModuleNotificationsList(m.NotificationProcedureLogin)).ToList();
            int unID = CurrentUser.UnID;
            int subUserID = CurrentUser.SubUserID;

            Parallel.ForEach(notifications, currentNotifs => currentNotifs.GetNotificationList(unID, subUserID));

            foreach (ModuleNotificationsList notifs in notifications)
            {
                if (Modules.Exists(m => m.NotificationProcedureLogin == notifs.ProcedureName))
                    Modules.Find(m => m.NotificationProcedureLogin == notifs.ProcedureName).SetLoginNotifications(notifs.Notifications);
            }
        }

        public void SetNotificaitonsViewed()
        {
            foreach (Module m in Modules)
                foreach (ModuleNotificaiton n in m.Notifications)
                    n.Viewed = true;
        }

        public string GetMandatoryNotificationURL()
        {
            foreach (Module m in Modules)
                if (m.HasMandatoryNotifications())
                    return m.Url;
            return "";

        }

        public bool HasPermission(Modules module, Permissions permission = Permissions.Read)
        {
            return Modules.Exists(m => m.ID == (int)module) && Modules.Find(m => m.ID == (int)module).HasPermission(permission);
        }

        #endregion

        public static bool AuthenticateUser(AuthUser currUser)
        {
            if (currUser != null && !currUser.IsCanceled)
            {
                CurrentUser = currUser;

                var ba = new BitArray(BitConverter.GetBytes(currUser.Role));
                if (currUser.UserType == (int)User.UserType.CommonUser) // Common User
                    currUser.IsMedViewer = ba[7];
                else if (currUser.UserType == (int)User.UserType.Employee)  // RS Emp
                    return false;
                else if (currUser.UserType == (int)User.UserType.MedicalAgency)
                    currUser.IsMedViewer = true;

                return true;
            }
            return false;
        }

        public static int UsertypeToPermission()
        {
            if (!Authenticated) return 1;

            switch ((UserType)CurrentUser.UserType)
            {
                case User.UserType.CommonUser: return 1;
                case User.UserType.SubUser: return 1;
                case User.UserType.Employee: return 2;
                case User.UserType.RegistratorNapr: return 4;
                case User.UserType.Daisy: return 16;
                case User.UserType.CashUnReg: return 32;
                case User.UserType.KasaGe: return 64;
                case User.UserType.InterModule: return 128;
                case User.UserType.InvestGroup: return 256;
                default: return 1;
            }
        }

        public List<Person> Persons { get; set; }

        public List<TrustingUser> TrustingUsers = new List<TrustingUser>();

        public Person GetActivePerson
        {
            get { return Authenticated && CurrentUser.IsRemoteRegisterer && Persons != null && Persons.Exists(p => p.Active) ? Persons.Find(p => p.Active) : null; }
        }

        public int? GetActivePersonUnID
        {
            get { return Authenticated && CurrentUser.IsRemoteRegisterer && Persons != null && Persons.Exists(p => p.Active) ? (int?)Persons.Find(p => p.Active).UnID : null; }
        }

        public List<string> Representatives { get; set; }

        //private ActiveUser _activeUser;

        //public ActiveUser ActiveUser
        //{
        //    get
        //    {
        //        if (_activeUser == null) _activeUser = new ActiveUser();
        //        return _activeUser;
        //    }
        //    set { _activeUser = value; }
        //}

        public LoginNotification LoginNotification;
    }

    [Serializable]
    public class LoginNotification
    {
        public string NotificationText { get; set; }
        public bool Seen { get; set; }

    }

    public class DeviceInfo
    {
        public string device_code { get; set; }

        public string address { get; set; }

        public string browser { get; set; }

        public string oper_system { get; set; }
    }

    public class AuthInfo
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string PinToken { get; set; }

        public string ConfirmLogin { get; set; }
        
        public bool IsPinCodeAuth
        {
            get { return !string.IsNullOrEmpty(ConfirmLogin); }
        }
         
        public bool IsCanceled { get; set; }

        public string MaskedMobile { get; set; }

        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public int UserID { get; set; }

        public int SubUserID { get; set; }

        public int UnID { get; set; }

        public string Tin { get; set; }

        public int UserType { get; set; }

        public bool IsVatPayer { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
    }

    //[Serializable]
    //public class ActiveUser
    //{
    //    public int? UnID
    //    {
    //        get
    //        {
    //            if (AuthUser.CurrentUser.UserType == (int)UserType.Employee)
    //            {
    //                return AuthUser.CurrentUser.IsRemoteRegisterer && AuthUser.CurrentUser.Persons != null && AuthUser.CurrentUser.Persons.Exists(p => p.Active) ? (int?)AuthUser.CurrentUser.Persons.Find(p => p.Active).UnID : null;
    //            }
    //            return AuthUser.CurrentUser.UnID;
    //        }
    //    }

    //    public string Tin
    //    {
    //        get
    //        {
    //            if (AuthUser.CurrentUser.UserType == (int)UserType.Employee)
    //            {
    //                return AuthUser.CurrentUser.IsRemoteRegisterer && AuthUser.CurrentUser.Persons != null && AuthUser.CurrentUser.Persons.Exists(p => p.Active) ? AuthUser.CurrentUser.Persons.Find(p => p.Active).PersonID : null;
    //            }
    //            return AuthUser.CurrentUser.Tin;
    //        }
    //    }
    //}

    public enum UserType
    {
        CommonUser = 0,
        Employee = 2,
        RegistratorNapr = 3,
        MedicalAgency = 4,
        SubUser = 5,
        Daisy = 6,
        CashUnReg = 7,
        RailWay = 8,
        ParcelUser = 9,
        FoodAgensy = 10,
        KasaGe = 11,
        InterModule = 12,
        InvestGroup = 13
    }

    public enum SamformaType
    {
        Company = 0,
        Industrialist,
        Individual
    }

    public enum AuthType
    {
        UserName,
        Tin
    }
}
