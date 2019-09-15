using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using BaseLib.ExtensionMethods;
using Microsoft.Security.Application;

namespace BaseLib.Common
{
    public class Serializer
    {

        /// <summary>
        /// Serializes xml object to xml-like string.
        /// </summary>
        /// <param name="objectToXml"> </param>
        /// <param name="bIncludeNamespaces">Checks whether to include namespace info tag.</param>
        /// <returns>Returns xml string of object.</returns>
        public static string SerializeToXmlString(Object objectToXml, bool bIncludeNamespaces)
        {
            StreamWriter stWriter = null;
            string buffer;
            if (objectToXml == null) return null;
            try
            {
                var xmlSerializer = new XmlSerializer(objectToXml.GetType());
                var memStream = new MemoryStream();
                stWriter = new StreamWriter(memStream);

                if (!bIncludeNamespaces)
                {
                    var xs = new XmlSerializerNamespaces();
                    xs.Add("", "");//To remove namespace and any other inline information tag
                    xmlSerializer.Serialize(stWriter, objectToXml, xs);
                }
                else
                {
                    xmlSerializer.Serialize(stWriter, objectToXml);
                }

                buffer = Encoding.UTF8.GetString(memStream.GetBuffer());
            }
            finally
            {
                if (stWriter != null)
                    stWriter.Close();
            }

            return CommonFunctions.Clean(buffer);
        }

        /// <summary>
        /// Serializes object to xml file.
        /// </summary>
        /// <param name="objToXml">Object to serialize.</param>
        /// <param name="filePath">Path to xml file.</param>
        /// <param name="includeNameSpace">Checks whether to include namespace info tag.</param>
        public static void SerializeToXmlFile(Object objToXml, string filePath, bool includeNameSpace)
        {
            StreamWriter stWriter = null;
            if (objToXml == null) return;
            try
            {
                var xmlSerializer = new XmlSerializer(objToXml.GetType());
                stWriter = new StreamWriter(filePath);

                if (!includeNameSpace)
                {
                    var xs = new XmlSerializerNamespaces();
                    xs.Add("", "");//To remove namespace and any other inline information tag
                    xmlSerializer.Serialize(stWriter, objToXml, xs);
                }
                else
                {
                    xmlSerializer.Serialize(stWriter, objToXml);
                }
            }
            finally
            {
                if (stWriter != null)
                    stWriter.Close();
            }
        }

        /// <summary>
        /// Deserializes the object given with the type from the given string
        /// </summary>
        /// <param name="xmlString">String containing the serialized xml form of the object</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeFromXmlString<T>(string xmlString) where T : class
        {
            if (String.IsNullOrEmpty(xmlString)) return null;
            var xmlSerializer = new XmlSerializer(typeof(T));

            //mStream_is magivrad chavsvi XR
            //byte[] bytes = new byte[xmlString.Length];
            //Encoding.ASCII.GetBytes(xmlString, 0, xmlString.Length, bytes, 0);
            //mStream = new MemoryStream(bytes);
            XmlReader xr = XmlReader.Create(new StringReader(xmlString));

            object objFromXml = xmlSerializer.Deserialize(xr);
            return (T)objFromXml;
        }

        /// <summary>
        /// De serializes the object given with the type from the given string
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type">Type of the object to be deserialized</param>
        /// <returns>Deserialized object</returns>
        public static object DeserializeFromXmlFile(string filePath, Type type)
        {
            FileStream fStream = null;

            try
            {
                var xmlSerializer = new XmlSerializer(type);
                fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                object objFromXml = xmlSerializer.Deserialize(fStream);
                return objFromXml;
            }
            finally
            {
                if (fStream != null)
                    fStream.Close();
            }
        }

        /// <summary>
        /// De serializes the object given with the type from the given string
        /// </summary>
        /// <param name="xmlString">String containing the serialized xml form of the object</param>
        /// <param name="type">Type of the object to be deserialized</param>
        /// <returns>Deserialized object</returns>
        public static object DeserializeFromXmlString(string xmlString, Type type)
        {
            var xmlSerializer = new XmlSerializer(type);
            object objFromXml = xmlSerializer.Deserialize(new StringReader(xmlString));
            return objFromXml;
        }
    }

    public class JSON
    {
        public const int TokenNone = 0;
        public const int TokenCurlyOpen = 1;
        public const int TokenCurlyClose = 2;
        public const int TokenSquaredOpen = 3;
        public const int TokenSquaredClose = 4;
        public const int TokenColon = 5;
        public const int TokenComma = 6;
        public const int TokenString = 7;
        public const int TokenNumber = 8;
        public const int TokenTrue = 9;
        public const int TokenFalse = 10;
        public const int TokenNull = 11;

        private const int BuilderCapacity = 2000;

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List, a Dictionary, a double, a string, null, true, or false</returns>
        public static object JsonDecode(string json, bool sanitarizeHtml = true)
        {
            bool success = true;

