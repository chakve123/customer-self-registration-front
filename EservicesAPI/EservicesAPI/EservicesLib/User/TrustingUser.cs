using System;
using System.Linq;

namespace EservicesLib.User
{
    [Serializable]
    public class TrustingUser
    {
        public string FullName { get; set; }

        public int UserID { get; set; }

        public int UserType { get; set; }

        public string UserName { get; set; }

        public string Tin { get; set; }

        public int SamFormaID { get; set; }

        public string SamFormaName { get; set; }

        public string SamForma
        {
            get
            {
                switch (SamFormaType)
                {
                    case SamformaType.Company: return "შ.პ.ს.";
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
                if (new[] { 28, 18, 19, 20, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 15, 21, 8, 22, 23, 24, 25, 26, 27 }.Contains(SamFormaID))
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

        public TrustingUser()
        {
        }

        public TrustingUser(string name, int id, int type, string userName, string tin, int samForma, string samFormaName)
        {
            FullName = name;
            UserID = id;
            UserType = type;
            UserName = userName;
            Tin = tin;
            SamFormaID = samForma;
            SamFormaName = samFormaName;
        }
    }
}
