using BaseLib.Exceptions;
using BaseLib.OraDataBase;
using EservicesLib.OraDatabase.StoredProcedures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace EservicesLib.Models.Invoice
{
    public class Field<T>
    {
        public Field(T val, FieldGroup group = null)
        {
            this.isValidFunctionParams = (Object param) => { return this.isValidFunction(); };
            this.value = val;
        }
        public Field(FieldGroup group = null)
        {
            this.isValidFunctionParams = (Object param) => { return this.isValidFunction(); };
            if (typeof(T) == typeof(int))
                this.value = (T)Convert.ChangeType(0, typeof(T));
            else if (typeof(T) == typeof(string))
                this.value = (T)Convert.ChangeType("", typeof(T));
        }

        public FieldGroup group = new FieldGroup();

        public T value { get; set; }

        public Type GetTypeOfValue()
        {
            return typeof(T);
        }
        //params:Object = null
        public Func<bool> isLockedFunction = () => { return false; };

        public Func<Object, bool> isValidFunctionParams = (Object param) => { return true; };
        public Func<bool> isValidFunction = () => { return true; };
        public Func<bool> isVisibleFunction = () => { return true; };

        private string _errorMessage = "";

        public string errorMessage { get {
                return this._errorMessage;
            } set {
                this._errorMessage = value;
                if (value.Length > 0 && !String.IsNullOrEmpty(value))
                    throw new UserExceptions(errorMessage);
            }
        }
        public bool isVisibleGroup()
        {
            return this.group.isVisibleGroup();
        }
        public bool isLocked(Object param = null)
        {
            return this.isLockedFunction();
        }
        public bool isValid(Object param = null)
        {
            this.errorMessage = "";
            if (param == null)
                return this.isValidFunction();
            else
                return this.isValidFunctionParams(param);
        }

        public bool isVisible()
        {
            return this.isVisibleFunction();
        }
        public static bool validateAll(Object a, Object param, string json = null)
        {
            foreach (FieldInfo i in a.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var isField = i.GetValue(a).GetType().GetProperty("value", BindingFlags.Public | BindingFlags.Instance);

                var isList = i.GetValue(a) is IList;
                if (isList == true)
                {
                    foreach (var x in (IList)i.GetValue(a)) {
                        if (!validateAll(x,null,x.ToString() + " JSON: {" + json + "}")) return false;
                    }
                }

                if (isField == null) continue;

                var isValidMethod = i.GetValue(a).GetType().GetMethod("isValid", BindingFlags.Public | BindingFlags.Instance);
                var isVisibleMethod = i.GetValue(a).GetType().GetMethod("isVisible", BindingFlags.Public | BindingFlags.Instance);
                var isVisibleGroupMethod = i.GetValue(a).GetType().GetMethod("isVisibleGroup");

                if (isValidMethod != null && isVisibleMethod != null && isVisibleGroupMethod != null)
                {
                    object[] x = new object[] { param };

                    if (!(bool)isVisibleGroupMethod.Invoke(i.GetValue(a), null) || !(bool)isVisibleMethod.Invoke(i.GetValue(a), null)) continue; // თუ ეს ველი, ან მისი ჯგუფი არ ჩანს ვალიდაცია არ გაიაროს


                    if (!(bool)isValidMethod.Invoke(i.GetValue(a), x))
                    {
                        DataProviderManager<PKG_INVOICE>.Provider.log_api_debug(json,i.Name + " is Invalid");
                        //throw new Exception(i.Name + " is Invalid");
                    }
                }
            }
            //if (a.GetType().GetProperty(i.Name.ToString()) != null && a.GetType().GetProperty(i.Name.ToString()).GetValue(a, null).GetType().Name is Field<T>)
            //if (!a.GetType().GetProperty(i.Name.ToString()).GetValue(a, null).isValid(param))
            //if (a[i].errorMessage.length > 0)
            //{
            //window.alert(a[i].errorMessage);
            //return false;
            //}
            //else
            //return false;
            // }
            return true;
        }
    }

    public class FieldGroup
    {
        public FieldGroup()
        {
        }
        public Func<bool> isVisibleFunction = () => { return true; };
        public bool isVisibleGroup()
        {
            return this.isVisibleFunction();
        }
    }
}