            return JsonDecode(json, sanitarizeHtml, ref success);
        }

        /// <summary>
        /// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="success">Successful parse?</param>
        /// <returns>An List, a Dictionary, a double, a string, null, true, or false</returns>
        public static object JsonDecode(string json, bool sanitaraizeHtml, ref bool success)
        {
            success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                object value = ParseValue(charArray, sanitaraizeHtml, ref index, ref success);
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a Dictionary / List object into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary / List</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string JsonEncode(object json, bool encode)
        {
            var builder = new StringBuilder(BuilderCapacity);
            bool success = SerializeValue(json, builder, encode);
            return (success ? builder.ToString() : null);
        }

        protected static Dictionary<string, object> ParseObject(char[] json, bool sanitarizeHtml, ref int index, ref bool success)
        {
            var table = new Dictionary<string, object>();

            // {
            NextToken(json, ref index);

            //   bool done = false;
            while (true)
            {
                int token = LookAhead(json, index);
                if (token == TokenNone)
                {
                    success = false;
                    CommonFunctions.CatchExceptions(new Exception("JSON parser error Decode JSON. ParseObject"), string.Format("json={0} index={1}", CommonFunctions.CharToString(json), index));
                    return null;
                }
                if (token == TokenComma)
                {
                    NextToken(json, ref index);
                }
                else if (token == TokenCurlyClose)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {

                    // name
                    string name = ParseString(json, sanitarizeHtml, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    // :
                    token = NextToken(json, ref index);
                    if (token != TokenColon)
                    {
                        success = false;
                        CommonFunctions.CatchExceptions(new Exception("JSON parser error Decode JSON. TokenColon"), string.Format("json={0} index={1}", CommonFunctions.CharToString(json), index));
                        return null;
                    }

                    // value
                    object value = ParseValue(json, sanitarizeHtml, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    if (name != "__type")
                        table.Add(name, value);
                }
            }

            // return table;
        }

        protected static List<object> ParseArray(char[] json, bool sanitarizeHtml, ref int index, ref bool success)
        {
            var array = new List<object>();

            // [
            NextToken(json, ref index);

            while (true)
            {
                int token = LookAhead(json, index);
                if (token == TokenNone)
                {
                    success = false;
                    CommonFunctions.CatchExceptions(new Exception("JSON parser error Decode JSON. TokenColon"), string.Format("json={0} index={1}", CommonFunctions.CharToString(json), index));
                    return null;
                }
                if (token == TokenComma)
                {
                    NextToken(json, ref index);
                }
                else if (token == TokenSquaredClose)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, sanitarizeHtml, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        protected static object ParseValue(char[] json, bool sanitarizeHtml, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case TokenString:
                    return ParseString(json, sanitarizeHtml, ref index, ref success);
                case TokenNumber:
                    return ParseNumber(json, ref index, ref success);
                case TokenCurlyOpen:
                    return ParseObject(json, sanitarizeHtml, ref index, ref success);
                case TokenSquaredOpen:
                    return ParseArray(json, sanitarizeHtml, ref index, ref success);
                case TokenTrue:
                    NextToken(json, ref index);
                    return true;
                case TokenFalse:
                    NextToken(json, ref index);
                    return false;
                case TokenNull:
                    NextToken(json, ref index);
                    return null;
                case TokenNone:
                    break;
            }

            success = false;
            CommonFunctions.CatchExceptions(new Exception("JSON parser error Decode JSON. TokenNone"), string.Format("json={0} index={1}", CommonFunctions.CharToString(json), index));
            return null;
        }

        protected static string ParseString(char[] json, bool sanitarizeHtml, ref int index, ref bool success)
        {
            var s = new StringBuilder(BuilderCapacity);

            EatWhitespace(json, ref index);

            // "
            char c = json[index++];

            bool complete = false;
            while (!complete)
            {

                if (index == json.Length)
                {
                    break;
                }

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }

                if (c == '\\')
                {

                    if (index == json.Length)
                    {
                        break;
                    }
                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return "";
                            }
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    s.Append(c);
                }
            }

            if (!complete)
            {
                success = false;
                CommonFunctions.CatchExceptions(new Exception("JSON parser error Decode JSON. ParseString"), string.Format("json={0} index={1}", CommonFunctions.CharToString(json), index));
                return null;
            }

            return sanitarizeHtml ? HttpUtility.HtmlDecode(Sanitizer.GetSafeHtmlFragment(s.ToString().Replace(" ", "&nbsp;")).Replace("&nbsp;", " ")) : s.ToString();
        }

        protected static double ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);

            int lastIndex = GetLastIndexOfNumber(json, index);
            int charLength = (lastIndex - index) + 1;

            double number;
            success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out number);

            index = lastIndex + 1;
            return number;
        }

        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        protected static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        protected static int LookAhead(char[] json, int index)
        {
            int saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);

            if (index == json.Length)
            {
                return TokenNone;
            }

