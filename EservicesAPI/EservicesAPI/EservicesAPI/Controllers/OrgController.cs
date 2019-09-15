using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesAPI.Auth;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Controllers
{
    public class OrgController : MainController
    {
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage IsVatPayer(JObject json)
        {
            var data = new Dictionary<string, object>();

            int unID = 0;

            try
            {
                unID = json["UnID"].ToString().ToNumber<int>();
            }
            catch { }

            var vatDate = DateTime.Parse(json["VatDate"].ToString());

            var isVatPayer = DataProviderManager<PKG_ORG_INFO>.Provider.is_vat_payer(unID, vatDate);

            data.Add("IsVatPayer", isVatPayer);
            return Success(data);
        }

        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetVatPayerStatus(JObject json)
        {
            var data = new Dictionary<string, object>();

            var tin = json["Tin"].ToString();
            var vatDate = DateTime.Parse(json["VatDate"].ToString());

            var isVatPayer = DataProviderManager<PKG_ORG_INFO>.Provider.is_vat_payer(tin, vatDate);

            data.Add("IsVatPayer", isVatPayer);
            return Success(data);
        }

        [Authenticate(Module = Modules.OrgInfo, Permission = Permissions.Read)]
        public Dictionary<string, object> GetOrgInfo(string Tin, DateTime OperationDate, bool CheckRegister = true, bool CheckAuthUser = true, bool CheckUnderage = false)
        {
            var result = new Dictionary<string, object>();

            var buyerName = ""; var address = ""; var buyerUnID = 0; var isVat = false; var samforma = "";
            var isDiplomat = false;

            //if (CheckAuthUser && Tin == AuthUser.Tin)
            //    throw new UserExceptions("მყიდველის ნომერი არ უნდა ემთხვეოდეს ავტორიზებულ მომხმარებელს");

            if (Tin.Length >= 6 && Tin.Length != 8 && Tin.Length <= 11)
            {

                bool isRegister, isUnderaged;
                DataProviderManager<PKG_ORG_INFO>.Provider.get_user_info(ref Tin, OperationDate, out buyerName, out address, out isVat, out isDiplomat, out isRegister, out isUnderaged, out samforma);

                if (!string.IsNullOrEmpty(buyerName) && (Tin.Length == 11 || Tin.Length == 9) && !isRegister && CheckRegister)
                {
                    throw new UserExceptions("გადამხდელი არ არის რეგისტრირებული ელ-დეკლარირებაში");
                }

                if (CheckUnderage && isUnderaged)
                {
                    throw new UserExceptions("მყიდველი არ მოიძებნა! შეიყვანეთ სწორი მონაცემი.");
                }
            }

            try
            {
                string desc;
                DataProviderManager<PKG_ORG_INFO>.Provider.get_un_id_from_tin(Tin, out buyerUnID, out desc);
            }
            catch
            {
            }

            result.Add("Tin", Tin);
            result.Add("Address", address);
            result.Add("IsVatPayer", isVat);
            result.Add("IsDiplomat", isDiplomat);
            result.Add("UnID", buyerUnID);



            string buyerInitials = string.Empty;
            if (samforma == "ინდ მეწარმე" || samforma == "ფიზიკური პირი")
            {
                if (!string.IsNullOrEmpty(buyerName))
                {
                    var tempBuyerName = buyerName.Trim();
                    tempBuyerName = Regex.Replace(tempBuyerName, @"\s+", " ", RegexOptions.Multiline);
                    buyerInitials += tempBuyerName.Substring(0, 1) + ". ";

                    while (tempBuyerName.IndexOf(" ") > 0)
                    {
                        tempBuyerName = tempBuyerName.Substring(tempBuyerName.IndexOf(" ") + 1, tempBuyerName.Length - tempBuyerName.IndexOf(" ") - 1);
                        buyerInitials += tempBuyerName.Substring(0, 1) + ". ";
                    }
                }
                buyerInitials = buyerInitials.Substring(0, buyerInitials.Length - 1);

                result.Add("Name", buyerInitials);
            }
            else
            {
                result.Add("Name", buyerName);
            }

            return result;
        }
        
        /// <summary>
        /// აბრუნებს და ლოგინ ებული იუზერის სრულ სახელს , Tin სა და ინიციალებს, გამოიყენება angular ის header ში
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authenticate]
        public HttpResponseMessage GetUserInfo()
        {
            var tin = AuthUser.Tin;

            // if (json != null && json.ContainsKey("Tin")) Tin = json["Tin"].ToString();

            //   Tin = string.IsNullOrEmpty(Tin) ? AuthUser.Tin : Tin;

            if (string.IsNullOrEmpty(tin))
            {
                ThrowError(-1);
            }


            var result = new Dictionary<string, object>();

            var buyerName = ""; var address = ""; var buyerUnID = 0; var isVat = false; var samforma = "";
            var isDiplomat = false;

            if (tin.Length >= 6 && tin.Length != 8 && tin.Length <= 11)
            {
                bool isRegister, isUnderaged;

                DataProviderManager<PKG_ORG_INFO>.Provider.get_user_info(ref tin, DateTime.Now, out buyerName, out address, out isVat, out isDiplomat, out isRegister, out isUnderaged, out samforma);
            }

            result.Add("Tin", tin);
            result.Add("Name", buyerName);
            
            var headerInitials = GetInitials(samforma, buyerName);

            result.Add("Initials", headerInitials);

            return Success(result);
        }

        /// <summary>
        /// ანგულარის header ისთვის ინიციალები
        /// </summary>
        private string GetInitials(string samforma, string buyerName)
        {

            string buyerInitials = string.Empty;
            if (samforma == "ინდ მეწარმე" || samforma == "ფიზიკური პირი")
            {
                if (!string.IsNullOrEmpty(buyerName))
                {
                    var tempBuyerName = buyerName.Trim();
                    tempBuyerName = Regex.Replace(tempBuyerName, @"\s+", " ", RegexOptions.Multiline);
                    buyerInitials += tempBuyerName.Substring(0, 1);

                    while (tempBuyerName.IndexOf(" ") > 0)
                    {
                        tempBuyerName = tempBuyerName.Substring(tempBuyerName.IndexOf(" ") + 1, tempBuyerName.Length - tempBuyerName.IndexOf(" ") - 1);
                        buyerInitials += tempBuyerName.Substring(0, 1) ;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(buyerName))
                {
                    string initials = string.Empty;
                    string[] unwantedLetters = { ",", ".", "\"" };
                    string[] headers = { "შპს", "სს", "სსიპ" };

                    foreach (string x in unwantedLetters)
                    {
                        buyerInitials = buyerName.Replace(x, "");
                    }

                    foreach (string h in headers)
                    {
                        if (buyerInitials.StartsWith(h))
                            buyerInitials = buyerInitials.Substring(h.Length);
                    }
                    buyerInitials = buyerInitials.Trim().Substring(0, 1);
                }
            }

            return buyerInitials;
        }

        [HttpPost]
        //[Route("GetOrgInfoByTin")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage GetOrgInfoByTin([FromBody] Dictionary<string, object> json)
        {
            var Tin = string.Empty;

            if (json != null && json.ContainsKey("Tin")) Tin = json["Tin"].ToString();

            Tin = string.IsNullOrEmpty(Tin) ? AuthUser.Tin : Tin;

            var name = new OrgController().GetOrgInfo(Tin, DateTime.Now, false, true, true);

            if (!name.TryGetValue("Name", out object fullNameObj))
            {
                return Success(name);
            }

            var fullName = fullNameObj.ToString();

            //string initials = string.Empty;
            //string[] unwantedLetters = { ",", ".", "\"" };
            //string[] headers = { "შპს", "სს", "სსიპ" };

            //if (AuthUser.SamFormaType != SamformaType.Company)
            //    initials = (fullName.Split(' ').Length > 1) ? fullName.Split(' ')[0].Substring(0, 1) +
            //                                                  fullName.Split(' ')[1].Substring(0, 1) : fullName.Split(' ')[0].Substring(0, 1);
            //else
            //{
            //    initials = fullName;

            //    foreach (string x in unwantedLetters)
            //    {
            //        initials = initials.Replace(x, "");
            //    }

            //    string header = (initials.IndexOf(' ') == -1) ? string.Empty : initials.Substring(0, initials.IndexOf(' '));

            //    foreach (string h in headers)
            //    {
            //        if (header == h) initials = initials.Substring(header.Length, 1);
            //    }
            //    initials = initials.Substring(0, 1);
            //}

            //name.Add("Initials", initials);

            return Success(name);
        }
        [HttpPost]
        //[Route("NonResidentInfo")]
        [Authenticate(Module = Modules.Invoice, Permission = Permissions.Read)]
        public HttpResponseMessage NonResidentInfo([FromBody] Dictionary<string, object> json)
        {
            var data = new Dictionary<string, object>();
            var fullName = string.Empty;

            var Tin = string.Empty;

            if (json != null && json.ContainsKey("Tin")) Tin = json["Tin"].ToString();

            DataProviderManager<PKG_ORG_INFO>.Provider.get_non_resident(Tin, out fullName);
            data.Add("Name", fullName);


            return Success(data);
        }

    }
}
