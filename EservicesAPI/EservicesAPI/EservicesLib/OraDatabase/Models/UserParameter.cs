using BaseLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.OraDatabase.Models
{
    public class PersonalInfo
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int SecretWord { get; set; }
        public int TwoWayAuthStatus{ get; set; }
        public DateTime PasswordExpireDate { get; set; }
        public DateTime PasswordChangeDate { get; set; }
  
    }
    public class AddressDetail
    {
        public string RegionId { get; set; }
        public string DistrictId { get; set; }
        public string StreetId { get; set; }
        public string StreetText { get; set; }
        public string PlaceNumber { get; set; }
    }

    public class SavedDevices
    {
        public decimal UserId { get; set; }
        public decimal SubUserId { get; set; }
        public DateTime InsertDate { get; set; }
        public string Vcode { get; set; }
        public string Address { get; set; }
        public string Browser { get; set; }
        public string OperSystem { get; set; }
    }
    public class UserContact
    {

        public decimal SetId { get; set; }


        public decimal UnId { get; set; }

 
        public decimal AppId { get; set; }

 
        public string AppName { get; set; }

   
        public string AppColor { get; set; }

  
        public string Mobile { get; set; }

  
        public string Mobile2 { get; set; }

 
        public string Phone { get; set; }


        public string Email { get; set; }


        public bool Visbility { get; set; }

      
        public bool Notify { get; set; }

      
        public bool NotifyMain { get; set; }

        public string SwitcherClass { get; set; }

        public string VisibilityTooltip { get; set; }

        public string NotifyTooltip { get; set; }

        public string ModuleVisibility { get; set; }

        public bool SendNotifyChecks { get; set; }

       
        public string SendNotifyText { get; set; }
    }


}