            char c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return TokenCurlyOpen;
                case '}':
                    return TokenCurlyClose;
                case '[':
                    return TokenSquaredOpen;
                case ']':
                    return TokenSquaredClose;
                case ',':
                    return TokenComma;
                case '"':
                    return TokenString;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TokenNumber;
                case ':':
                    return TokenColon;
            }
            index--;

            int remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' &&
                    json[index + 1] == 'a' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 's' &&
                    json[index + 4] == 'e')
                {
                    index += 5;
                    return TokenFalse;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' &&
                    json[index + 1] == 'r' &&
                    json[index + 2] == 'u' &&
                    json[index + 3] == 'e')
                {
                    index += 4;
                    return TokenTrue;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' &&
                    json[index + 1] == 'u' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 'l')
                {
                    index += 4;
                    return TokenNull;
                }
            }

            return TokenNone;
        }

        protected static bool SerializeValue(object value, StringBuilder builder, bool encode = true)
        {
            bool success = true;

            if (value is string || value is DateTime)
            {
                if (encode)
                    success = SerializeString(HttpUtility.HtmlDecode(Sanitizer.GetSafeHtmlFragment(value.ToString().Replace(" ", "&nbsp;")).Replace("&nbsp;", " ")), builder);
                else
                    success = SerializeString(value.ToString(), builder);
            }
            else if (value is Dictionary<string, object>)
            {
                success = SerializeObject((Dictionary<string, object>)value, builder, encode);
            }
            else if (value is IEnumerable)
            {
                success = SerializeArray((IEnumerable)value, builder, encode);
            }
            else if ((value is Boolean) && (Boolean)value)
            {
                builder.Append("true");
            }
            else if ((value is Boolean) && ((Boolean)value == false))
            {
                builder.Append("false");
            }
            else if (value is ValueType && value.ToString().IsNumber<double>())
            {
                success = SerializeNumber(value.ToString().ToNumber<double>(), builder);
            }
            else if (value is ValueType)
            {
                success = SerializeNumber(Convert.ToDouble(value), builder);
            }
            else if (value == null)
            {
                builder.Append("null");
            }
            else if (value.IsClass())
            {
                success = SerializeClass(value, builder, encode);
            }
            else
            {
                success = false;
            }

            if (success == false)
            {
                var e = new Exception("JSON parser error");
                CommonFunctions.CatchExceptions(e, string.Format("value can't parse={0} part of json={1}", Serializer.SerializeToXmlString(value, false), builder));
                throw e;
            }

            return success;
        }

        protected static bool SerializeObject(Dictionary<string, object> anObject, StringBuilder builder, bool encode)
        {
            builder.Append("{");

            IDictionaryEnumerator e = anObject.GetEnumerator();
            bool first = true;
            while (e.MoveNext())
            {
                string key = e.Key.ToString();
                object value = e.Value == null || string.IsNullOrEmpty(e.Value.ToString()) ? null : e.Value;

                if (!first)
                {
                    builder.Append(", ");
                }

                SerializeString(key, builder);
                builder.Append(":");
                if (!SerializeValue(value, builder, encode))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("}");
            return true;
        }

        protected static bool SerializeClass(object anObject, StringBuilder builder, bool encode)
        {
            builder.Append("{");

            var type = anObject.GetType();
            var props = type.GetProperties();

            bool first = true;
            foreach (var prop in props)
            {
                string key = prop.Name;
                object value = prop.GetValue(anObject, null);

                if (!first)
                {
                    builder.Append(", ");
                }

                SerializeString(key, builder);
                builder.Append(":");
                if (!SerializeValue(value, builder, encode))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("}");
            return true;
        }

        protected static bool SerializeArray(IEnumerable anArray, StringBuilder builder, bool encode)
        {
            builder.Append("[");

            bool first = true;
            foreach (object value in anArray)
            {
                if (!first)
                {
                    builder.Append(", ");
                }

                if (!SerializeValue(value, builder, encode))
                {
                    return false;
                }

                first = false;
            }

            builder.Append("]");
            return true;
        }

        protected static bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");

            char[] charArray = aString.ToCharArray();
            foreach (char c in charArray)
            {
                if (c == '"')
                {
                    builder.Append("\\\"");
                }
                else if (c == '\\')
                {
                    builder.Append("\\\\");
                }
                else if (c == '\b')
                {
                    builder.Append("\\b");
                }
                else if (c == '\f')
                {
                    builder.Append("\\f");
                }
                else if (c == '\n')
                {
                    builder.Append("\\n");
                }
                else if (c == '\r')
                {
                    builder.Append("\\r");
                }
                else if (c == '\t')
                {
                    builder.Append("\\t");
                }
                else
                {
                    builder.Append(c);
                }
            }

            builder.Append("\"");
            return true;
        }

        protected static bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(Convert.ToString(number, Thread.CurrentThread.CurrentCulture));
            return true;
        }
    }
}
