using BaseLib.ExtensionMethods;
using EservicesLib.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.Models.Invoice
{
    public class InvoiceSub
    {
        public Field<string> OPERATION_DATE_STR = new Field<string>("");
        public Field<string> SUB_INVOICE_ID = new Field<string>();
        public Field<string> INV_NUMBER = new Field<string>();
        public Field<string> INV_SERIE = new Field<string>();
        public Field<string> INV_STATUS = new Field<string>();
        public Field<string> BUYER = new Field<string>();
        public Field<string> SELLER = new Field<string>();
        public Field<DateTime> OPERATION_DATE = new Field<DateTime>(DateTime.Today);
        public Field<int> INV_CATEGORY = new Field<int>();
        public Field<int> INV_TYPE = new Field<int>();
        public Field<int> AMOUNT_FULL = new Field<int>();
        public Field<int> AMOUNT = new Field<int>();
        public Field<int> AMOUNT_MAX = new Field<int>();
        public Field<int> GOODS_SUM = new Field<int>();
        public Field<int> GOODS_AMOUNT_SUM = new Field<int>();
        public Field<string> ACTIVATE_DATE = new Field<string>("");
        // this.OPERATION_DATE_STR.value = this.calendar.formatToGeoDate(new Date(OPERATION_DATE.value).toDateString());
        // ACTIVATE_DATE.value = this.calendar.formatToGeoDate(new Date(ACTIVATE_DATE.value).toDateString());
        public InvoiceSub()
        {
            AMOUNT.isValidFunction = () =>
            {
                if (AMOUNT.value.ToString().ToNumber<int>() > AMOUNT_MAX.value.ToString().ToNumber<int>() || AMOUNT.value.ToString().ToNumber<int>() <= 0)
                    return false;

                return true;
            };
        }
    }



    public class InvoiceReturn
    {
        public Field<int> RETURN_INVOICE_ID = new Field<int>();
        public Field<int> CORRECTED_INVOICE_ID = new Field<int>();
        public Field<string> INV_NUMBER = new Field<string>();
        public Field<string> INV_SERIE = new Field<string>();
        public Field<string> INV_STATUS = new Field<string>();
        public Field<string> BUYER = new Field<string>();
        public Field<DateTime> OPERATION_DATE = new Field<DateTime>(DateTime.Today);
    }

    public class InvoiceDetails
    {
        public bool isLocked()
        {
            if (this.SELLER_ACTION.value == -1) return true;  // დოკუმენტი წაშლილია

            if (this.USER_ROLE.value == 2 || this.USER_ROLE.value == 3) return true; // 1 - გამყიდველი, 2 - მყიდველი, 3 - გადამზიდავი კომპანია
            if (this.ID.value > 0)
            { // თუ გახსნილია არსებული დოკუმენტი
                if (this.SELLER_ACTION.value == 1) // თუ გააქტიურებულია
                    return true;
                if (this.SELLER_ACTION.value == 2) // თუ გაუქმებულია
                    return true;
                if (this.SELLER_ACTION.value == 3) // თუ კორექტირებულია
                    return true;
                if ((this.BUYER_ACTION.value == 1 || this.BUYER_ACTION.value == 2) && this.INV_TYPE.value != 1) // თუ დადასტურებულია ან უარყოფილია და არ არის შიდა გადაზიდვა
                    return true;
            }
            return false;
        }

        public bool oilTransStarted()
        {
            if (this.INV_CATEGORY.value == (int)InvCategory.Oil &&
                this.TRANS_START_DATE.value != null &&
                this.TRANS_START_DATE.value.ToString() != "")
            {
                return true;
            }
            return false;
        }
        private bool isOilImpOrExp()
        {
            return this.INV_CATEGORY.value == (int)InvCategory.Oil && (this.INV_TYPE.value == 9 || this.INV_TYPE.value == 10) && !this.isLocked();
        }


        //public static Object[] INVOICE_CATEGORIES_ALL = new Object[new Object { label:"1"; value = "2"; }];
        //    //{ label = "მიწოდება/მომსახურება", value = 1 },
        //    //{ label: "ხე-ტყე", value: 2, disabled:true },
        //    //{ label: "ნავთობი", value: 3, disabled:true },
        //    //{ label: "ავანსი", value: 4 },


        //public Field<string> static INVOICE_TYPES_ALL: Object[] = [
        //    { label: "შიდა გადაზიდვა", value: 1, category: ",1,3,2," },
        //    { label: "ტრანსპორტირებით", value: 2, category: ",1,2," },
        //    { label: "ტრანსპორტირების გარეშე", value: 3, category: ",1,2," },
        //    { label: "დისტრიბუცია", value: 4, category: ",1,2," },
        //    { label: "უკან დაბრუნება", value: 5, category: ",1,2," },
        //    { label: "ავანსი", value: 6, category: ",4," }, // this inv_type is hidden but usable, use this value for filtering or something instead of inv_category value
        //    { label: "საცალო მიწოდებისთვის", value: 7, category: ",3," },
        //    { label: "საბითუმო მიწოდებისთვის", value: 8, category: ",3," },
        //    { label: "იმპორტირებისას დასაწყობების ადგილამდე ტრანსპორტირებისათვის", value: 9, category: ",3," },
        //    { label: "ექსპორტისას დასაწყობების ადგილიდან ტრანსპორტირებისათვის", value: 10, category: ",3," },
        //    { label: "მომსახურება", value: 11, category: ",1," }, // this inv_type is hidden but usable, use this value for filtering or something instead of inv_category value
        //]

        //public Field<string> static TRANS_TYPES_ALL: Object[] = [
        //    { label: "საავტომობილო", value: 1, category: ",1,2,3,", type: ",1,2,4,5,7,8,9,10," },
        //    { label: "გადამზიდავი საავტომობილო", value: 2, category: ",1,", type: ",1,2,5," },
        //    { label: "სარკინიგზო", value: 4, category: ",3,", type: "*" },
        //    { label: "მილსადენი", value: 5, category: ",3,", type: "*" },
        //    { label: "ტალონებით", value: 6, category: ",3,", type: "*" },
        //    { label: "პლასტიკური ბარათით", value: 7, category: ",3,", type: "*" },
        //    { label: "გადაწერა", value: 8, category: ",3,", type: ",7,8,9,10," },
        //    { label: "საავიაციო", value: 9, category: ",3,", type: "*" },
        //    { label: "საზღვაო", value: 10, category: ",3,", type: "*" },
        //    { label: "სხვა", value: 3, category: ",1,3,", type: ",1,2,4,5,7,8,9,10," }
        //]

        //public Field<string> static WOOD_DOCUMENTS_ALL: WoodDocument[] = [];

        //@JsonIgnore()
        //public Field<string> TransTypes: Object[] = [];

        #region Groups
        public FieldGroup CommonInfoGroup = new FieldGroup();
        public FieldGroup SellerGroup = new FieldGroup();
        public FieldGroup BuyerGroup = new FieldGroup();
        public FieldGroup TransportGroup = new FieldGroup();
        public FieldGroup TransportStartGroup = new FieldGroup();
        public FieldGroup TransportEndGroup = new FieldGroup();
        public FieldGroup TransCompanyGroup = new FieldGroup();
        public FieldGroup TransDriverGroup = new FieldGroup();
        #endregion


        public Field<int> ID = new Field<int>();
        public Field<string> INV_SERIE = new Field<string>();
        public Field<string> INV_NUMBER = new Field<string>();
        public Field<int> INV_CATEGORY = new Field<int>();
        public Field<int> INV_TYPE = new Field<int>();
        public Field<int> SELLER_ACTION = new Field<int>();
        public Field<int> BUYER_ACTION = new Field<int>();

        public Field<DateTime> OPERATION_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> ACTIVATE_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> CREATE_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> CONFIRM_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> REFUSE_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> REQUEST_CANCEL_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> DELIVERY_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> AGREE_CANCEL_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> CORRECT_DATE = new Field<DateTime>(DateTime.Today);
        public Field<DateTime> TRANS_START_DATE = new Field<DateTime>(null);
        // public Field<string> TRANS_START_DATE_ORIGINAL= new Field<string>();

        public Field<int> CORRECT_REASON_ID = new Field<int>();
        public Field<string> UNID_SELLER = new Field<string>();
        public Field<string> UNID_BUYER = new Field<string>();
        public Field<string> TIN_SELLER = new Field<string>();
        public Field<string> TIN_BUYER = new Field<string>();
        public Field<bool> FOREIGN_BUYER = new Field<bool>();
        public Field<string> NAME_SELLER = new Field<string>();
        public Field<string> NAME_BUYER = new Field<string>();
        public Field<string> USER_ID_SELLER = new Field<string>();
        public Field<string> USER_ID_BUYER = new Field<string>();
        public Field<string> SEQNUM_SELLER = new Field<string>();
        public Field<string> SEQNUM_BUYER = new Field<string>();
        public Field<string> SELLER_STATUS = new Field<string>();
        public Field<string> BUYER_STATUS = new Field<string>();
        public Field<string> STATUS_TXT_GEO = new Field<string>();
        public Field<string> STATUS_TXT_ENG = new Field<string>();
        public Field<string> SUBUSER_ID_SELLER = new Field<string>();
        public Field<string> SUBUSER_ID_BUYER = new Field<string>();
        public Field<int> AMOUNT_FULL = new Field<int>();
        public Field<string> AMOUNT_EXCISE = new Field<string>();
        public Field<int> AMOUNT_VAT = new Field<int>();
        public Field<int> AMOUNT_MAX = new Field<int>();

        public Field<int> QUANTITY_SUM = new Field<int>();

        public Field<string> TRANS_START_ADDRESS = new Field<string>();
        public Field<string> TRANS_END_ADDRESS = new Field<string>();
        public Field<string> TRANS_START_ADDRESS_NO = new Field<string>();
        public Field<string> TRANS_END_ADDRESS_NO = new Field<string>();


        public Field<int> TRANS_TYPE = new Field<int>();
        public Field<string> TRANS_TYPE_TXT = new Field<string>();
        public Field<string> TRANS_COMPANY_UNID = new Field<string>();
        public Field<string> TRANS_COMPANY_TIN = new Field<string>();
        public Field<string> TRANS_COMPANY_NAME = new Field<string>();
        public Field<bool> TRANS_DRIVER_FOREIGN = new Field<bool>();
        public Field<string> TRANS_DRIVER_TIN = new Field<string>();
        public Field<string> TRANS_DRIVER_NAME = new Field<string>();
        public Field<string> TRANS_DRIVER_COUNTRY = new Field<string>();
        public Field<string> TRANS_CAR_MODEL = new Field<string>();
        public Field<string> TRANS_CAR_NO = new Field<string>();
        public Field<string> TRANS_TRAILER_NO = new Field<string>();
        public Field<int> TRANS_COST = new Field<int>();
        public Field<int> TRANS_COST_PAYER = new Field<int>();
        public Field<string> INV_COMMENT = new Field<string>();
        public Field<int> PARENT_ID = new Field<int>();
        public Field<int> PREV_CORRECTION_ID = new Field<int>();
        public Field<int> NEXT_CORRECTION_ID = new Field<int>();
        public Field<DateTime> PREV_OPERATION_DATE = new Field<DateTime>(DateTime.Today);

        public Field<string> INV_SOURCE = new Field<string>();
        public Field<int> USER_ROLE = new Field<int>(); // 1 - seller 2 - buyer 3 - transporter

        public List<InvoiceGoods> INVOICE_GOODS = new List<InvoiceGoods>();
        public List<InvoiceGoods> INVOICE_PARENT_GOODS = new List<InvoiceGoods>();
        public List<InvoiceReturn> INVOICE_RETURN = new List<InvoiceReturn>();
        public List<InvoiceSub> SUB_INVOICES_DISTRIBUTION = new List<InvoiceSub>();
        //public InvoiceOilDoc[] INVOICE_OIL_DOCS = [];
        public List<InvoiceSub> INVOICE_ADVANCE = new List<InvoiceSub>();

        public InvoiceDetails()
        {
            #region Set Groups
            // Attaching Children to their Groups, so when GROUP is not visible, his Children should not pay attention to Validations 
            this.TIN_SELLER.group = this.SellerGroup;
            this.NAME_SELLER.group = this.SellerGroup;
            this.TIN_BUYER.group = this.BuyerGroup;
            this.NAME_BUYER.group = this.BuyerGroup;
            this.FOREIGN_BUYER.group = this.BuyerGroup;
            this.TRANS_START_DATE.group = this.TransportStartGroup;
            this.TRANS_START_ADDRESS.group = this.TransportStartGroup;
            this.TRANS_START_ADDRESS_NO.group = this.TransportStartGroup;
            this.TRANS_END_ADDRESS.group = this.TransportEndGroup;
            this.TRANS_END_ADDRESS_NO.group = this.TransportEndGroup;
            this.TRANS_TYPE.group = this.TransportGroup;
            this.TRANS_TYPE_TXT.group = this.TransportGroup;
            this.TRANS_COMPANY_UNID.group = this.TransCompanyGroup;
            this.TRANS_COMPANY_NAME.group = this.TransCompanyGroup;
            this.TRANS_COMPANY_TIN.group = this.TransCompanyGroup;
            this.TRANS_DRIVER_FOREIGN.group = this.TransDriverGroup;
            this.TRANS_DRIVER_TIN.group = this.TransDriverGroup;
            this.TRANS_DRIVER_NAME.group = this.TransDriverGroup;
            this.TRANS_DRIVER_COUNTRY.group = this.TransDriverGroup;
            this.TRANS_CAR_MODEL.group = this.TransDriverGroup;
            this.TRANS_CAR_NO.group = this.TransDriverGroup;
            this.TRANS_TRAILER_NO.group = this.TransDriverGroup;
            this.TRANS_COST.group = this.TransDriverGroup;
            this.TRANS_COST_PAYER.group = this.TransDriverGroup;
            #endregion

            #region Group Visible Functions
            this.SellerGroup.isVisibleFunction = () =>
            {
                if (this.INV_TYPE.value != 9) return true;
                else return false;
            };
            this.BuyerGroup.isVisibleFunction = () =>
            {
                return (this.INV_TYPE.value != 1 && this.INV_TYPE.value != 4 && this.INV_TYPE.value != 10);

            };
            this.TransportGroup.isVisibleFunction = () =>
            {
                return this.INV_TYPE.value != 3 && (this.PARENT_ID.value == 0 || this.PARENT_ID.value != 0 && this.INV_TYPE.value == 1) && this.INV_TYPE.value != 6 && this.INV_TYPE.value != 11;
            };
            this.TransportStartGroup.isVisibleFunction = () =>
            {
                if (!this.TransportGroup.isVisibleGroup()) return false;
                return this.TRANS_TYPE.value > 0;
            };
            this.TransportEndGroup.isVisibleFunction = () =>
            {
                if (!this.TransportStartGroup.isVisibleGroup()) return false;
                return this.INV_TYPE.value != 4;
            };

            this.TransCompanyGroup.isVisibleFunction = () =>
            {
                if (!this.TransportStartGroup.isVisibleGroup()) return false;

                return this.TRANS_TYPE.value == 2 || this.INV_CATEGORY.value == 3;
            };
            this.TransDriverGroup.isVisibleFunction = () =>
            {
                if (!this.TransportStartGroup.isVisibleGroup()) return false;

                return this.TRANS_TYPE.value == 1 || this.TRANS_TYPE.value == 2 || this.TRANS_TYPE.value == 4;
            };
            #endregion

            #region Visible Functions
            this.FOREIGN_BUYER.isVisibleFunction = () =>
            {
                return this.INV_CATEGORY.value != 3;
            };

            this.TRANS_TYPE_TXT.isVisibleFunction = () =>
            {
                return this.TRANS_TYPE.value == 3;
            };
            this.TRANS_START_ADDRESS_NO.isVisibleFunction = () =>
            {
                return this.INV_CATEGORY.value == 3;
            };
            this.TRANS_END_ADDRESS_NO.isVisibleFunction = () =>
            {
                return this.INV_CATEGORY.value == 3;
            };
            this.TRANS_DRIVER_TIN.isVisibleFunction = () =>
            {
                return this.TRANS_TYPE.value != 4;
            };
            this.TRANS_DRIVER_NAME.isVisibleFunction = () =>
            {
                return this.TRANS_TYPE.value != 4;
            };
            this.TRANS_CAR_MODEL.isVisibleFunction = () =>
            {
                return this.TRANS_TYPE.value != 4;
            };
            this.TRANS_TRAILER_NO.isVisibleFunction = () =>
            {
                return this.TRANS_TYPE.value != 4;
            };
            this.TRANS_COST.isVisibleFunction = () =>
            {
                return this.INV_CATEGORY.value == 1 && this.INV_TYPE.value != 4;
            };

            #endregion


            #region  Lock Functions

            this.INV_CATEGORY.isLockedFunction = () =>
            {
                if (this.ID.value > 0) return true;

                if (this.ID.value == 0 && (this.INV_TYPE.value == 3 || this.INV_TYPE.value == 1) && this.PARENT_ID.value > 0) return true; // დისტრიბუციის ქვე დოკუმენტის გამოწერის დროს დალოქოს

                if (this.ID.value == 0 && this.PREV_CORRECTION_ID.value > 0) return true; // თუ ვქმნით კორექტირების ახალ დოკუმენტს

                return this.isLocked();
            };

            this.INV_TYPE.isLockedFunction = () =>
            {
                if (this.ID.value > 0) return true;

                if (this.ID.value == 0 && (this.INV_TYPE.value == 3 || this.INV_TYPE.value == 1) && this.PARENT_ID.value > 0) return true; // დისტრიბუციის ქვე დოკუმენტის გამოწერის დროს დალოქოს

                if (this.ID.value == 0 && this.PREV_CORRECTION_ID.value > 0) return true; // თუ ვქმნით კორექტირების ახალ დოკუმენტს

                return this.isLocked();
            };

            this.OPERATION_DATE.isLockedFunction = () =>
            {
                if (this.ID.value > 0) return true;
                if (this.CORRECT_REASON_ID.value == 5) // რედაქტირება
                    return true;

                return this.isLocked();
            };

            this.TIN_BUYER.isLockedFunction = () =>
            {
                if (this.INVOICE_ADVANCE.Count > 0)
                    return true;
                return this.isLocked();
            };

            this.TIN_SELLER.isLockedFunction = () =>
            {
                return this.isLocked();
            };

            this.TRANS_TYPE.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;
                if (this.ID.value > 0) return true;
                if (this.isOilImpOrExp()) return false;

                return this.isLocked();
            };
            this.TRANS_TYPE_TXT.isLockedFunction = () =>
        {
            if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                return true;
            return this.isLocked();
        };

            this.TRANS_START_DATE.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil)
                {
                    if (this.SELLER_ACTION.value == 1 && this.BUYER_ACTION.value == 1) return false;
                    else return true;
                }
                return this.isLocked();
            };



            this.TRANS_COMPANY_TIN.isLockedFunction = () =>
            {
                return this.isLocked();
            };

            this.TRANS_DRIVER_TIN.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;

                if (this.TRANS_TYPE.value == 2 && this.USER_ROLE.value != 3) return true;
                else if (this.USER_ROLE.value == 3) return false;

                if (this.isOilImpOrExp()) return false;

                return this.isLocked();
            };

            this.TRANS_CAR_MODEL.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;

                if (this.TRANS_TYPE.value == 2 && this.USER_ROLE.value != 3) return true;
                else if (this.USER_ROLE.value == 3) return false;

                if (this.isOilImpOrExp()) return false;

                return this.isLocked();
            };


            this.TRANS_CAR_NO.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;

                if (this.TRANS_TYPE.value == 2 && this.USER_ROLE.value != 3) return true;
                else if (this.USER_ROLE.value == 3) return false;

                if (this.isOilImpOrExp()) return false;

                return this.isLocked();
            };

            this.TRANS_TRAILER_NO.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;

                if (this.TRANS_TYPE.value == 2 && this.USER_ROLE.value != 3) return true;
                else if (this.USER_ROLE.value == 3) return false;

                return this.isLocked();
            };

            this.TRANS_COST.isLockedFunction = () =>
            {
                return this.isLocked();
            };

            this.TRANS_COST_PAYER.isLockedFunction = () =>
            {
                return this.isLocked();
            };


            this.TRANS_START_ADDRESS_NO.isLockedFunction = () =>
            {
                return true;
            };

            this.TRANS_START_ADDRESS.isLockedFunction = () =>
            {
                if (this.ID.value == 0 && this.INV_TYPE.value == 1 && this.PARENT_ID.value > 0) // თუ დისტრიბუციის ქვე დოკუმენტია (შიდა გადაზიდვა)
                    return true;

                if (this.isLocked()) return true;

                if (this.INV_CATEGORY.value == (int)InvCategory.Oil &&
                    (this.TRANS_START_ADDRESS_NO.value.Substring(0, 2) == "19" || this.TRANS_START_ADDRESS_NO.value == "-")) return false;
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_START_ADDRESS_NO.value != "") return true;

                return false;
            };

            this.TRANS_END_ADDRESS_NO.isLockedFunction = () =>
            {
                return true;
            };

            this.TRANS_END_ADDRESS.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;

                if (this.INV_CATEGORY.value == (int)InvCategory.Oil &&
                    (this.TRANS_END_ADDRESS_NO.value.Substring(0, 2) == "19" || this.TRANS_END_ADDRESS_NO.value == "-"))
                    return false;
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_END_ADDRESS_NO.value != "") return true;

                return false;
            };

            this.TIN_SELLER.isLockedFunction = () => { return true; };

            this.NAME_SELLER.isLockedFunction = () => { return true; };

            this.NAME_BUYER.isLockedFunction = () =>
            {
                return true;
            };

            this.SUBUSER_ID_BUYER.isLockedFunction = () => { return true; };
            this.TRANS_COMPANY_NAME.isLockedFunction = () => { return true; };

            this.TRANS_DRIVER_NAME.isLockedFunction = () =>
            {
                if (this.isOilImpOrExp()) return false;
                return true;
            };

            this.INV_COMMENT.isLockedFunction = () =>
            {

                if (this.isLocked()) return true;

                if (this.USER_ROLE.value != 1)
                    return true;

                return false;
            };


            #endregion

            #region  Valid Functions


            this.INV_CATEGORY.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value > -1)
                    return true;

                return false;
            };
            this.INV_TYPE.isValidFunction = () =>
            {
                if (this.INV_TYPE.value > -1)
                    return true;

                return false;
            };

            this.OPERATION_DATE.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value != (int)InvCategory.Oil && this.OPERATION_DATE.value.Date > DateTime.Today)
                    return false;

                // თუ ავანსის ოპერაციის თარიღი მეტია სდ -ს ოპერაციის თარიღზე ვალიდაცია არ გაატაროს
                var BreakException = "";
                try
                {
                    foreach (var element in this.INVOICE_ADVANCE)
                    {
                        if (element.OPERATION_DATE.value.Date >= this.OPERATION_DATE.value.Date)
                            throw new Exception();
                    };
                }
                catch (Exception ex)
                {
                    this.OPERATION_DATE.errorMessage = "ოპერაციის პერიოდი უნდა იყოს ავანსის ოპერაციის პერიოდზე მეტი.";
                    return false;
                }

                return true;
            };

            this.TIN_BUYER.isValidFunction = () =>
            {
                if ((this.TIN_BUYER.value.Length >= 6 && this.TIN_BUYER.value.Length != 8 && this.TIN_BUYER.value.Length <= 11) || (this.FOREIGN_BUYER.value && this.TIN_BUYER.value.Length > 0))
                    return true;

                return false;
            };

            this.NAME_BUYER.isValidFunction = () =>
            {
                if (this.NAME_BUYER.value.Length > 0) return true;

                return false;
            };

            this.TIN_SELLER.isValidFunction = () =>
            {
                if (this.TIN_SELLER.value.Length >= 6 && this.TIN_SELLER.value.Length != 8 && this.TIN_SELLER.value.Length <= 11)
                    return true;

                return false;
            };

            this.NAME_SELLER.isValidFunction = () =>
            {
                if (this.NAME_SELLER.value.Length > 0) return true;

                return false;
            };

            this.TRANS_TYPE.isValidFunction = () =>
            {
                //if (this.TransTypes.findIndex(x => typeof x["value"] != "undefined" && x["value"] == this.TRANS_TYPE.value) == -1)
                //    return false;

                if (this.TRANS_TYPE.value > 0)
                    return true;

                return false;
            };
            this.TRANS_TYPE_TXT.isValidFunction = () =>
            {
                if (this.TRANS_TYPE_TXT.value.Length > 0)
                    return true;

                return false;
            };

            this.TRANS_START_DATE.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil) return true;

                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;

                if (opDate.Date < today.Date) return true;

                if (this.TRANS_START_DATE.value != null && this.TRANS_START_DATE.value.Year.ToString() != "1")  // if date is null
                    return true;
                return false;
            };

            this.TRANS_START_ADDRESS.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil) return true;
                if (this.TRANS_START_ADDRESS.value.Length > 0)
                    return true;

                return false;
            };

            this.TRANS_START_ADDRESS_NO.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_START_ADDRESS_NO.value == "") return false;
                return true;
            };

            this.TRANS_COMPANY_TIN.isValidFunction = () =>
            {
                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;

                if (opDate.Date < today.Date) return true;

                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_COMPANY_TIN.value == "") return true;
                if (this.TRANS_COMPANY_TIN.value.Length >= 6 && this.TRANS_COMPANY_TIN.value.Length != 8 && this.TRANS_COMPANY_TIN.value.Length <= 11) return true;

                return false;
            };

            this.TRANS_DRIVER_TIN.isValidFunction = () =>
            {
                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;


                if (opDate.Date < today.Date) return true;

                if (this.TRANS_DRIVER_TIN.isLocked()) return true;

                if ((this.TRANS_DRIVER_TIN.value.Length >= 6 && this.TRANS_DRIVER_TIN.value.Length != 8 && this.TRANS_DRIVER_TIN.value.Length <= 11) || (this.TRANS_DRIVER_FOREIGN.value && this.TRANS_DRIVER_TIN.value.Length > 0))
                    return true;

                return false;
            };

            this.TRANS_DRIVER_NAME.isValidFunction = () =>
            {
                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;

                if (opDate.Date < today.Date) return true;


                if (this.TRANS_DRIVER_TIN.isLocked()) return true;

                if (this.TRANS_DRIVER_NAME.value.Length > 0) return true;
                else return false;
            };

            this.TRANS_CAR_MODEL.isValidFunction = () =>
            {
                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;
                if (opDate.Date < today.Date) return true;


                if (this.TRANS_CAR_MODEL.isLocked()) return true;
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_TYPE.value == (int)TransType.Automobile) return true;

                if (this.TRANS_CAR_MODEL.value.Length > 0) return true;

                return false;
            };

            this.TRANS_CAR_NO.isValidFunction = () =>
            {
                var opDate = this.OPERATION_DATE.value;
                var today = DateTime.Today;
                if (opDate.Date < today.Date) return true;


                if (this.TRANS_CAR_NO.isLocked()) return true;

                if (this.TRANS_CAR_NO.value.Length > 0)
                    return true;
                return false;
            };

            this.TRANS_COST_PAYER.isValidFunction = () =>
            {
                if (this.INV_TYPE.value == 1 && this.TRANS_TYPE.value == 1) // თუ შიდა გადაზიდვაა საავტომობილო
                    return true;
                if (this.TRANS_COST_PAYER.value > 0)
                    return true;

                return false;
            };

            this.TRANS_END_ADDRESS.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil) return true;
                if (this.TRANS_END_ADDRESS.value.Length > 0)
                    return true;

                return false;
            };

            this.TRANS_END_ADDRESS_NO.isValidFunction = () =>
            {
                if (this.INV_CATEGORY.value == (int)InvCategory.Oil && this.TRANS_END_ADDRESS_NO.value == "") return false;
                return true;
            };


            #endregion

        }

    }


    public class InvoiceGoods
    {

        public bool isLocked()
        {
            if (this.QUANTITY_MAX.value != null)  // is Distribution Sub Invoice then lock 
                return true;
            return this.LOCKED; // პოპაპის გახსნისას ივსება this.LOCKED = InvoiceDetails.isLocked() - მთლიანი დოკუმენტის ლოქი
        }

        private bool isInternalOilInv()
        {
            if (this.INV_CATEGORY == (int)InvCategory.Oil &&
              (this.INV_TYPE == (int)InvoiceType.Import ||
                this.INV_TYPE == (int)InvoiceType.Export ||
                this.INV_TYPE == (int)InvoiceType.InternalTrans))
            {
                return true;
            }
            else return false;
        }

        public int INV_TYPE = 0;
        public int INV_CATEGORY = 0;
        public int CORRECT_REASON_ID = 0;
        public bool LOCKED = false;
        public Field<int> LOCAL_ID = new Field<int>();
        public Field<int> ID = new Field<int>();
        public Field<int> INVOICE_ID = new Field<int>();
        public Field<string> GOODS_NAME = new Field<string>();
        public Field<string> BARCODE = new Field<string>();
        public Field<int> UNIT_ID = new Field<int>();
        public Field<string> UNIT_TXT = new Field<string>();
        public Field<double> QUANTITY = new Field<double>();
        public Field<double> QUANTITY_EXT = new Field<double>();
        public Field<double> UNIT_PRICE = new Field<double>();
        public Field<double> AMOUNT = new Field<double>();
        public Field<double> VAT_AMOUNT = new Field<double>();
        public Field<double> EXCISE_AMOUNT = new Field<double>();
        public Field<string> EXCISE_AMOUNT_TXT = new Field<string>();
        public Field<string> EXCISE_ID = new Field<string>();
        public Field<int> VAT_TYPE = new Field<int>();
        public Field<string> VAT_TYPE_TXT = new Field<string>();
        public Field<double> EXCISE_UNIT_PRICE = new Field<double>();
        public Field<string> QUANTITY_MAX = new Field<string>(null); //დისტრიბუციის ქვე-დოკუმენტის დროს რაოდენობის ლიმიტი
        public Field<double> QUANTITY_STOCK = new Field<double>(); // დისტრიბუციის ქვე-დოკუმენტის დროს, მთავარი დოკუმენტის მარაგის(გაუნაშთავი) რაოდენობა
        public Field<double> QUANTITY_USED = new Field<double>(); // დისტრიბუციის მთავარი დოკუმენტის დროს, ქვე დოკუმენტებში ჯამური განაშთული რაოდენობა
        public Field<double> QUANTITY_FULL = new Field<double>();
        public Field<string> SSF_CODE = new Field<string>();
        public InvoiceGoods()
        {
            // --------------           VALIDATIONS    -------------------------

            GOODS_NAME.isValidFunction = () =>
            {
                if (this.INV_TYPE == (int)InvoiceType.AdvanceInvoice) return true;
                if (this.GOODS_NAME.value == "") return false;
                return true;
            };


            BARCODE.isValidFunction = () =>
            {
                if (this.INV_CATEGORY == (int)InvCategory.Oil || this.INV_CATEGORY == (int)InvCategory.Advance || this.INV_TYPE == (int)InvoiceType.InternalTrans || this.INV_TYPE == (int)InvoiceType.Service) return true;
                if (this.BARCODE.value == "") return false;
                return true;
            };


            UNIT_ID.isValidFunction = () =>
            {

                if (this.INV_CATEGORY == (int)InvCategory.Oil || this.INV_CATEGORY == (int)InvCategory.Advance) return true;
                if (this.UNIT_ID.value == 0) return false;
                return true;
            };


            QUANTITY.isValidFunctionParams = (param) =>
            {
                if (this.INV_TYPE == (int)InvoiceType.Distribution) // თუ დისტრიბუციის მთავარი დოკუმენტია
                    if (this.QUANTITY.value < this.QUANTITY_USED.value)
                    {
                        this.QUANTITY.errorMessage = "შეყვანილი რაოდენობა არ უნდა იყოს განაშთული რაოდენობაზე ნაკლები.";
                        return false;
                    }
                if (this.QUANTITY_MAX.value != null) // თუ დისტრიბუციის ქვე დოკუმენტია
                    if (this.QUANTITY.value <= 0)
                        if (param != null && param.GetType().GetProperty("QUANTITY_SUM").GetValue(param, null).ToString().ToNumber<int>() <= 0)
                        {
                            this.QUANTITY.errorMessage = "შეიყვანეთ პროდუქციის რაოდენობა";
                            return false;
                        }
                        else return true;

                if (this.UNIT_ID.value == 16) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.SSF_CODE.value == "2713") return true;
                if (this.INV_CATEGORY == (int)InvCategory.Advance) return true;
                if (this.QUANTITY.value <= 0) return false;
                if (this.QUANTITY_MAX.value != null) // თუ დისტრიბუციის ქვე დოკუმენტია
                    if (this.QUANTITY.value > this.QUANTITY_MAX.value.ToString().ToNumber<int>())
                    {
                        this.QUANTITY.errorMessage = "შეყვანილი რაოდენობა არ უნდა იყოს " + this.QUANTITY_MAX.value + " ზე მეტი";
                        return false;
                    }
                return true;
            };

            QUANTITY_EXT.isValidFunction = () =>
            {
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.SSF_CODE.value == "2713" && QUANTITY_EXT.value <= 0) return false;
                return true;
            };

            UNIT_PRICE.isValidFunction = () =>
            {
                if (this.UNIT_ID.value == 16) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Advance) return true;
                if (this.isInternalOilInv()) return true;
                if (this.INV_TYPE == (int)InvoiceType.InternalTrans) return true;
                if (this.UNIT_PRICE.value <= 0 && this.INV_TYPE != 5) return false;
                return true;
            };

            AMOUNT.isValidFunction = () =>
            {
                if (this.isInternalOilInv()) return true;
                if (this.INV_TYPE == (int)InvoiceType.InternalTrans) return true;
                if (this.AMOUNT.value <= 0 && this.INV_TYPE != (int)InvoiceType.Return) return false;
                return true;
            };

            VAT_AMOUNT.isValidFunction = () =>
            {
                if (this.INV_TYPE == (int)InvoiceType.InternalTrans) return true;
                if (this.VAT_AMOUNT.value < 0) return false;
                return true;
            };

            VAT_TYPE.isValidFunction = () =>
            {
                if (this.isInternalOilInv() || this.INV_TYPE == (int)InvoiceType.InternalTrans) return true;
                if (this.VAT_TYPE.value == -1) return false;
                return true;
            };

            EXCISE_AMOUNT_TXT.isValidFunction = () =>
            {
                if (this.INV_CATEGORY == (int)InvCategory.Advance || this.INV_TYPE == (int)InvoiceType.InternalTrans) return true;
                if (this.EXCISE_AMOUNT_TXT.value == "" && this.EXCISE_ID.value.ToString().ToNumber<int>() > 0 && this.INV_CATEGORY != (int)InvCategory.Oil) return false;
                return true;
            };

            //  ---------------         LOCKS     ---------------------------------

            GOODS_NAME.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil) return true;
                return false;
            };

            BARCODE.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil || this.INV_CATEGORY == (int)InvCategory.Advance || this.INV_CATEGORY == (int)InvCategory.Wood) return true;
                return false;
            };

            UNIT_ID.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID > 0) return true;
                return false;
            };

            UNIT_TXT.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID > 0) return true;
                return false;
            };

            QUANTITY.isLockedFunction = () =>
            {
                if (this.isLocked() && (this.INV_TYPE != (int)InvoiceType.WithOutTrans && this.INV_TYPE != (int)InvoiceType.InternalTrans)) return true;  // თუ დისტრიბუციის ქვე დოკუმენტია რომლის ტიპიც განსხვავებულია ტრანს. გარეშე ან შიდა გადაზიდვისგან
                if (this.UNIT_ID.value == 16) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID > 0 && this.CORRECT_REASON_ID < 4)) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID == 4)) return false;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.SSF_CODE.value == "2713") return true;
                if (this.QUANTITY_MAX.value != null && this.QUANTITY_MAX.value.ToString().ToNumber<int>() > 0) return false; // დისტ. ქვე-დოკ. -> მაქსიმალური რაოდ. მეტია 0 ზე
                return false;
            };

            QUANTITY_EXT.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID > 0 && this.CORRECT_REASON_ID < 4)) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID == 4) return false;
                return false;
            };

            UNIT_PRICE.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.UNIT_ID.value == 16) return true;
                if (this.isInternalOilInv()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID > 0) return true;
                return false;
            };

            AMOUNT.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.isInternalOilInv()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID == 1 || this.CORRECT_REASON_ID == 2)) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID > 2)) return false;
                return false;
            };

            VAT_AMOUNT.isLockedFunction = () =>
            {
                return true;
            };

            VAT_TYPE.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.isInternalOilInv()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.CORRECT_REASON_ID > 0 && this.CORRECT_REASON_ID != 2)) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID == 2) return false;
                return false;
            };

            EXCISE_AMOUNT_TXT.isLockedFunction = () =>
            {
                if (this.isLocked()) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID > 0 && this.CORRECT_REASON_ID < 4) return true;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && this.CORRECT_REASON_ID == 4) return false;
                if (this.INV_CATEGORY == (int)InvCategory.Oil && (this.INV_TYPE == (int)InvoiceType.Retail || this.INV_TYPE == (int)InvoiceType.WholeSale)) return false;
                return true;
            };
        }
    }

}



public enum InvCategory
{
    Supply = 1,
    Wood = 2,
    Oil = 3,
    Advance = 4
}

public enum InvoiceType
{
    InternalTrans = 1,
    Transportation = 2,
    WithOutTrans = 3,
    Distribution = 4,
    Return = 5,
    AdvanceInvoice = 6,
    Retail = 7,
    WholeSale = 8,
    Import = 9,
    Export = 10,
    Service = 11
}

public enum TransType
{
    Automobile = 1,
    TransCar = 2,
    Other = 3,
    Railway = 4,
    Pipeline = 5,
    Coupons = 6,
    Card = 7,
    Copying = 8
}

public enum OilDocCategory
{
    Purchase = 1,
    Import = 2
}