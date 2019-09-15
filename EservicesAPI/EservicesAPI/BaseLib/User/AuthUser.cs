using System;
using System.Collections.Generic;
using System.Web;
using BaseLib.Common;

namespace BaseLib.User
{
    public class AuthUser
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int ID { get; set; }

        public bool TestUser { get; set; }

        public List<UnionOrg> UnionOrgs { get; set; }

        private DateTime? _sessionLastUpdate;
        public DateTime? SessionLastUpdate
        {
            get
            {
                if (_sessionLastUpdate == null)
                    _sessionLastUpdate = DateTime.Now;
                return _sessionLastUpdate;
            }
            set { _sessionLastUpdate = value; }
        }

        public static AuthUser CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session != null)
                    return HttpContext.Current.Session["AuthUser"] as AuthUser;
                else return null;
            }
            set { HttpContext.Current.Session["AuthUser"] = value; }
        }

        public static bool Authenticated
        {
            get
            {
                return CurrentUser != null;
            }
        }

        public static Language CurrLanguage
        {
            get { return HttpContext.Current.Session["Language"] == null ? Language.Georgia : (Language)HttpContext.Current.Session["Language"]; }
            set { HttpContext.Current.Session["Language"] = (int)value; }
        }

        Dictionary<string, object> _additionalProperties = new Dictionary<string, object>();

        public Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

        #region Sequence

        private int _sequence = 0;
        public int NextSequence
        {
            get { return ++_sequence; }
            set { _sequence = value; }
        }

        #endregion
    }



    [Serializable]
    public class UnionOrg
    {
        public bool IsAvtive { get; set; }

        public int UnID { get; set; }

        public string OrgName { get; set; }
    }

}
