using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using BaseLib.Exceptions;
using Oracle.DataAccess.Client;

namespace BaseLib.ExtensionMethods
{
    public static class Extensions
    {
        public static bool In<T>(this T source, params T[] items)
        {
            if (items == null) throw new ArgumentNullException("items");
            return (Array.IndexOf(items, source) != -1);
            //return items.e(item);
        }

        public static bool NotIn<T>(this T source, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            return (Array.IndexOf(items, source) == -1);
            //return items.e(item);
        }

        public static bool ContainsAny(this string source, params string[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            return items.Any(source.Contains);
        }

        public static bool ContainsAll(this string source, params string[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            return items.All(source.Contains);
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static void AddXml(this DataTable dt, object[] ob)
        {
            for (var i = 0; i < ob.Length; i++)
            {
                ob[i] = HttpUtility.HtmlEncode(ob[i]);
            }

            dt.Rows.Add(ob);
        }

        public static void Fill(this DataTable dt, OracleDataReader dr)
        {
            if (dt.Columns.Count == 0)
            {
                for (var i = 0; i < dr.FieldCount; i++)
                {
                    dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                }
            }

            var datarow = dt.NewRow();
            for (var i = 0; i < dr.FieldCount; i++)
            {
                datarow[i] = dr[i];
            }

            dt.Rows.Add(datarow);
        }

        public static T ToNumber<T>(this string input) where T : struct
        {
            try
            {
                input = input.Replace(',', '.');
                if (string.IsNullOrEmpty(input.Trim()) || input == "null") input = "0";

                var paramTypes = new[] { typeof(string), typeof(T).MakeByRefType() };
                var method = typeof(T).GetMethod("TryParse", paramTypes);
                var parameters = new Object[] { input, null };
                if (method != null)
                {
                    var val = (bool)method.Invoke(typeof(T), parameters);
                    if (val)
                        return (T)parameters[1];
                }
                throw new Exception();
            }
            catch (Exception)
            {
                string typeStr;
                switch (typeof(T).Name.ToLower())
                {
                    case "int32": goto case "int64";
                    case "int64": typeStr = "მთელი"; break;
                    case "double": typeStr = "წილადური"; break;
                    default:
                        typeStr = typeof(T).Name; break;
                }
                // CommonFunctions.CatchExceptions(ex, "ToNumber Error : Type=" + typeof(T).Name + " Value=" + input, false);
                throw new UserExceptions("არა-სწორი " + typeStr + " რიცხვი : " + input);
            }
        }

        public static string ObjToString(this object s)
        {
            var result = "";
            if (s != null) result = s.ToString() == "null" ? "" : s.ToString();
            return result;
        }

        public static bool IsNumber<T>(this string input) where T : struct
        {
            try
            {
                var paramTypes = new[] { typeof(string), typeof(T).MakeByRefType() };
                var method = typeof(T).GetMethod("TryParse", paramTypes);
                var parameters = new Object[] { input, null };
                if (method != null)
                {
                    var val = (bool)method.Invoke(typeof(T), parameters);
                    return val;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static double Ceiling(this double input, int digit)
        {
            var con = Math.Pow(10, digit);

            if (digit == 0)
                return input;

            return Math.Ceiling(input * con) / con;

        }

        public static T JsonDeserilization<T>(this string json) where T : class
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public static string JsonSerilization(this object json)
        {
            return new JavaScriptSerializer().Serialize(json);
        }

        public static string MakeValueCsvFriendly(this object value)
        {
            if (value == null) return "";
            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }

        public static int BoolToNum(this string s)
        {
            bool b;
            if (Boolean.TryParse(s, out b))
            {
                if (b) return 1;
                else return 0;
            }

            return -1;
        }

        public static bool IsClass(this object obj)
        {
            if (obj == null)
                return false;

            Type type = obj.GetType();

            if (type.IsClass)
            {
                var props = type.GetProperties();
                return props.Length > 0;
            }

            return false;
        }

        public static string RemoveNonPrintableChars(this string value)
        {
            //char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            //var newstr = new String(value.Where(c => !invalid.Contains(c)).ToArray());

            var newstr = new String(value.Where(c => !Char.IsControl(c)).ToArray());

            return newstr;
        }
    }
}
